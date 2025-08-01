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

document.addEventListener("DOMContentLoaded",
    async function() {
        const map = L.map("map").setView([52.1143385, 19.4236714], 7);
        let currentStartLatLng = null;

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
            {
                maxZoom: 19,
                attribution: "© OpenStreetMap",
            }).addTo(map);

        const sensorData = await fetch("/api/sensor/GetAllSensorData")
            .then((res) => res.json())
            .catch((err) => {
                console.error("Error loading sensor data:", err);
                return [];
            });

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

        const icons = {
            veryGoodAirQualityIcon: L.icon({iconUrl: "/AirQualityMarkers/VeryGoodAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            goodAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/GoodAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            moderateAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/ModerateAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            sufficientAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/SufficientAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            badAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/BadAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            veryBadAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/VeryBadAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            unknownAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/UnknownAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
            defaultGreen: L.icon({ iconUrl: "https://maps.gstatic.com/mapfiles/ms2/micons/green-dot.png", iconSize: [32, 32], iconAnchor: [16, 32], popupAnchor: [0, -32] }),
            defaultBlue: L.icon({ iconUrl: "https://maps.gstatic.com/mapfiles/ms2/micons/blue-dot.png", iconSize: [32, 32], iconAnchor: [16, 32], popupAnchor: [0, -32] })
        };

        const markers = L.markerClusterGroup();
        const goodQualityMarkers = [];

        sensorData.forEach((sensor, index) => {
            let latLng = null;
            const popupText =
                `SensorId: ${textValues[index].sensorId}<br/>${textValues[index].textResults || "Sensor"}`;
            const iconName = iconNames[index].icon;
            let icon = icons[iconName] || icons.defaultGreen;

            if (sensor.latitude != null && sensor.longitude != null) {
                latLng = L.latLng(sensor.latitude, sensor.longitude);
            } else if (
                sensor.location?.latitude != null &&
                    sensor.location?.longitude != null
            ) {
                latLng = L.latLng(sensor.location.latitude, sensor.location.longitude);
                icon = icons[iconName] || icons.defaultBlue;
            }

            if (latLng) {
                const marker = L.marker(latLng, { icon }).bindPopup(popupText);
                markers.addLayer(marker);

                if (
                    icon.options.iconUrl.includes("ModerateAQ.png") ||
                        icon.options.iconUrl.includes("GoodAQ.png") ||
                        icon.options.iconUrl.includes("VeryGoodAQ.png")
                ) {
                    goodQualityMarkers.push(latLng);
                }
            }
        });

        map.addLayer(markers);

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

                    setupRouting(map, () => userLatLng, goodQualityMarkers);
                },
                (error) => {
                    console.error("Błąd uzyskiwania lokalizacji:", error);
                }
            );
        } else {
            alert("Geolokalizacja niedostępna.");
        }

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

                    setupRouting(map, getCurrentStartLatLng, goodQualityMarkers);
                }
            });

        const locateControl = createLocateControl({
            currentStartLatLng,
            goodQualityMarkers,
        });

        locateControl.addTo(map);
        topRightControl.addTo(map);
        setupRouting(map, getCurrentStartLatLng, goodQualityMarkers);
    });