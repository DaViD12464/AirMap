// wwwroot/js/map/astar.js

import { haversineDistance } from "./distance.js";

// Pomocnicza funkcja do generowania klucza
function latLngKey(latlng) {
    return `${latlng.lat},${latlng.lng}`;
}

export function aStar(start, goal, nodes, maxDistanceKm = 100) {
    if (!start || !goal || !Array.isArray(nodes) || nodes.length === 0)
        return null;

    const openSet = [latLngKey(start)];
    const nodeMap = new Map(nodes.map((n) => [latLngKey(n), n]));
    const cameFrom = new Map();
    const gScore = new Map(nodes.map((n) => [latLngKey(n), Infinity]));
    const fScore = new Map(nodes.map((n) => [latLngKey(n), Infinity]));

    gScore.set(latLngKey(start), 0);
    fScore.set(latLngKey(start), haversineDistance(start, goal));

    while (openSet.length > 0) {
        openSet.sort((a, b) => fScore.get(a) - fScore.get(b));
        const currentKey = openSet.shift();
        const current = nodeMap.get(currentKey);

        if (haversineDistance(current, goal) < 1) {
            const path = [goal];
            let currKey = currentKey;
            while (cameFrom.has(currKey)) {
                path.unshift(nodeMap.get(currKey));
                currKey = cameFrom.get(currKey);
            }
            path.unshift(start);
            return path;
        }

        for (let neighbor of nodes) {
            const neighborKey = latLngKey(neighbor);
            if (neighborKey === currentKey) continue;
            const dist = haversineDistance(current, neighbor);
            if (dist > maxDistanceKm) continue;

            const tentativeG = gScore.get(currentKey) + dist;
            if (tentativeG < gScore.get(neighborKey)) {
                cameFrom.set(neighborKey, currentKey);
                gScore.set(neighborKey, tentativeG);
                fScore.set(neighborKey, tentativeG + haversineDistance(neighbor, goal));
                if (!openSet.includes(neighborKey)) openSet.push(neighborKey);
            }
        }
    }

    return null;
}