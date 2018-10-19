$(document).ready(function () {
    var budgetAddForm = $("#budget-add-form");
    var addBudgetBtn = budgetAddForm.find("input[type='submit']");

    var budgetNumber = budgetAddForm.find("#BudgetNumber");
    var description = budgetAddForm.find("#Description");
    var commitedAmount = budgetAddForm.find("#CommitedAmount");

    function removeValidationSummary() {
        var validation = $(".validation-summary-errors");
        validation.remove();
    }

    function createValidationSummary(errorArray) {
        removeValidationSummary();
        var div = $('<div class="text-danger validation-summary-errors">');
        var ul = $("<ul>");
        errorArray.forEach(function (element) {
            var li = $("<li>").text(element);
            ul.append(li);
            div.append(ul);
        });

        return div;
    }

    function clearBudgetForm() {
        budgetNumber.val("");
        description.val("");
        commitedAmount.val("");
    }

    function addBudgetCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        if (budgetAddForm.valid()) {
            var newBudget = {
                BudgetNumber: budgetNumber.val(),
                Description: description.val(),
                CommitedAmount: commitedAmount.val()
            };

            $.ajax(
                {
                    url: "https://localhost:5001/Budget/AddJson",
                    data: newBudget,
                    type: "POST",

                    success: function (result) {
                        if (result.success) {

                            removeValidationSummary();
                            clearBudgetForm();

                            alertify.notify('Dodano nowy Budżet z numerem <b>' + result.Budget.BudgetNumber + '</b>', 'success', 10);

                        } else {
                            var div = createValidationSummary(result.errors);
                            alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        alert(xhr.status);
                    }
                });
        } else {
            return false;
        }
    }

    addBudgetBtn.on("click", addBudgetCallback);

});