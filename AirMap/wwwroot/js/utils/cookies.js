const cookieBox = document.querySelector(".cookies"),
    buttons = document.querySelectorAll(".button");

const executeCodes = () => {
    if (document.cookie.includes("AirMap_cookies")) return;
    cookieBox.classList.add("show");

    buttons.forEach(button => {
        button.addEventListener("click", () => {
            cookieBox.classList.remove("show");

            if (button.id == "acceptButton") {
                console.log("Cookies accepted");
                //set cookies for 1month [60= 1min, 60 = 1hour, 24 = 1 day, exdays = value to be defined for how many days cookie should last]
                const d = new Date();
                const exdays = 30;
                d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
                const expires = `expires=${d.toUTCString()}`;
                document.cookie = "cookiesAccepted = AirMap_cookies; " + expires + ";path=/";
            }
            else { //add a cookie when cookies are rejected to stop cookie box from showing again for 1 month
                console.log("Cookies rejected");
                const d = new Date();
                const exdays = 30;
                d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
                const expires = `expires=${d.toUTCString()}`;
                document.cookie = "cookiesRejected = AirMap_cookies; " + expires + ";path=/";
            }
        });
    });
};

window.addEventListener("load", executeCodes);


export function getCookie(cname) {
    const name = cname + "=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const ca = decodedCookie.split(";");
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == " ") {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}