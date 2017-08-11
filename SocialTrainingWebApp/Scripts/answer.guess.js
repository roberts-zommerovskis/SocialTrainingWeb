function checkAnswers(correctAnswerNumber) {

    $("input[name = 'optionsRadios']").on("click", function () {
        var answer = sendValueToController($(this).attr("id"));
        if (answer === true) {
            $(this).parent().css("background-color", "#68a611");
        }
        else {
            $(this).parent().css("background-color", "#c72828");
            $("#" + correctAnswerNumber).parent().css("background-color", "#68a611");
        }
        $(this).attr("id").prop("checked");
        document.body.innerHTML += '<div style="background-color: rgba(1, 1, 1, 0.01);bottom: 0;left: 0;position: fixed;right: 0;top: 0;"></div>';


    });
}

function sendValueToController(id) {
    $.ajax({
        url: "/Home/CheckAnswer/",
        data: { ID: id },
        cache: false,
        type: "GET",
        timeout: 10000,
        dataType: "json",
        success: function (result) {
            if (result.Success) { // this sets the value from the response
                return true;
            } else {
                return false;
            }
        }
    });
}

