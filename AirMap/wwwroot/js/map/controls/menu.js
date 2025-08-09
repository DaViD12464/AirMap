import SessionCache from "../../utils/sessions.js";

const topRightControl = L.control({ position: "topright" });
topRightControl.onAdd = function(map) {
    const container = L.DomUtil.create("div", "leaflet-bar leaflet-control");
    container.id = "container";

    const button = L.DomUtil.create("button", "leaflet-control-custom");
    button.innerHTML = "<strong>Filtry</strong>";
    button.title = "Filtry użytkownika";
    button.style.backgroundColor = "#2c3e50";
    button.style.width = "40px";
    button.style.height = "32px";
    button.style.cursor = "pointer";
    button.style.border = "none";
    button.style.borderRadius = "4px";
    button.style.color = "white";

    const dropdown = L.DomUtil.create("div", "filter-dropdown");
    dropdown.style.position = "absolute";
    dropdown.style.top = "35px";
    dropdown.style.right = "0px";
    dropdown.style.backgroundColor = "#2c3e50";
    dropdown.style.border = "1px solid #ccc";
    dropdown.style.borderRadius = "4px";
    dropdown.style.boxShadow = "0 2px 10px rgba(0,0,0,0.2)";
    dropdown.style.padding = "10px";
    dropdown.style.minWidth = "250px";
    dropdown.style.maxHeight = "400px";
    dropdown.style.overflowY = "auto";
    dropdown.style.display = "none";
    dropdown.style.zIndex = "1000";
    dropdown.style.color = "white";

    const header = L.DomUtil.create("div");
    header.innerHTML = "<strong>Filtry trasy</strong>";
    header.style.marginBottom = "10px";
    header.style.borderBottom = "1px solid #eee";
    header.style.paddingBottom = "5px";
    header.style.color = "white";
    header.style.textAlign = "center";
    header.style.fontSize = "15px";

    const routeOptions = [
        {
            id: "shortest",
            label: "Najkrótsza ścieżka ",
            type: "radio",
            name: "route",
            checked: false,
        },
        {
            id: "cleanest",
            label: "Najczystsza ścieżka ",
            type: "radio",
            name: "route",
            checked: true,
        },
        {
            id: "loopedRoute",
            label: "Zapętlenie trasy ",
            type: "checkbox",
            checked: false,
        },
        {
            id: "rangeLimiter",
            label: "Ogranicznik odległości ",
            type: "range",
            min: 10,
            max: 1000,
            defaultValue: 100,
        },
        {
            id: "lengthLimiter",
            label: "Długość trasy (Km)",
            type: "range",
            min: 0,
            max: 1000,
            defaultValue: 100,
        },
    ];

    const sensorTypes = [
        { id: "pm1", label: "PM1", type: "checkbox", checked: true },
        { id: "pm25", label: "PM2.5", type: "checkbox", checked: true },
        { id: "pm10", label: "PM10", type: "checkbox", checked: true },
        { id: "pm4", label: "PM4", type: "checkbox", checked: true },
        { id: "HcHo", label: "HcHo", type: "checkbox", checked: true },
        {
            id: "unknownAirQuality",
            label: "unknownAirQuality",
            type: "checkbox",
            checked: true,
        },
    ];

    const checkList = L.DomUtil.create("div");

    const routeSection = L.DomUtil.create("div");
    routeSection.style.marginBottom = "15px";

    const routeTitle = L.DomUtil.create("div");
    routeTitle.innerHTML = "<strong>Opcje trasy:</strong>";
    routeTitle.style.marginBottom = "8px";
    routeTitle.style.fontSize = "13px";
    routeTitle.style.color = "#FFFAFA";
    routeSection.appendChild(routeTitle);

    routeOptions.forEach((option) => {
        const optionContainer = L.DomUtil.create("div");
        optionContainer.style.marginBottom = "5px";
        optionContainer.style.display = "flex";
        optionContainer.style.alignItems = "center";

        let input;

        if (option.type === "range") {
            input = L.DomUtil.create("input");
            input.type = "range";
            input.id = option.id;
            input.min = option.min;
            input.max = option.max;
            input.value = option.defaultValue;
            input.style.width = "100%";

            const valueDisplay = L.DomUtil.create("span");
            valueDisplay.innerHTML = option.defaultValue;
            valueDisplay.style.marginLeft = "8px";
            valueDisplay.style.fontSize = "12px";
            valueDisplay.style.color = "white";

            input.oninput = function() {
                valueDisplay.innerHTML = input.value;
                console.log(
                    `Zmieniono filtr: ${option.label} na wartość: ${input.value}`
                );
            };

            optionContainer.appendChild(input);
            optionContainer.appendChild(valueDisplay);
        } else {
            input = L.DomUtil.create("input");
            input.type = option.type;
            input.id = option.id;
            input.name = option.name;
            input.checked = option.checked;
            input.style.marginRight = "8px";

            input.onchange = function() {
                if (option.type === "radio" && input.checked) {
                    console.log(`Wybrano filtr: ${option.label}`);
                } else if (option.type === "checkbox") {
                    console.log(
                        `Filtr ${option.label} ${input.checked ? "włączony" : "wyłączony"}`
                    );
                }
            };

            optionContainer.appendChild(input);
        }

        if (option.type !== "range") {
            const label = L.DomUtil.create("label");
            label.htmlFor = option.id;
            label.innerHTML = option.label;
            label.style.cursor = "pointer";
            label.style.fontSize = "13px";
            optionContainer.appendChild(label);
        } else {
            const rangeLabel = L.DomUtil.create("div");
            rangeLabel.innerHTML = option.label;
            rangeLabel.style.marginBottom = "4px";
            rangeLabel.style.fontSize = "13px";
            rangeLabel.style.color = "#ccc";
            optionContainer.insertBefore(rangeLabel, input);
        }

        routeSection.appendChild(optionContainer);
    });

    const sensorSection = L.DomUtil.create("div");
    sensorSection.style.paddingTop = "10px";
    sensorSection.style.borderTop = "1px solid #eee";

    const sensorTitle = L.DomUtil.create("div");
    sensorTitle.innerHTML = "<strong>Sensory PM:</strong>";
    sensorTitle.style.marginBottom = "8px";
    sensorTitle.style.fontSize = "13px";
    sensorTitle.style.color = "#FFFAFA";
    sensorSection.appendChild(sensorTitle);

    sensorTypes.forEach((sensor) => {
        const sensorContainer = L.DomUtil.create("div");
        sensorContainer.style.marginBottom = "5px";
        sensorContainer.style.display = "flex";
        sensorContainer.style.alignItems = "center";

        const checkbox = L.DomUtil.create("input");
        checkbox.type = sensor.type;
        checkbox.id = sensor.id;
        checkbox.checked = sensor.checked;
        checkbox.style.marginRight = "8px";

        checkbox.onchange = function() {
            console.log(
                `Sensor PM filtr: ${sensor.label} ${
                checkbox.checked ? "włączony" : "wyłączony"
                }`
            );
        };

        const label = L.DomUtil.create("label");
        label.htmlFor = sensor.id;
        label.innerHTML = sensor.label;
        label.style.cursor = "pointer";
        label.style.fontSize = "13px";

        sensorContainer.appendChild(checkbox);
        sensorContainer.appendChild(label);
        sensorSection.appendChild(sensorContainer);
    });

    checkList.appendChild(routeSection);
    checkList.appendChild(sensorSection);

    const actionButtons = L.DomUtil.create("div");
    actionButtons.style.marginTop = "10px";
    actionButtons.style.paddingTop = "10px";
    actionButtons.style.borderTop = "1px solid #eee";
    actionButtons.style.display = "flex";
    actionButtons.style.gap = "5px";

    const applyButton = L.DomUtil.create("button");
    applyButton.innerHTML = "Zastosuj";
    applyButton.style.backgroundColor = "#007cbb";
    applyButton.style.color = "white";
    applyButton.style.border = "none";
    applyButton.style.padding = "5px 10px";
    applyButton.style.borderRadius = "3px";
    applyButton.style.cursor = "pointer";
    applyButton.style.fontSize = "12px";

    const resetButton = L.DomUtil.create("button");
    resetButton.innerHTML = "Reset";
    resetButton.style.backgroundColor = "#f0f0f0";
    resetButton.style.color = "#333";
    resetButton.style.border = "1px solid #ccc";
    resetButton.style.padding = "5px 10px";
    resetButton.style.borderRadius = "3px";
    resetButton.style.cursor = "pointer";
    resetButton.style.fontSize = "12px";

    applyButton.onclick = function(e) {
        e.stopPropagation();

        const prevFilters = SessionCache.get("routeFilters") || {};
        const prevActiveSensors = prevFilters.activeSensors || [];

        const selectedRoute =
            routeOptions.find((option) => {
                    if (option.type === "radio") {
                        const radio = dropdown.querySelector(`#${option.id}`);
                        return radio && radio.checked;
                    }
                    return false;
                })?.id ||
                "cleanest";

        const loopedRoute =
            dropdown.querySelector("#loopedRoute")?.checked || false;
        const rangeLimiter = dropdown.querySelector("#rangeLimiter")?.value || 100;
        const lengthLimiter =
            dropdown.querySelector("#lengthLimiter")?.value || 100;

        const activeSensors = sensorTypes
            .filter((sensor) => dropdown.querySelector(`#${sensor.id}`)?.checked)
            .map((sensor) => sensor.id);

        SessionCache.set("routeFilters",
            {
                selectedRoute,
                loopedRoute,
                rangeLimiter: Number(rangeLimiter),
                lengthLimiter: Number(lengthLimiter),
                activeSensors,
            });

        console.log("Wybrane filtry:",
            {
                selectedRoute,
                loopedRoute,
                rangeLimiter: Number(rangeLimiter),
                lengthLimiter: Number(lengthLimiter),
                activeSensors,
            });

        const sensorsChanged =
            prevActiveSensors.length !== activeSensors.length ||
                prevActiveSensors.some((s) => !activeSensors.includes(s));
        if (sensorsChanged && typeof window.onSensorFilterChanged === "function") {
            window.onSensorFilterChanged();
        }

        dropdown.style.display = "none";
        button.style.backgroundColor = "2c3e50";
        isOpen = false;

        if (typeof window.onRouteFiltersChanged === "function") {
            window.onRouteFiltersChanged();
        }
    };

    resetButton.onclick = function(e) {
        e.stopPropagation();

        routeOptions.forEach((option) => {
            const input = dropdown.querySelector(`#${option.id}`);
            if (!input) return;
            if (option.type === "radio") {
                input.checked = option.id === "cleanest";
            } else if (option.type === "checkbox") {
                input.checked = option.checked;
            } else if (option.type === "range") {
                input.value = option.defaultValue;
                input.dispatchEvent(new Event("input"));
            }
        });

        sensorTypes.forEach((sensor) => {
            const checkbox = dropdown.querySelector(`#${sensor.id}`);
            if (checkbox) checkbox.checked = sensor.checked;
        });

        SessionCache.set("routeFilters",
            {
                selectedRoute: "cleanest",
                loopedRoute: false,
                rangeLimiter: 100,
                lengthLimiter: 100,
                activeSensors: sensorTypes.map((s) => s.id),
            });

        console.log("Filtry zostały zresetowane do wartości domyślnych");
    };

    actionButtons.appendChild(applyButton);
    actionButtons.appendChild(resetButton);

    dropdown.appendChild(header);
    dropdown.appendChild(checkList);
    dropdown.appendChild(actionButtons);

    let isOpen = false;
    button.onclick = function(e) {
        e.stopPropagation();
        isOpen = !isOpen;
        dropdown.style.display = isOpen ? "block" : "none";
        button.style.backgroundColor = isOpen ? "midnightblue" : "#2c3e50";
        button.style.color = "white";
    };

    document.addEventListener("click",
        function(e) {
            if (!container.contains(e.target)) {
                dropdown.style.display = "none";
                button.style.backgroundColor = "#2c3e50";
                isOpen = false;
            }
        });

    dropdown.onclick = function(e) {
        e.stopPropagation();
    };

    L.DomEvent.disableScrollPropagation(dropdown);
    L.DomEvent.disableClickPropagation(container);
    container.appendChild(button);
    container.appendChild(dropdown);

    return container;
};

export default topRightControl;