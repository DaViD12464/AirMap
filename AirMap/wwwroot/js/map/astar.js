// wwwroot/js/map/astar.js

import { haversineDistance } from "./distance.js";

export function aStar(start, goal, nodes) {
    const openSet = [start];
    const cameFrom = new Map();
    const gScore = new Map(nodes.map(n => [n, Infinity]));
    const fScore = new Map(nodes.map(n => [n, Infinity]));

    gScore.set(start, 0);
    fScore.set(start, haversineDistance(start, goal));

    while (openSet.length > 0) {
        openSet.sort((a, b) => fScore.get(a) - fScore.get(b));
        const current = openSet.shift();

        if (haversineDistance(current, goal) < 1) {
            const path = [goal];
            let curr = current;
            while (cameFrom.has(curr)) {
                path.unshift(curr);
                curr = cameFrom.get(curr);
            }
            path.unshift(start);
            return path;
        }

        for (let neighbor of nodes) {
            if (neighbor === current) continue;
            const dist = haversineDistance(current, neighbor);
            if (dist > 1000) continue; // km

            const tentativeG = gScore.get(current) + dist;
            if (tentativeG < gScore.get(neighbor)) {
                cameFrom.set(neighbor, current);
                gScore.set(neighbor, tentativeG);
                fScore.set(neighbor, tentativeG + haversineDistance(neighbor, goal));
                if (!openSet.includes(neighbor)) openSet.push(neighbor);
            }
        }
    }

    return null;
}