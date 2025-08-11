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

function isNearAny(point, routePoints, maxDistanceMeters = 100) {
    if (!routePoints || routePoints.length === 0) return false;
    for (const r of routePoints) {
        if (point.distanceTo(r) <= maxDistanceMeters) return true;
    }
    return false;
}

function pushIfNotClose(arr, point, minDistMeters = 1) {
    for (const p of arr) {
        if (p.distanceTo(point) <= minDistMeters) return;
    }
    arr.push(point);
}

let routeLine = null;
let loopedRouteLine = null;
let destinationMarker = null;

export function restoreRouteLine(map) {
    const cachedRoute = SessionCache.get("routeLine");
    if (cachedRoute && cachedRoute.length > 1) {
        routeLine = L.polyline(cachedRoute, { color: "green", weight: 5 }).addTo(map);
        window.routeLine = routeLine;
    }
    const cachedLooped = SessionCache.get("loopedRouteLine");
    if (cachedLooped && cachedLooped.length > 1) {
        const filters = SessionCache.get("routeFilters") || {};
        const primaryColor = (filters.selectedRoute === "cleanest")
            ? "#00b894"
            : (filters.selectedRoute === "shortest" ? "#0984e3" : "#636e72");
        loopedRouteLine = L.polyline(cachedLooped, { color: primaryColor, weight: 5, opacity: 0.5 }).addTo(map);
        window.loopedRouteLine = loopedRouteLine;
    }
}

export function removeRouteLine(map) {
    if (window.routeLine) {
        try {
            map.removeLayer(window.routeLine);
        } catch (e) {
        }
        window.routeLine = null;
        routeLine = null;
    }
    if (SessionCache.exists("routeLine")) SessionCache.remove("routeLine");

    if (window.loopedRouteLine) {
        try {
            map.removeLayer(window.loopedRouteLine);
        } catch (e) {
        }
        window.loopedRouteLine = null;
        loopedRouteLine = null;
    }
    if (SessionCache.exists("loopedRouteLine")) SessionCache.remove("loopedRouteLine");
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
            } else {
                graphPoints = [userLatLng, ...goodMarkers, destLatLng];
            }

            const aStarPath = aStar(
                userLatLng,
                destLatLng,
                graphPoints,
                filters.rangeLimiter,
                goodMarkers,
                filters.selectedRoute === "cleanest",
                allMarkersLatLng.filter(p => !goodMarkers.includes(p))
            );

            if (!aStarPath || aStarPath.length < 2) {
                alert("Nie znaleziono żadnej sensownej ścieżki.");
                return;
            }

            const primaryRoute = await fetchRouteBetweenWaypoints(aStarPath);

            if (!primaryRoute || primaryRoute.length < 2) {
                alert("Nie udało się pobrać trasy drogowej.");
                return;
            }

            SessionCache.set("primaryRoute", { path: aStarPath, route: primaryRoute });
            const primaryColor = routeColors[filters.selectedRoute] || routeColors.default;
            window.routeLine = L.polyline(primaryRoute, { color: primaryColor, weight: 5 }).addTo(map);
            SessionCache.set("routeLine", window.routeLine.getLatLngs());

            if (!filters.loopedRoute) {
                const airScore = calculateAirScore(primaryRoute, goodMarkers);
                const qualityPercent = Math.round((airScore / primaryRoute.length) * 100);
                console.log(`Trasa: ${primaryRoute.length} pkt, ${airScore} blisko czystego (${qualityPercent}%)`);
                return;
            }

            const attemptDistances = [200, 150, 120, 100, 80, 60, 40];
            let foundAltPath = null;
            let foundAltRoute = null;

            const primaryPathNodes = aStarPath.slice();

            for (const distMeters of attemptDistances) {
                const filteredNodes = graphPoints.filter(p => {
                    if (p.distanceTo(userLatLng) < 0.5) return true;
                    if (p.distanceTo(destLatLng) < 0.5) return true;
                    return !isNearAny(p, primaryRoute, distMeters);
                });

                const altGraph = [];
                pushIfNotClose(altGraph, destLatLng);
                for (const p of filteredNodes) pushIfNotClose(altGraph, p);
                pushIfNotClose(altGraph, userLatLng);

                try {
                    const alt = aStar(
                        destLatLng,
                        userLatLng,
                        altGraph,
                        filters.rangeLimiter,
                        goodMarkers,
                        filters.selectedRoute === "cleanest",
                        primaryPathNodes
                    );

                    if (alt && alt.length >= 2) {
                        const altR = await fetchRouteBetweenWaypoints(alt);
                        if (altR && altR.length >= 2) {
                            foundAltPath = alt;
                            foundAltRoute = altR;
                            console.log(
                                `Znaleziono trasę powrotną na progu ${distMeters}m, długość punków: ${alt.length}`);
                            break;
                        } else {
                            console.log("alt znaleziono, ale nie pobrano geometria (OSRM).");
                        }
                    } else {
                        console.log("Brak alt path na tym progu.");
                    }
                } catch (err) {
                    console.warn("Błąd podczas aStar (alt):", err);
                }
            }

            if (!foundAltRoute) {
                console.warn(
                    "Nie znaleziono wystarczająco innej trasy powrotnej - stosuję fallback (odwrócona trasa podstawowa).");
                const reversedPath = aStarPath.slice().reverse();
                const reversedRoute = await fetchRouteBetweenWaypoints(reversedPath);
                if (reversedRoute && reversedRoute.length >= 2) {
                    foundAltPath = reversedPath;
                    foundAltRoute = reversedRoute;
                } else {
                    console.error("Fallback też nie zwrócił geometrii. Brak zapętlonej trasy.");
                    foundAltPath = null;
                    foundAltRoute = null;
                }
            }

            if (foundAltRoute && foundAltRoute.length >= 2) {
                window.loopedRouteLine =
                    L.polyline(foundAltRoute, { color: primaryColor, weight: 5, opacity: 0.5 }).addTo(map);
                SessionCache.set("loopedRouteLine", window.loopedRouteLine.getLatLngs());
                SessionCache.set("returnRoute", { path: foundAltPath, route: foundAltRoute });
                console.log("Trasa powrotna (zapętlona) narysowana.");
            } else {
                console.warn("Nie narysowano zapętlonej trasy - pozostała tylko trasa podstawowa.");
            }

        });
}

window.onRouteFiltersChanged = function() {
    const filters = SessionCache.get("routeFilters") || {};
    SessionCache.set("routeFilters", filters);
    console.log("Zapisano nowe filtry trasy:", filters);
};