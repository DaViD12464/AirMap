import {
    removeRouteLine,
    removeDestinationMarker,
    setupRouting,
    restoreRouteLine,
    restoreDestinationMarker,
} from "../route.js";
import SessionCache from "../../utils/sessions.js";

export default function createLocateControl({
    currentStartLatLng,
    goodQualityMarkers,
}) {
    const locateControl = L.control({ position: "topleft" });

    locateControl.onAdd = function(map) {
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
            if (map._startMarker) map.removeLayer(map._startMarker);
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition((pos) => {
                    const userLatLng = L.latLng(
                        pos.coords.latitude,
                        pos.coords.longitude
                    );
                    SessionCache.set("startLatLng", userLatLng);
                    map._startMarker = L.marker(userLatLng)
                        .addTo(map)
                        .bindPopup("Twoja lokalizacja")
                        .openPopup();
                    map.setView(userLatLng, 10);

                    restoreRouteLine(map);
                    restoreDestinationMarker(map);

                    setupRouting(map, () => userLatLng, goodQualityMarkers);
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