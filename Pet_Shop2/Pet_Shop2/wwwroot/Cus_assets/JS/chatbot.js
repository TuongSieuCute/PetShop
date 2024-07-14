document.addEventListener("DOMContentLoaded", function () {
    var chatbox = document.getElementById("chatbox");
    var boxbody = document.getElementById("box_body");
    var boxfooter = document.getElementById("box_footer");
    var closeButton = document.querySelector(".close_chatbox");

    var isChatboxHidden = true;

    closeButton.addEventListener("click", function (event) {
        event.preventDefault();
        if (isChatboxHidden) {
            chatbox.classList.remove("slide-down");
            boxbody.classList.remove("slide-down");
            boxfooter.classList.remove("hide");
            closeButton.innerHTML = "<i class='bx bx-x-circle'></i>";
        } else {
            chatbox.classList.add("slide-down");
            boxbody.classList.add("slide-down");
            boxfooter.classList.add("hide");
            closeButton.innerHTML = "<i class='bx bx-up-arrow-alt'></i>";
        }
        isChatboxHidden = !isChatboxHidden;
    });
});
