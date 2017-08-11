function resizeIFrameToFitContent() {
    var iframes = document.querySelectorAll("iframe");
    for (var i = 0; i < iframes.length; i++) {
        iframes[i].width = iframes[i].contentWindow.document.body.scrollWidth;
        iframes[i].height = iframes[i].contentWindow.document.body.scrollHeight;
    }
}
window.onload = resizeIFrameToFitContent;