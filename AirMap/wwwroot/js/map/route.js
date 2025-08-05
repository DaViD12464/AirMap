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

export function restoreRouteLine(map) {
    const cachedRoute = SessionCache.get("routeLine");
    if (cachedRoute && cachedRoute.length > 1) {
        routeLine = L.polyline(cachedRoute, { color: "green", weight: 5 }).addTo(
            map
        );
    }
}


export function removeRouteLine(map) {
    if (window.routeLine) {
        map.removeLayer(window.routeLine);
        window.routeLine = null;
    }
    if (SessionCache.exists("routeLine")) SessionCache.remove("routeLine");
}

export function restoreDestinationMarker(map) {
    const cachedMarker = SessionCache.get("destinationMarker");
    if (cachedMarker) {
        destinationMarker = L.marker(cachedMarker)
            .addTo(map)
            .bindPopup("Nowy punkt docelowy")
            .openPopup();
    }
}

export function removeDestinationMarker(map) {
    if (destinationMarker) {
        map.removeLayer(destinationMarker);
        destinationMarker = null;
    }
    if (SessionCache.exists("destinationMarker"))
        SessionCache.remove("destinationMarker");
}

const routeColors = {
    shortest: "#0984e3",
    cleanest: "#00b894",
    default: "#636e72",
};

function generateGridPoints(start, end, spacingDegrees = 0.001) {
    const points = [];

    const minLat = Math.min(start.lat, end.lat);
    const maxLat = Math.max(start.lat, end.lat);
    const minLng = Math.min(start.lng, end.lng);
    const maxLng = Math.max(start.lng, end.lng);

    for (let lat = minLat; lat <= maxLat; lat += spacingDegrees) {
        for (let lng = minLng; lng <= maxLng; lng += spacingDegrees) {
            points.push(L.latLng(lat, lng));
        }
    }

    return points;
}

export function setupRouting(map, getStartLatLng, goodMarkers, allMarkersLatLng) {
    if (map._hasRoutingHandler) return;
    map._hasRoutingHandler = true;

    map.on("contextmenu",
        async function(e) {
            removeRouteLine(map);

            const destLatLng = e.latlng;
            const userLatLng = getStartLatLng();
            if (!userLatLng) {
                alert("Punkt startowy nie jest ustawiony.");
                return;
            }

            removeDestinationMarker(map);
            destinationMarker = L.marker(destLatLng)
                .addTo(map)
                .bindPopup("Nowy punkt docelowy")
                .openPopup();
            SessionCache.set("destinationMarker", destLatLng);

            const filters = SessionCache.get("routeFilters") || {};
            let graphPoints;
            if (filters.selectedRoute === "shortest") {
                graphPoints = [userLatLng, ...allMarkersLatLng, destLatLng];
            }
            else {
                graphPoints = [userLatLng, ...goodMarkers, destLatLng];
            }

            const aStarPath = aStar(userLatLng, destLatLng, graphPoints, filters.rangeLimiter , goodMarkers, filters.selectedRoute === "cleanest", allMarkersLatLng.filter(p => !goodMarkers.includes(p)));

            if (!aStarPath || aStarPath.length < 2) {
                alert("Nie znaleziono żadnej sensownej ścieżki.");
                return;
            }

            const route = await fetchRouteBetweenWaypoints(aStarPath);

            if (!route || route.length < 2) {
                alert("Nie udało się pobrać trasy drogowej.");
                return;
            }

            const totalLengthMeters = aStarPath.reduce((acc, curr, i, arr) => {
                if (i === 0) return 0;
                return acc + getDistance(arr[i - 1], curr);
            }, 0);

            const totalLengthKm = totalLengthMeters / 1000;

            if (filters.lengthLimiter && totalLengthKm > filters.lengthLimiter) {
                alert(
                    `Trasa przekracza dozwolony limit długości: ${totalLengthKm.toFixed(2)} km > ${filters.lengthLimiter} km`
                );
                removeRouteLine(map);
                removeDestinationMarker(map);
                return;
            }

            const airScore = calculateAirScore(route, goodMarkers);
            const qualityPercent = Math.round((airScore / route.length) * 100);

            const color = routeColors[filters.selectedRoute] || routeColors.default;

            removeRouteLine(map);

            window.routeLine = L.polyline(route, { color, weight: 5 }).addTo(map);
            SessionCache.set("routeLine", window.routeLine.getLatLngs());

            const typTrasy =
                filters.selectedRoute === "cleanest"
                    ? "Najczystsza ścieżka"
                    : "Najkrótsza ścieżka";
            const loopedRoute = filters.loopedRoute ? ", zapętlona" : "";
            const lengthLimiter = filters.lengthLimiter
                ? `, długość ograniczona do ${filters.lengthLimiter} km`
                : "";
            const activeSensors = filters.activeSensors
                ? `, sensory PM: [${filters.activeSensors.join(", ")}]`
                : "";

            console.log(
                `Trasa (${typTrasy}${loopedRoute}${lengthLimiter}${activeSensors}): ${route.length} punktów, z czego ${airScore
                } blisko czystego powietrza (${qualityPercent}%)`
            );
        });
}

window.onRouteFiltersChanged = function() {
    const filters = SessionCache.get("routeFilters") || {};
    SessionCache.set("routeFilters", filters);
    // Możesz dodać tu log, jeśli chcesz widzieć zmiany filtrów:
    console.log("Zapisano nowe filtry trasy:", filters);
};

function getDistance(point1, point2) {
    const R = 6371e3;
    const toRad = (deg) => deg * Math.PI / 180;

    const lat1 = toRad(point1.lat);
    const lat2 = toRad(point2.lat);
    const deltaLat = toRad(point2.lat - point1.lat);
    const deltaLng = toRad(point2.lng - point1.lng);

    const a =
        Math.sin(deltaLat / 2) * Math.sin(deltaLat / 2) +
            Math.cos(lat1) * Math.cos(lat2) *
            Math.sin(deltaLng / 2) * Math.sin(deltaLng / 2);

    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return R * c;
}
