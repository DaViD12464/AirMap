
/*< !--Map Initialization Script-- >*/
document.addEventListener("DOMContentLoaded", async function () {
    var map = L.map('map').setView([52.1143385, 19.4236714], 7);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap'
    }).addTo(map);

    const sensorData = JSON.parse(document.getElementById('sensorData').textContent);

    const icons = {
        veryGoodAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/VeryGoodAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        goodAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/GoodAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        moderateAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/ModerateAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        sufficientAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/SufficientAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        badAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/BadAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        veryBadAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/VeryBadAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        unknownAirQualityIcon: L.icon({ iconUrl: '/AirQualityMarkers/UnknownAQ.png', iconSize: [64, 64], iconAnchor: [32, 64], popupAnchor: [6, -48] }),
        defaultGreen: L.icon({ iconUrl: 'https://maps.gstatic.com/mapfiles/ms2/micons/green-dot.png', iconSize: [32, 32], iconAnchor: [16, 32], popupAnchor: [6, -32] }),
        defaultBlue: L.icon({ iconUrl: 'https://maps.gstatic.com/mapfiles/ms2/micons/blue-dot.png', iconSize: [32, 32], iconAnchor: [16, 32], popupAnchor: [6, -32] }),
    };
    //Icon Data fetching & PopUp Data fetching
    const [iconNames, textValues] = await Promise.all([
        fetch('/api/sensor/GetIconDataBatch', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(sensorData)
        }).then(r => r.json()),

        fetch('/api/sensor/GetPopUpDataBatch', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(sensorData)
        }).then(r => r.json())
        
    ]);

    const markers = L.markerClusterGroup();

    sensorData.forEach((sensor, index) => {
        let latLng = null;
        let popupText = "SensorId: " + textValues[index].sensorId + "<br/>" + ((textValues[index].textResults) || "Sensor");
        const iconName = iconNames[index].icon;
        let icon = icons[iconName.toString()] || icons["defaultGreen"];

        if (sensor.latitude != null && sensor.longitude != null) {
            latLng = L.latLng(sensor.latitude, sensor.longitude);
        } else if (sensor.location?.latitude != null && sensor.location?.longitude != null) {
            latLng = L.latLng(sensor.location.latitude, sensor.location.longitude);
            icon = icons[iconName.toString()] || icons["defaultBlue"];
        }

        if (latLng) {
            const marker = L.marker(latLng, { icon }).bindPopup(popupText);
            markers.addLayer(marker);
        }
    });
    delete sensorData;

    map.addLayer(markers);

    let userLatLng = null;
    let routingControl = null;
    let destinationMarker = null;

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            userLatLng = L.latLng(position.coords.latitude, position.coords.longitude);

            L.marker(userLatLng).addTo(map)
                .bindPopup("Twoja aktualna lokalizacja");

            map.setView(userLatLng, 10);
        }, function (error) {
            console.error("Błąd uzyskiwania lokalizacji: ", error);
        });
    }

    map.on('contextmenu', function (e) {
        if (!userLatLng) {
            alert("Lokalizacja użytkownika nie jest jeszcze dostępna.");
            return;
        }

        const destLatLng = e.latlng;

        if (destinationMarker) {
            map.removeLayer(destinationMarker);
        }

        destinationMarker = L.marker(destLatLng).addTo(map)
            .bindPopup("Nowy punkt docelowy")
            .openPopup();

        if (routingControl) {
            map.removeControl(routingControl);
        }

        routingControl = L.Routing.control({
            waypoints: [userLatLng, destLatLng],
            routeWhileDragging: false,
            show: false,
            createMarker: () => null
        }).addTo(map);
    });
});