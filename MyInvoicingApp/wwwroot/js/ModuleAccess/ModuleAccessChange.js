$(document).ready(function () {
    var moduleAccessSaveBtn = $("#save-module-access-btn");

    var moduleAccessTable = $("#module-access-table");

    var roleId = moduleAccessTable.attr("data-roleid");

    var moduleAccessRows = moduleAccessTable.find("tbody tr");

    function getModuleAccessObjectFromRow(row) {
        var id = $(row).attr("data-id");
        var module = $(row).children(".module-access-module").eq(0).attr("data-module");
        var chbxIndex = $(row).find(".module-access-index").eq(0).prop('checked');
        var chbxAdd = $(row).find(".module-access-add").eq(0).prop('checked');
        var chbxEdit = $(row).find(".module-access-edit").eq(0).prop('checked');
        var chbxClose = $(row).find(".module-access-close").eq(0).prop('checked');
        var chbxOpen = $(row).find(".module-access-open").eq(0).prop('checked');
        var chbxCancel = $(row).find(".module-access-cancel").eq(0).prop('checked');
        var chbxSend = $(row).find(".module-access-send").eq(0).prop('checked');
        var chbxDetails = $(row).find(".module-access-details").eq(0).prop('checked');
        var chbxApprove = $(row).find(".module-access-approve").eq(0).prop('checked');
        var chbxRemove = $(row).find(".module-access-remove").eq(0).prop('checked');

        var moduleAccess = {
            Id: id,
            AccessorId: roleId,
            AccessorType: 2,
            Module: module,
            Index: chbxIndex,
            Add: chbxAdd,
            Edit: chbxEdit,
            Close: chbxClose,
            Open: chbxOpen,
            Cancel: chbxCancel,
            Send: chbxSend,
            Details: chbxDetails,
            Approve: chbxApprove,
            Remove: chbxRemove
        };

        return moduleAccess;
    }

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

    function saveModuleAccessesCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        var moduleAccessObjects = [];

        moduleAccessRows.each(function (index, element) {
            var access = getModuleAccessObjectFromRow(element);

            moduleAccessObjects.push(access);
        });

        //console.log(moduleAccessObjects);

        $.ajax(
            {
                url: "https://localhost:5001/ModuleAccess/ChangeRoleAccessJson",
                data: JSON.stringify(moduleAccessObjects),
                type: "POST",
                //traditional: true,
                contentType: 'application/json',
                dataType: "json",
                success: function (result) {
                    if (result.success) {

                        alertify.notify('Zapisano zmiany dla roli <b>' + 'ROLA' + '</b>', 'success', 10);

                    } else {
                        var div = createValidationSummary(result.errors);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert(xhr.status);
                }
            });
    }

    moduleAccessSaveBtn.on("click", saveModuleAccessesCallback);
});