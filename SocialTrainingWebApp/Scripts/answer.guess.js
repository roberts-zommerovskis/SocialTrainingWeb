$(document).ready(function () {
    var chosenOption;
    $("input[name = 'optionsRadios']").on("click", function () {
        chosenOption = $(this);
        $('#optionButton').removeClass('disabled');
        $('#optionButton').addClass('acceptAns');
    });

    $("#optionButton").click(function () {
        if ($("#optionButton").hasClass("acceptAns")) {
            sendValueToController(chosenOption.attr("id"), handleAnswers);
        }
        else if ($("#optionButton").hasClass("navFurther")) {
            redirect();
        }
    });

    function handleAnswers(result) {
        chosenOption.attr("checked", true);
        $("#guessingForm input:radio").attr('disabled', true);
        $("#guessingForm").unbind("click");
        $("#guessingForm").addClass('disabled-radios');
        $('#optionButton').removeClass('acceptAns');
        $('#optionButton').addClass("navFurther");
        $('#optionButton').text('Next');
        if (result.Success) {
            chosenOption.parent().css("background-color", "#68a611");
        }
        else {
            chosenOption.parent().css("background-color", "#c72828");
            getCorrectAnswerNumber(markCorrectAnswer);
        }
    }

    function markCorrectAnswer(result) {
        $("#" + result.Answer).parent().css("background-color", "#68a611");
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