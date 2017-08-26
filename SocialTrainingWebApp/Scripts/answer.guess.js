$(document).ready(function () {
    var chosenOption;
    $("input[name = 'optionsRadios']").on("click", function () {
        chosenOption = $(this);
        $('#optionButton').removeClass('disabled');
        $('#optionButton').addClass('acceptAns');
    });

    $("#optionButton").click(function () {
        if ($("#optionButton").hasClass("acceptAns")) {
            sendValueToController(chosenOption.attr("id"), myCallback);
        }
        else if ($("#optionButton").hasClass("navFurther")) {
            redirect();
        }
    });

    function myCallback(result) {
        if (result.Success) {
            chosenOption.parent().css("background-color", "#68a611");
            chosenOption.attr("checked", true);
            document.body.innerHTML += '<div style="background-color: rgba(1, 1, 1, 0.01);bottom: 0;left: 0;position: fixed;right: 0;top: 0;"></div>';
            redirect(chosenOption.attr("id"));
        }
        else {
            chosenOption.parent().css("background-color", "#c72828");
            getCorrectAnswerNumber(myCallback2);
        }
    }

    function myCallback2(result) {
        $("#" + result.Answer).parent().css("background-color", "#68a611");
        chosenOption.attr("checked", true);
        $("#guessingForm input:radio").attr('disabled', true);
        $("#guessingForm").unbind("click");
        $("#guessingForm").addClass('disabled-radios');
        $('#optionButton').removeClass('acceptAns');
        $('#optionButton').addClass("navFurther");
        $('#optionButton a').text('Next');
        //document.body.innerHTML += '<div style="background-color: rgba(1, 1, 1, 0.01);bottom: 0;left: 0;position: fixed;right: 0;top: 0;"></div>';
        //redirect();
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

});