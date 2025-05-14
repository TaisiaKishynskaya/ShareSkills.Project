// wwwroot/js/chat.js
window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

function reloadPage() {
    location.reload();
}