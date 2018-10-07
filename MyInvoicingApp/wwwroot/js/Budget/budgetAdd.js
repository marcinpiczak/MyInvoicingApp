$(document).ready(function () {
    var budgetAddForm = $("#budget-add-form");
    var addBudgetBtn = budgetAddForm.find("input[type='submit']");

    console.log(budgetAddForm);
    console.log(addBudgetBtn);

    var budgetNumber = budgetAddForm.find("#BudgetNumber");
    var description = budgetAddForm.find("#Description");
    var commitedAmount = budgetAddForm.find("#CommitedAmount");

    console.log(budgetNumber);
    console.log(description);
    console.log(commitedAmount);

    addBudgetBtn.on("click", function(event) {
        event.preventDefault();

        //budgetAddForm.validate();

        if (budgetAddForm.valid()) {
            var newBudget = {
                BudgetNumber: budgetNumber.val(),
                Description: description.val(),
                CommitedAmount: commitedAmount.val()
            };

            $.ajax(
                {
                    url: "https://localhost:5001/Budget/Add",
                    data: newBudget,
                    type: "POST",

                    success: function (data, textStatus, xhr) {
                        console.log(textStatus);
                        console.log(data);
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        console.log(xhr.status);
                    }
                });
        } else {
            return false;
        }
    });

});