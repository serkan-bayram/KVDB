// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function setStartTimes() {
    const videoElements = document.querySelectorAll("video");

    videoElements.forEach((video) => {
        const startTime = video.getAttribute("data-start-time");

        // Start from 5 second before  
        video.currentTime = startTime - 5;

    })

}

function setStartTimesForDownloading() {
    const startInputs = document.querySelectorAll(".startFrom");
    const currentTimeInputs = document.querySelectorAll("#currentTime");

    startInputs.forEach((input) => {

        const transcriptId = input.getAttribute("data-transcript-id");

        const videoElement = document.querySelector(`video[data-transcript-id="${transcriptId}"]`);

        videoElement.addEventListener("timeupdate", (e) => {
            input.value = e.target.currentTime;

            const currentTimeInput = Array.from(currentTimeInputs).find((currentTimeInput) => currentTimeInput.getAttribute("data-transcript-id") === transcriptId)

            if (currentTimeInput) {
                currentTimeInput.innerText = e.target.currentTime.toFixed(0);
            }
        })
    })
}

document.addEventListener("DOMContentLoaded", () => {
    setStartTimes();
    setStartTimesForDownloading();
});
