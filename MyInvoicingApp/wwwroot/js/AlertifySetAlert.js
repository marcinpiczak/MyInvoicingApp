$(document).ready(function() {
    var errorMessageDiv = $("div#app-error-message");

    function displayErrorMessage() {
        var errorMessage = errorMessageDiv.find("strong");

        if (errorMessage.length !== 0) {
            errorMessageDiv.remove();
            errorMessage.addClass("alert-danger");
            alertify.alert().setContent(errorMessage[0]).show();
        }
    }

    displayErrorMessage();
});