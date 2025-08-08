// wwwroot/js/map/init.js
import {
    setupRouting,
    removeRouteLine,
    removeDestinationMarker,
    restoreRouteLine,
    restoreDestinationMarker,
} from "./route.js";
import SessionCache from "../utils/sessions.js";
import createLocateControl from "./controls/locateControl.js";
import topRightControl from "./controls/menu.js";
import { aStar } from "./astar.js";
import { haversineDistance } from "./distance.js";
import { prepareMarkers } from "../utils/sensor.js";

//import loading from loadingWindow.js
import { updateProgress } from "../utils/loadingWindow.js"

document.addEventListener("DOMContentLoaded",
    async function mainFunction() {
        const map = L.map("map").setView([52.1143385, 19.4236714], 7);
        let currentStartLatLng = null;

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
            {
                maxZoom: 19,
                attribution: "© OpenStreetMap",
            }).addTo(map);

        updateProgress(); //map loaded - update step #1
        /*console.log("Step#1 - updated progress.");*/

        const sensorData = await fetch("/api/sensor/GetAllSensorData")
            .then((res) => res.json())
            .catch((err) => {
                console.error("Error loading sensor data:", err);
                return [];
            });

        updateProgress(); //sensor data fetched - update step #2
        //console.log("Step#2 - updated progress.");

        const [iconNames, textValues] = await Promise.all([
            fetch("/api/sensor/GetIconDataBatch",
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(sensorData),
                }).then((r) => r.json()),
            fetch("/api/sensor/GetPopUpDataBatch",
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(sensorData),
                }).then((r) => r.json()),
        ]);

        updateProgress(); //Fetch icon batch in parallel - update step #3
        //console.log("Step#3 - updated progress.");
        updateProgress(); //Fetch popup data batch in parallel - update step #4
        //console.log("Step#4 - updated progress.");

        const icons = {
            veryGoodAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/VeryGoodAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            goodAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/GoodAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            moderateAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/ModerateAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            sufficientAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/SufficientAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            badAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/BadAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            veryBadAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/VeryBadAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            unknownAirQualityIcon: L.icon({
                iconUrl: "/AirQualityMarkers/UnknownAQ.png",
                iconSize: [64, 64],
                iconAnchor: [32, 64],
                popupAnchor: [6, -48],
            }),
            defaultGreen: L.icon({
                iconUrl: "https://maps.gstatic.com/mapfiles/ms2/micons/green-dot.png",
                iconSize: [32, 32],
                iconAnchor: [16, 32],
                popupAnchor: [0, -32],
            }),
            defaultBlue: L.icon({
                iconUrl: "https://maps.gstatic.com/mapfiles/ms2/micons/blue-dot.png",
                iconSize: [32, 32],
                iconAnchor: [16, 32],
                popupAnchor: [0, -32],
            }),
        };

        const filters = {
            selectedRoute: "shortest",
            loopedRoute: false,
            lengthLimiter: 100,
            activeSensors: [
                "pm1",
                "pm25",
                "pm10",
                "pm4",
                "HcHo",
                "unknownAirQuality"
            ]
        };
        SessionCache.set("routeFilters", filters);

        let markers = L.markerClusterGroup();
        const goodQualityMarkers = [];
        const allMarkersLatLng = [];

        markers = prepareMarkers(sensorData,
            markers,
            goodQualityMarkers,
            allMarkersLatLng,
            textValues,
            iconNames,
            icons);

        map.addLayer(markers);
        updateProgress(); //Add markers- update step #5
        //console.log("Step#5 - updated progress.");
        function getCurrentStartLatLng() {
            const cached = SessionCache.get("startLatLng");
            return cached ? L.latLng(cached.lat, cached.lng) : null;
        }

        SessionCache.remove("routeLine");
        SessionCache.remove("startLatLng");
        SessionCache.remove("destinationMarker");

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    const userLatLng = L.latLng(
                        position.coords.latitude,
                        position.coords.longitude
                    );
                    SessionCache.set("startLatLng", userLatLng);

                    if (map._startMarker) map.removeLayer(map._startMarker);
                    map._startMarker = L.marker(userLatLng)
                        .addTo(map)
                        .bindPopup("Twoja aktualna lokalizacja")
                        .openPopup();

                    map.setView(userLatLng, 10);

                    setupRouting(map, () => userLatLng, goodQualityMarkers, allMarkersLatLng);
                },
                (error) => {
                    console.error("Błąd uzyskiwania lokalizacji:", error);
                }
            );
        } else {
            alert("Geolokalizacja niedostępna.");
        }

        updateProgress(); //Set user location - update step #6
        //console.log("Step#6 - updated progress.");

        SessionCache.remove("routeLine");

        restoreRouteLine(map);
        restoreDestinationMarker(map);

        map.on("click",
            function(e) {
                if (e.originalEvent.ctrlKey) {
                    removeRouteLine(map);
                    removeDestinationMarker(map);

                    if (map._startMarker) map.removeLayer(map._startMarker);
                    SessionCache.set("startLatLng", e.latlng);

                    map._startMarker = L.marker(e.latlng)
                        .addTo(map)
                        .bindPopup("Punkt startowy (Ctrl+klik)")
                        .openPopup();

                    setupRouting(map, getCurrentStartLatLng, goodQualityMarkers, allMarkersLatLng);
                }
            });

        const locateControl = createLocateControl({
            currentStartLatLng,
            goodQualityMarkers,
        });

        locateControl.addTo(map);
        updateProgress(); //locateControl loaded - update step #7
        //console.log("Step#7 - updated progress.");
        topRightControl.addTo(map);
        updateProgress(); //topRightControl [menu] loaded - update step #8
        //console.log("Step#8 - updated progress.");
        setupRouting(map, getCurrentStartLatLng, goodQualityMarkers, allMarkersLatLng);

        window._leafletMapInstance = map;
        window._sensorData = sensorData;
        window.aStar = aStar;
        window.haversineDistance = haversineDistance;

        window.onSensorFilterChanged = function() {
            const filters = SessionCache.get("routeFilters") || {};
            const activeSensors = filters.activeSensors;

            map.removeLayer(markers);
            markers.clearLayers();
            goodQualityMarkers.length = 0;
            let visibleCount = 0;

            markers = prepareMarkers(sensorData,
                markers,
                goodQualityMarkers,
                allMarkersLatLng,
                textValues,
                iconNames,
                icons,
                activeSensors);
            visibleCount = markers._needsClustering.length;

            map.addLayer(markers);
            console.log(
                "Widoczne sensory PM:",
                activeSensors,
                "Liczba markerów:",
                visibleCount
            );
            if (typeof window.onRouteFiltersChanged === "function") {
                window.onRouteFiltersChanged();
            }
        };


        
    });