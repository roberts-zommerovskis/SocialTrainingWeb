$(window).on('resize', resize);
$(document).ready(resize);

function resize() {
    var windowWidth = $(window).width();
    var windowHeight = $(window).height();
    if ((windowWidth < 1450 && windowWidth > 630) && $(".container-fluid").width != 640) {
        $(".container-fluid").css("width", "640px");
    }
    else if (windowWidth < 630 && $(".container-fluid").width != "50%") {
        $(".container-fluid").css("width", "90%");
    }
    if (windowWidth < 775 && $(".container-fluid").width != "80%") {
        $(".container-fluid").css("height", "80%");
    }
    else {
        $(".container-fluid").css("height", "100%");
    }
}