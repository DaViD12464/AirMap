document.addEventListener("DOMContentLoaded", () => {
    const questions = document.querySelectorAll(".FAQheader");

    questions.forEach(question => {
        question.addEventListener("click", () => {
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
        });
    });
});


function toggleArrows(icon) {
    const down = icon.querySelector('.down-arrow');
    const up = icon.querySelector('.up-arrow');

    down.classList.toggle('hidden');
    down.classList.toggle('active');

    up.classList.toggle('hidden');
    up.classList.toggle('active');
}
