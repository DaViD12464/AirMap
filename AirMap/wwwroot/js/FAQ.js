document.addEventListener("DOMContentLoaded", () => {
    const questions = document.querySelectorAll(".FAQheader");

    questions.forEach(question => {
        question.addEventListener("click", () => toggleFAQ(question));
    });

    openFAQFromHash();

    window.addEventListener("hashchange", openFAQFromHash);
});

function toggleFAQ(question) {
    question.classList.toggle("active");
    const answer = question.nextElementSibling;
    const icon = question.querySelector(".icon");

    if (answer.classList.contains("hidden")) {
        answer.classList.remove("hidden");
        answer.style.transition = "";
        requestAnimationFrame(() => {
            answer.classList.add("open");
        });
    } else {
        answer.style.transition = "max-height 0.6s ease-in-out, padding 0.8s ease-in-out";
        answer.classList.remove("open");
        answer.addEventListener("transitionend", function handler() {
            answer.classList.add("hidden");
            answer.style.transition = "";
            answer.removeEventListener("transitionend", handler);
        });
    }

    toggleArrows(icon);
}

function toggleArrows(icon) {
    const down = icon.querySelector('.down-arrow');
    const up = icon.querySelector('.up-arrow');

    down.classList.toggle('hidden');
    down.classList.toggle('active');

    up.classList.toggle('hidden');
    up.classList.toggle('active');
}

function openFAQFromQuery() {
    const params = new URLSearchParams(window.location.search);
    const openList = params.get("open");
    if (!openList) return;

    const ids = openList.split(",");
    ids.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            const question = element.querySelector(".FAQheader");
            if (question) toggleFAQOpen(question, element);
        }
    });
}

function toggleFAQOpen(question, element) {
    const answer = question.nextElementSibling;
    if (answer && answer.classList.contains("hidden")) {
        answer.classList.remove("hidden");
        answer.style.transition = "";
        requestAnimationFrame(() => {
            answer.classList.add("open");
        });

        const icon = question.querySelector(".icon");
        if (icon) {
            const downArrow = icon.querySelector(".down-arrow");
            const upArrow = icon.querySelector(".up-arrow");
            downArrow?.classList.add("hidden");
            downArrow?.classList.remove("active");
            upArrow?.classList.remove("hidden");
            upArrow?.classList.add("active");
        }
    }

    // Scroll into view for that FAQ element
    element.scrollIntoView({ behavior: "smooth" });
}

// Run when page loads
document.addEventListener("DOMContentLoaded", openFAQFromQuery);


