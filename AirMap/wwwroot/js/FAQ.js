document.addEventListener("DOMContentLoaded", () => {
    const questions = document.querySelectorAll(".FAQheader");

    questions.forEach(question => {
        question.addEventListener("click", () => {
            question.classList.toggle("active");
            const answer = question.nextElementSibling;
            const icon = question.querySelector(".icon");

            if (answer.style.display == "block") {
                answer.style.display = "none";
            } else {
                answer.style.display = "block";
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
