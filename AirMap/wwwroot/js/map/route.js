// wwwroot/js/map/route.js

import { aStar } from "./astar.js";
import SessionCache from "../utils/sessions.js";

async function fetchRouteBetweenWaypoints(waypoints) {
    const fullPath = [];

    for (let i = 0; i < waypoints.length - 1; i++) {
        const a = waypoints[i];
        const b = waypoints[i + 1];
        const url =
            `https://router.project-osrm.org/route/v1/driving/${a.lng},${a.lat};${b.lng},${b.lat
                }?overview=full&geometries=geojson`;

        try {
            const res = await fetch(url);
            const data = await res.json();

            if (data.routes?.[0]?.geometry?.coordinates) {
                const coords = data.routes[0].geometry.coordinates.map(([lng, lat]) =>
                    L.latLng(lat, lng)
                );
                fullPath.push(...coords);
            }
        } catch (err) {
            console.error("OSRM fetch error:", err);
        }
    }

    return fullPath;
}

function calculateAirScore(routePoints, goodMarkers, radiusMeters = 500) {
    let score = 0;
    for (const point of routePoints) {
        for (const marker of goodMarkers) {
            const dist = point.distanceTo(marker);
            if (dist <= radiusMeters) {
                score++;
                break;
            }
        }
    }
    return score;
}

let routeLine = null;
let destinationMarker = null;

export function removeRouteLine(map) {
    if (routeLine) {
        map.removeLayer(routeLine);
        routeLine = null;
        if (SessionCache.exists("routeLine")) SessionCache.remove("routeLine");
    }
}

export function removeDestinationMarker(map) {
    if (destinationMarker) {
        map.removeLayer(destinationMarker);
        destinationMarker = null;
    }
}

export function setupRouting(map, getStartLatLng, goodMarkers) {
    map.on("contextmenu",
        async function(e) {
            removeRouteLine(map);

            const destLatLng = e.latlng;
            const userLatLng = getStartLatLng();
            if (!userLatLng) {
                alert("Punkt startowy nie jest ustawiony.");
                return;
            }

            if (destinationMarker) map.removeLayer(destinationMarker);
            destinationMarker = L.marker(destLatLng)
                .addTo(map)
                .bindPopup("Nowy punkt docelowy")
                .openPopup();

            const graphPoints = [userLatLng, ...goodMarkers, destLatLng];
            const aStarPath = aStar(userLatLng, destLatLng, graphPoints);

            if (!aStarPath || aStarPath.length < 2) {
                alert("Nie znaleziono żadnej sensownej ścieżki.");
                return;
            }

            const route = await fetchRouteBetweenWaypoints(aStarPath);

            if (!route || route.length < 2) {
                alert("Nie udało się pobrać trasy drogowej.");
                return;
            }

            const airScore = calculateAirScore(route, goodMarkers);
            const qualityPercent = Math.round((airScore / route.length) * 100);
            const color = "green";

            routeLine = L.polyline(route, { color, weight: 5 }).addTo(map);

            SessionCache.set("routeLine", routeLine.getLatLngs());

            console.log(
                `Trasa: ${route.length} punktów, z czego ${airScore} blisko czystego powietrza (${qualityPercent}%)`
            );
        });
}