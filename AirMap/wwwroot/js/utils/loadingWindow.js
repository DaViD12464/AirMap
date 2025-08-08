import { addGuideCookie } from "./guide.js";

let progress = 0;
const totalSteps = 8;

export function updateProgress() {
    const progressFill = document.querySelector(".progress-fill");
    progress = Math.min(progress + 1, totalSteps);
    const percentage = (progress / totalSteps) * 100;

    //console.log(`Progress: ${progress}/${totalSteps} -> ${percentage}%`);

    if (progressFill) {
        progressFill.style.width = percentage + "%";
    }

    if (percentage >= 100) {
        const overlay = document.getElementById("loading-overlay");
        if (overlay) overlay.style.display = "none";
        const content = document.getElementById("page-content");
        if (content) content.style.display = "block";

        setTimeout(() => {
            addGuideCookie();
        }, 500); // 0.5 second delay
    }
}