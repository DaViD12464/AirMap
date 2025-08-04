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
    if (routeLine) {
        map.removeLayer(routeLine);
        routeLine = null;
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

export function setupRouting(map, getStartLatLng, goodMarkers) {
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

            const filters = SessionCache.get("routeFilters") || {};
            const typTrasy =
                filters.selectedRoute === "cleanest"
                    ? "Najczystsza ścieżka"
                    : "Najkrótsza ścieżka";
            const zapetlenie = filters.loopedRoute ? ", zapętlona" : "";
            const dlugosc = filters.lengthLimiter
                ? `, długość ograniczona do ${filters.lengthLimiter} km`
                : "";
            const sensory = filters.activeSensors
                ? `, sensory PM: [${filters.activeSensors.join(", ")}]`
                : "";

            console.log(
                `Trasa (${typTrasy}${zapetlenie}${dlugosc}${sensory}): ${route.length} punktów, z czego ${airScore
                } blisko czystego powietrza (${qualityPercent}%)`
            );
        });
}

function getFilteredMarkers(sensorData, activeSensors) {
    return sensorData.filter((sensor) => {
        if (!sensor.type) return false;
        return activeSensors.includes(sensor.type);
    });
}

window.onRouteFiltersChanged = async function() {
    const map = window._leafletMapInstance;
    if (!map) return;

    const filters = SessionCache.get("routeFilters") || {};
    const {
        selectedRoute = "shortest",
        loopedRoute = false,
        rangeLimiter = 100,
        lengthLimiter = 100,
        activeSensors = ["pm1", "pm25", "pm10", "pm4", "HcHo", "unknownAirQuality"],
    } = filters;

    let sensorData = window._sensorData || [];
    if (!sensorData.length && typeof fetch === "function") {
        try {
            sensorData = await fetch("/api/sensor/GetAllSensorData").then((r) =>
                r.json()
            );
        } catch (e) {
            sensorData = [];
        }
    }

    const filteredMarkers = sensorData.filter((sensor) =>
        activeSensors.includes(sensor.type)
    );
    const goodMarkers = filteredMarkers
        .filter((sensor) => {
            return ["ModerateAQ", "GoodAQ", "VeryGoodAQ"].some((q) =>
                (sensor.iconName || "").includes(q)
            );
        })
        .map((sensor) => L.latLng(sensor.latitude, sensor.longitude));

    const userLatLng = SessionCache.get("startLatLng");
    const destLatLng = SessionCache.get("destinationMarker");
    if (!userLatLng || !destLatLng) return;

    if (typeof removeRouteLine === "function") removeRouteLine(map);

    let graphPoints = [userLatLng, ...goodMarkers, destLatLng];
    let aStarPath;
    if (selectedRoute === "shortest") {
        graphPoints = [userLatLng, destLatLng];
        aStarPath = [userLatLng, destLatLng];
    } else if (selectedRoute === "cleanest") {
        aStarPath = window.aStar
            ? window.aStar(userLatLng, destLatLng, graphPoints)
            : null;
    } else {
        aStarPath = [userLatLng, destLatLng];
    }

    if (loopedRoute && aStarPath && aStarPath.length > 1) {
        aStarPath.push(userLatLng);
    }

    if (aStarPath && aStarPath.length > 1) {
        let totalDist = 0;
        const limitedPath = [aStarPath[0]];
        for (let i = 1; i < aStarPath.length; i++) {
            const prev = aStarPath[i - 1];
            const curr = aStarPath[i];
            const dist = window.haversineDistance
                ? window.haversineDistance(prev, curr)
                : 0;
            if (totalDist + dist > lengthLimiter) break;
            totalDist += dist;
            limitedPath.push(curr);
        }
        aStarPath = limitedPath;
    }

    let route = [];
    if (aStarPath && aStarPath.length > 1) {
        if (typeof fetchRouteBetweenWaypoints === "function") {
            route = await fetchRouteBetweenWaypoints(aStarPath);
        } else {
            route = aStarPath;
        }
    }

    if (route && route.length > 1) {
        const color = selectedRoute === "cleanest" ? "#00b894" : "#0984e3";
        if (window.routeLine) map.removeLayer(window.routeLine);
        window.routeLine = L.polyline(route, { color, weight: 5 }).addTo(map);
        SessionCache.set("routeLine", window.routeLine.getLatLngs());
    }
};