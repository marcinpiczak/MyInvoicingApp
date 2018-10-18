$(document).ready(function() {
    var errorMessageDiv = $("div#app-error-message");
    var successMessageDiv = $("div#app-success-message");

    function displayErrorMessage() {
        var errorMessage = errorMessageDiv.find("strong");

        if (errorMessage.length !== 0) {
            errorMessageDiv.remove();
            errorMessage.addClass("alert-danger");
            alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(errorMessage[0]).show();
        }
    }

    function displaySuccessMessage() {
        var successMessage = successMessageDiv.find("strong");

        if (successMessage.length !== 0) {
            var successMessageText = successMessage.text();
            successMessageDiv.remove();
            alertify.notify(successMessageText, 'success', 10);
        }
    }

    displayErrorMessage();
    displaySuccessMessage();
});