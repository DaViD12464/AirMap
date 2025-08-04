// wwwroot/js/utils/sensor.js

export function prepareMarkers(
    sensorData,
    markers,
    goodQualityMarkers,
    textValues,
    iconNames,
    icons,
    filters = null
) {
    sensorData.forEach((sensor, index) => {
        const iconName = iconNames[index].icon;
        const unknownFlag = iconName.includes("unknown");

        if (filters && filters.length < 6) {
            let show = false;

            if (filters.includes("unknownAirQuality") && unknownFlag) {
                show = true;
            }

            if (filters.includes("pm1") && sensor.pm1 !== null) {
                show = true;
            }

            if (filters.includes("pm25") && sensor.pm25 !== null) {
                show = true;
            }

            if (filters.includes("pm10") && sensor.pm10 !== null) {
                show = true;
            }

            if (filters.includes("pm4") && sensor.pm4 !== null) {
                show = true;
            }

            if (filters.includes("HcHo") && sensor.HcHo !== null) {
                show = true;
            }

            if (!show) return;
        }

        let latLng = null;
        const popupText = `SensorId: ${textValues[index].sensorId}<br/>${
            textValues[index].textResults || "Sensor"
            }`;

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

    return markers;
}