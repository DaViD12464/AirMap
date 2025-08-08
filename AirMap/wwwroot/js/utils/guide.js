const guideBox = document.querySelector(".guide"),
    guidebuttons = document.querySelectorAll(".guidebutton");

const addGuideCookie = () => {
    if (document.cookie.includes("AirMap_guide")) return;
    guideBox.classList.add("show");

    guidebuttons.forEach(button => {
        button.addEventListener("click", () => {
            guideBox.classList.remove("show");

            if (button.id == "acceptButton") {
                console.log("Guide accepted");
                //set cookie for 1year [60= 1min, 60 = 1hour, 24 = 1 day, exdays = value to be defined for how many days cookie should last]
                const d = new Date();
                const exdays = 365;
                d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
                const expires = `expires=${d.toUTCString()}`;
                document.cookie = "guideAccepted = AirMap_guide; " + expires + ";path=/";

                // Redirect to FAQ page with hash or query
                window.location.href = "/HOME/FAQ#guide";
            }
            else { //add a cookie when cookies are rejected to stop guide box from showing again for 1 year
                console.log("Guide known & rejected");
                const d = new Date();
                const exdays = 365;
                d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
                const expires = `expires=${d.toUTCString()}`;
                document.cookie = "guideRejected = AirMap_guide; " + expires + ";path=/";
            }
        });
    });
};

window.addEventListener("load", addGuideCookie);