$(document).ready(function () {
    var element = $(this);
    $("input[name = 'optionsRadios']").on("click", function () {
        element = $(this);
        sendValueToController($(this).attr("id"), myCallback);
    });

    function myCallback(result) {
        if (result.Success) {
            element.parent().css("background-color", "#68a611");
            element.attr("checked", true);
            document.body.innerHTML += '<div style="background-color: rgba(1, 1, 1, 0.01);bottom: 0;left: 0;position: fixed;right: 0;top: 0;"></div>';
            redirect(element.attr("id"));
        }
        else {
            element.parent().css("background-color", "#c72828");
            getCorrectAnswerNumber(myCallback2)
        }
    }

    function myCallback2(result) {
        $("#" + result.Answer).parent().css("background-color", "#68a611");
        element.attr("checked", true);
        document.body.innerHTML += '<div style="background-color: rgba(1, 1, 1, 0.01);bottom: 0;left: 0;position: fixed;right: 0;top: 0;"></div>';
        redirect(element.attr("id"));
    }

    function sendValueToController(id, callback) {
        $.ajax({
            url: "/Home/CheckAnswer/",
            data: { ID: id },
            cache: false,
            type: "GET",
            timeout: 10000,
            dataType: "json",
            success: callback
        });
    }

    function getCorrectAnswerNumber(callback) {
        $.ajax({
            url: "/Home/ReturnCorrectAnswerNumber/",
            data: {},
            cache: false,
            type: "GET",
            timeout: 10000,
            dataType: "json",
            success: callback
        });
    }

})