// wwwroot/js/map/init.js
import { setupRouting, removeRouteLine, removeDestinationMarker } from "./route.js";
// ReSharper disable once InconsistentNaming
import SessionCache from "../utils/sessions.js";
import createLocateControl from "./controls/locateControl.js"; 
import topRightControl from "./controls/menu.js";

document.addEventListener("DOMContentLoaded",
    async function() {
        const map = L.map("map").setView([52.1143385, 19.4236714], 7);
        let startMarker = null;
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
            veryGoodAirQualityIcon: L.icon({ iconUrl: "/AirQualityMarkers/VeryGoodAQ.png", iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
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
            let popupText = "SensorId: " + textValues[index].sensorId + "<br/>" + ((textValues[index].textResults) || "Sensor");
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
                    icon.options.iconUrl.includes("VeryBadAQ.png") ||
                    icon.options.iconUrl.includes("GoodAQ.png") ||
                    icon.options.iconUrl.includes("VeryGoodAQ.png")
                    
                ) {
                    goodQualityMarkers.push(latLng);
                }
            }
        });

        map.addLayer(markers);

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    currentStartLatLng = L.latLng(
                        position.coords.latitude,
                        position.coords.longitude
                    );

                    if (startMarker) map.removeLayer(startMarker);
                    startMarker = L.marker(currentStartLatLng)
                        .addTo(map)
                        .bindPopup("Twoja aktualna lokalizacja")
                        .openPopup();

                    map.setView(currentStartLatLng, 10);
                    setupRouting(map, () => currentStartLatLng, goodQualityMarkers);
                },
                (error) => {
                    console.error("Błąd uzyskiwania lokalizacji:", error);
                }
            );
        } else {
            alert("Geolokalizacja niedostępna.");
        }

        map.on("click",
            function(e) {
                if (e.originalEvent.lon) {
                    removeRouteLine(map);
                    removeDestinationMarker(map);

                    if (startMarker) map.removeLayer(startMarker);
                    currentStartLatLng = e.latlng;

                    startMarker = L.marker(currentStartLatLng
                            //,
                            //{
                            //    icon: L.icon({
                            //        iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
                            //        iconSize: [32, 32],
                            //        iconAnchor: [16, 32],
                            //        popupAnchor: [0, -32]
                            //    })
                            //}
                        )
                        .addTo(map)
                        .bindPopup("Punkt startowy (Ctrl+klik)")
                        .openPopup();

                    setupRouting(map, () => currentStartLatLng, goodQualityMarkers);
                }
            });

        const locateControl = createLocateControl({
            startMarker,
            currentStartLatLng,
            goodQualityMarkers
        });

        locateControl.addTo(map);

        topRightControl.addTo(map);
    });