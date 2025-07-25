import { removeRouteLine, removeDestinationMarker, setupRouting } from "../route.js";

export default function createLocateControl({
    startMarker,
    currentStartLatLng,
    goodQualityMarkers
}) {
    const locateControl = L.control({ position: "topleft" });

    locateControl.onAdd = function (map) {
        const button = L.DomUtil.create(
            "button",
            "leaflet-bar leaflet-control leaflet-control-custom"
        );
        button.innerHTML = "📍";
        button.title = "Pokaż moją lokalizację";
        button.style.backgroundColor = "#2c3e50";
        button.style.width = "32px";
        button.style.height = "32px";
        button.style.cursor = "pointer";
        button.style.color = "white";

        button.onclick = () => {
            removeRouteLine(map);
            removeDestinationMarker(map);
            if (startMarker) map.removeLayer(startMarker);
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition((pos) => {
                    const userLatLng = L.latLng(
                        pos.coords.latitude,
                        pos.coords.longitude
                    );
                    L.marker(userLatLng)
                        .addTo(map)
                        .bindPopup("Twoja lokalizacja")
                        .openPopup();
                    map.setView(userLatLng, 10);
                    setupRouting(map, () => currentStartLatLng, goodQualityMarkers);
                });
            } else {
                alert("Geolokalizacja niedostępna.");
            }
        };

        L.DomEvent.disableScrollPropagation(button);
        L.DomEvent.disableClickPropagation(button);

        return button;
    };

    return locateControl;
}