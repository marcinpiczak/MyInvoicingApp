$(document).ready(function () {
    //tabs
    var invoiceTabs = $("ul#InvoiceTabs");
    var invoiceHdrTab = invoiceTabs.find("li:nth-child(1) a");
    var invoiceLineTab = invoiceTabs.find("li:nth-child(2) a");
    var invoiceAttachments = invoiceTabs.find("li:nth-child(3) a");

    var invoiceTabContent = $("div#InvoiceTabsContent");

    //invoice header
    var invoiceHdr = invoiceTabContent.find("div#invoice-header");
    var invoiceHdrAddForm = invoiceHdr.find("form#add-invoice-header-form");
    var invoiceHdrAddBtn = invoiceHdr.find("input#add-invoice-header-btn");

    //invoice header fields
    var invoiceId = invoiceHdrAddForm.find("input#Id");
    var invoiceNumber = invoiceHdrAddForm.find("input#InvoiceNumber");
    var referenceNumber = invoiceHdrAddForm.find("input#ReferenceNumber");
    var paymentMethod = invoiceHdrAddForm.find("input#PaymentMethod");
    var paymentDueDate = invoiceHdrAddForm.find("input#PaymentDueDate");
    var issueDate = invoiceHdrAddForm.find("input#IssueDate");
    var receiveDate = invoiceHdrAddForm.find("input#ReceiveDate");
    var customerId = invoiceHdrAddForm.find("select#CustomerId");
    var defaultBudgetId = invoiceHdrAddForm.find("select#BudgetId");
    var defaultCurrency = invoiceHdrAddForm.find("input#Currency");

    //invoice line
    var invoiceLine = invoiceTabContent.find("div#invoice-lines");
    var invoiceLineAddForm = invoiceLine.find("form#add-invoice-line-form");
    var invoiceLineAddBtn = invoiceLine.find("input#add-invoice-line-btn");
    var invoiceLineCancelBtn = invoiceLine.find("a#cancel-add-invoice-line-btn");

    //invoice line fields
    var invoiceLineId = invoiceLineAddForm.find("input#Id");
    var invoiceLineInvoiceId = invoiceLineAddForm.find("input#InvoiceLine_InvoiceId");
    var invoiceLineLineNumber = invoiceLineAddForm.find("input#InvoiceLine_LineNumber");
    var invoiceLineItemName = invoiceLineAddForm.find("input#InvoiceLine_ItemName");
    var invoiceLineDescription = invoiceLineAddForm.find("textarea#InvoiceLine_Description");
    var invoiceLineQuantity = invoiceLineAddForm.find("input#InvoiceLine_Quantity");
    var invoiceLinePrice = invoiceLineAddForm.find("input#InvoiceLine_Price");
    var invoiceLineCurrency = invoiceLineAddForm.find("input#InvoiceLine_Currency");
    var invoiceLineCurrencyRate = invoiceLineAddForm.find("input#InvoiceLine_CurrencyRate");
    var invoiceLineTaxRate = invoiceLineAddForm.find("input#InvoiceLine_TaxRate");
    var invoiceLineNetto = invoiceLineAddForm.find("input#InvoiceLine_Netto");
    var invoiceLineTax = invoiceLineAddForm.find("input#InvoiceLine_Tax");
    var invoiceLineGross = invoiceLineAddForm.find("input#InvoiceLine_Gross");
    var invoiceLineBaseNetto = invoiceLineAddForm.find("input#InvoiceLine_BaseNetto");
    var invoiceLineBaseTax = invoiceLineAddForm.find("input#InvoiceLine_BaseTax");
    var invoiceLineBaseGross = invoiceLineAddForm.find("input#InvoiceLine_BaseGross");
    var invoiceLineBudgetId = invoiceLineAddForm.find("select#InvoiceLine_BudgetId");
    var invoiceLineBudgetNumber = invoiceLineBudgetId;

    //invoice line list
    var invoiceLineListTemplateRow = invoiceLine.find("div#invoice-lines-div-list tbody tr.template-invoice-line");
    //console.log(invoiceLineListTemplateRow);

    var invoiceLineListTBody = invoiceLine.find("div#invoice-lines-div-list tbody");

    //invoice attachments form
    var invoiceAttachmentForm = invoiceTabContent.find("form#add-invoice-attachment-form");
    var fileDescription = invoiceAttachmentForm.find("#FileDescription");
    var documentType = invoiceAttachmentForm.find("#DocumentType");
    var documentId = invoiceAttachmentForm.find("#DocumentId");
    var invoiceAttachmentAddBtn = invoiceAttachmentForm.find("input#add-invoice-attachment-btn");

    //invoice attachments lines
    var invoiceAttachmentsListTBody = invoiceTabContent.find("table#invoice-attachments-list tbody");
    var attachmentTemplateRow = invoiceAttachmentsListTBody.find("tr.template-invoice-attachment-row");

    //set default values for attachment form
    documentType.val(2);
    documentId.val(invoiceId.val());


    //set default locale for numeral
    numeral.locale("pl");

    //set default position for alertifyJS
    alertify.set('notifier', 'position', 'top-right');

    function activateDisabledTab(tab) {
        tab.removeClass("disabled");
    }

    function showTab(tab) {
        tab.removeClass("disabled");
        tab.tab("show");
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

    function disableFormFields(form) {
        //form.warp('<fieldset disabled=""><fieldset>');
        var formInputs = $(form).find("input, select");
        formInputs.attr("disabled", "disabled");
    }

    function prepareNewRowForInvoiceLineFromTemplate(invoiceLine) {
        var newRow = invoiceLineListTemplateRow.clone();
        newRow.removeClass(["template", "template-invoice-line"]);

        //newRow.data("id", invoiceLine.Id);
        newRow.attr("data-Id", invoiceLine.Id);
        newRow.children().eq(0).text(invoiceLine.Status);                                       //Status
        newRow.children().eq(1).text(invoiceLine.LineNumber);                                   //LineNumber
        newRow.children().eq(2).text(invoiceLine.ItemName);                                     //ItemName
        newRow.children().eq(3).text(invoiceLine.Description);                                  //Description
        newRow.children().eq(4).text(invoiceLine.Quantity);                                     //Quantity
        newRow.children().eq(5).text(numeral(invoiceLine.Price).format("0,0.00"));            //Price
        newRow.children().eq(6).text(invoiceLine.Currency);                                     //Currency
        newRow.children().eq(7).text(numeral(invoiceLine.CurrencyRate).format("0.000000"));     //CurrencyRate
        newRow.children().eq(8).text(numeral(invoiceLine.TaxRate).format("0.00"));              //TaxRate
        newRow.children().eq(9).text(numeral(invoiceLine.Netto).format("0,0.00"));            //Netto
        newRow.children().eq(10).text(numeral(invoiceLine.Tax).format("0,0.00"));             //Tax
        newRow.children().eq(11).text(numeral(invoiceLine.Gross).format("0,0.00"));           //Gross
        newRow.children().eq(12).text(numeral(invoiceLine.BaseNetto).format("0,0.00"));       //BaseNetto
        newRow.children().eq(13).text(numeral(invoiceLine.BaseTax).format("0,0.00"));         //BaseTax
        newRow.children().eq(14).text(numeral(invoiceLine.BaseGross).format("0,0.00"));       //BaseGross
        newRow.children().eq(15).text(invoiceLine.BudgetNumber);                                //BudgetNumber
        newRow.children().eq(15).attr("data-Id", invoiceLine.BudgetId);

        var actions = newRow.children().eq(16).find("a");
        actions.each(function (index, element) {
            var href = $(element).attr("href");
            $(element).attr("href", href + "?Id=" + invoiceLine.Id);
        });

        return newRow;
    }

    function addInvoiceLineToList(invoiceLine) {
        var newRow = prepareNewRowForInvoiceLineFromTemplate(invoiceLine);

        newRow.appendTo(invoiceLineListTemplateRow.parent());
    }

    function replaceModifiedLineInList(invoiceLine) {
        var newRow = prepareNewRowForInvoiceLineFromTemplate(invoiceLine);

        var trToRemove = invoiceLineListTBody.find("tr[data-id='" + invoiceLine.Id + "']").first();
        newRow.insertAfter(trToRemove);
        trToRemove.remove();
    }

    function clearInvoiceLineForm() {
        invoiceLineId.val("");
        invoiceLineItemName.val("");
        invoiceLineDescription.val("");
        invoiceLineQuantity.val(1);
        invoiceLinePrice.val(numeral("0,00").format());
        invoiceLineTaxRate.val(numeral("23,00").format());
        invoiceLineNetto.val(numeral("0,00").format());
        invoiceLineTax.val(numeral("0,00").format());
        invoiceLineGross.val(numeral("0,00").format());
        invoiceLineBaseNetto.val(numeral("0,00").format());
        invoiceLineBaseTax.val(numeral("0,00").format());
        invoiceLineBaseGross.val(numeral("0,00").format());
    }

    //returns invoice header model instance
    function getInvoiceHeaderFromForm() {
        var invoice = {
            Id: invoiceId.val(),
            InvoiceNumber: invoiceNumber.val(),
            ReferenceNumber: referenceNumber.val(),
            PaymentMethod: paymentMethod.val(),
            PaymentDueDate: paymentDueDate.val(),
            IssueDate: issueDate.val(),
            ReceiveDate: receiveDate.val(),
            CustomerId: customerId.val(),
            BudgetId: defaultBudgetId.val(),
            Currency: defaultCurrency.val()
        };

        return invoice;
    }

    //returns invoice line model instance
    function getInvoiceLineFromForm() {
        var newInvoiceLine = {
            Id: invoiceLineId.val(),
            Status: "Opened",
            InvoiceId: invoiceLineInvoiceId.val(),
            LineNumber: invoiceLineLineNumber.val(),
            ItemName: invoiceLineItemName.val(),
            Description: invoiceLineDescription.val(),
            Quantity: invoiceLineQuantity.val(),
            Price: invoiceLinePrice.val(),
            Currency: invoiceLineCurrency.val(),
            CurrencyRate: invoiceLineCurrencyRate.val(),
            TaxRate: invoiceLineTaxRate.val(),
            Netto: invoiceLineNetto.val(),
            Tax: invoiceLineTax.val(),
            Gross: invoiceLineGross.val(),
            BaseNetto: invoiceLineBaseNetto.val(),
            BaseTax: invoiceLineBaseTax.val(),
            BaseGross: invoiceLineBaseGross.val(),
            BudgetId: invoiceLineBudgetId.val(),
            BudgetNumber: invoiceLineBudgetNumber.find("option:selected").text()
        };

        return newInvoiceLine;
    }

    function setInvoiceLineAddBtnDefaultAction() {
        invoiceLineAddBtn.val("Dodaj linię");
        invoiceLineAddBtn.off();
        setInvoiceLineCancelBtnDefaultAction();
        invoiceLineAddBtn.on("click", addInvoiceLineCallback);
    }

    function setInvoiceLineAddBtnEditAction() {
        invoiceLineAddBtn.val("Zapisz");
        invoiceLineAddBtn.off();
        setInvoiceLineCancelBtnCancelEditAction();
        invoiceLineAddBtn.on("click", editInvoiceLineCallback);
    }

    function setInvoiceLineCancelBtnDefaultAction() {
        invoiceLineCancelBtn.off();
    }

    function setInvoiceLineCancelBtnCancelEditAction() {
        invoiceLineCancelBtn.on("click", function (event) {
            event.preventDefault();
            event.stopImmediatePropagation();

            clearInvoiceLineForm();
            setInvoiceLineAddBtnDefaultAction();
        });
    }

    function addInvoiceHaderCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        invoiceHdrAddForm.validate();

        if (invoiceHdrAddForm.valid()) {

            var newInvoice = getInvoiceHeaderFromForm();

            //console.log(newInvoice);

            $.ajax(
                {
                    url: "https://localhost:5001/Invoice/AddJson",
                    data: newInvoice,
                    type: "POST",
                    dataType: "json",
                    success: function (result) {
                        if (result.success) {
                            //alert('success');
                            //console.log(result);

                            invoiceId.val(result.Invoice.Id);
                            invoiceNumber.val(result.Invoice.InvoiceNumber);

                            removeValidationSummary();
                            disableFormFields(invoiceHdrAddForm);
                            activateDisabledTab(invoiceLineTab);
                            showTab(invoiceLineTab);
                            activateDisabledTab(invoiceAttachments);

                            invoiceLineInvoiceId.val(result.Invoice.Id);

                            //set default values for line from header: currency and budget
                            invoiceLineCurrency.val(defaultCurrency.val());
                            invoiceLineBudgetId.val(defaultBudgetId.val()).change();

                            //set documentId for attachment form
                            documentId.val(result.Invoice.Id);

                            alertify.notify('Dodano nową fakturę z numerem <b>' + result.Invoice.InvoiceNumber + '</b>', 'success', 10);
                        } else {
                            //console.log("error");
                            var div = createValidationSummary(result.errors);
                            //invoiceHdrAddForm.prepend(div);
                            alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        //alert(xhr.status);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent("Wystąpił błąd: " + errorThrown).show();
                    }
                });
        } else {
            return false;
        }
    }

    function editInvoiceHaderCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        invoiceHdrAddForm.validate();

        if (invoiceHdrAddForm.valid()) {

            var invoice = getInvoiceHeaderFromForm();

            console.log(invoice);

            $.ajax(
                {
                    url: "https://localhost:5001/Invoice/EditJson",
                    data: invoice,
                    type: "POST",
                    dataType: "json",
                    success: function (result) {
                        if (result.success) {
                            removeValidationSummary();
                            activateDisabledTab(invoiceLineTab);
                            showTab(invoiceLineTab);

                            //set default values for line from header: currency and budget
                            invoiceLineCurrency.val(defaultCurrency.val());
                            invoiceLineBudgetId.val(defaultBudgetId.val()).change();

                            alertify.notify('Zapisano zmiany dla faktury <b>' + result.Invoice.InvoiceNumber + '</b>', 'success', 10);
                        } else {
                            //console.log("error");
                            var div = createValidationSummary(result.errors);
                            //invoiceHdrAddForm.prepend(div);
                            alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        //alert(xhr.status);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent("Wystąpił błąd: " + errorThrown).show();
                    }
                });
        } else {
            return false;
        }
    }

    function addInvoiceLineCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        invoiceHdrAddForm.validate();

        if (invoiceLineAddForm.valid()) {

            var newInvoiceLine = getInvoiceLineFromForm();

            //console.log(newInvoiceLine);
            //console.log(invoiceLineBudgetId);
            //console.log(invoiceLineBudgetNumber);

            //testy
            //addInvoiceLineToList(newInvoiceLine);
            //clearInvoiceLineForm();

            $.ajax(
                {
                    url: "https://localhost:5001/Invoice/AddLineJson",
                    data: newInvoiceLine,
                    type: "POST",
                    dataType: "json",
                    success: function (result) {
                        if (result.success) {
                            //alert('success');
                            removeValidationSummary();

                            invoiceLineId.val(result.InvoiceLine.Id);
                            newInvoiceLine.Id = result.InvoiceLine.Id;
                            newInvoiceLine.Status = result.InvoiceLine.Status;
                            newInvoiceLine.LineNumber = result.InvoiceLine.LineNumber;

                            addInvoiceLineToList(newInvoiceLine);
                            clearInvoiceLineForm();

                            alertify.notify('Dodano nową linię faktury <b>' + result.InvoiceLine.InvoiceNumber + '</b>', 'success', 10);
                        } else {
                            //console.log("error");
                            var div = createValidationSummary(result.errors);
                            //invoiceLineAddForm.prepend(div);
                            alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        //alert(xhr.status);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent("Wystąpił błąd: " + errorThrown).show();
                    }
                });

        } else {
            return false;
        }
    }

    function editInvoiceLineCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        invoiceHdrAddForm.validate();

        if (invoiceLineAddForm.valid()) {

            var invoiceLine = getInvoiceLineFromForm();

            $.ajax(
                {
                    url: "https://localhost:5001/Invoice/EditLineJson",
                    data: invoiceLine,
                    type: "POST",
                    dataType: "json",
                    success: function (result) {
                        if (result.success) {
                            //alert('success');
                            removeValidationSummary();

                            replaceModifiedLineInList(invoiceLine);
                            clearInvoiceLineForm();
                            setInvoiceLineAddBtnDefaultAction();

                            alertify.notify('Zapisano zmiany dla linii faktury <b>' + result.InvoiceLine.InvoiceNumber + '</b>', 'success', 10);
                        } else {
                            //console.log("error");
                            var div = createValidationSummary(result.errors);
                            //invoiceLineAddForm.prepend(div);
                            alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        //alert(xhr.status);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent("Wystąpił błąd: " + errorThrown).show();
                    }
                });

        } else {
            return false;
        }
    }

    //
    if (invoiceLineId.val().trim() === "") {
        invoiceHdrAddBtn.on("click", addInvoiceHaderCallback);
    } else {
        invoiceHdrAddBtn.val("Zapisz");
        invoiceHdrAddBtn.on("click", editInvoiceHaderCallback);
    }

    //add invoice header
    invoiceHdrAddBtn.on("click", addInvoiceHaderCallback);

    //add invoice line
    invoiceLineAddBtn.on("click", addInvoiceLineCallback);

    //update number values
    invoiceLineAddForm.on("change",
        "input#InvoiceLine_Quantity, input#InvoiceLine_CurrencyRate, input#InvoiceLine_TaxRate, input#InvoiceLine_Price",
        function (event) {
            numeral.defaultFormat("0.00");

            var taxRate = numeral(invoiceLineTaxRate.val()).value();
            var currRate = numeral(invoiceLineCurrencyRate.val()).format("0.000000");
            currRate = numeral(currRate).value();
            var price = numeral(invoiceLinePrice.val()).format();
            price = numeral(price).value();
            var qty = numeral(invoiceLineQuantity.val()).value();

            var netto = price * qty;
            var tax = netto * (taxRate / 100);
            var gross = netto + tax;

            var baseNetto = netto * currRate;
            var baseTax = baseNetto * (taxRate / 100);
            var baseGross = baseNetto + baseTax;

            invoiceLineCurrencyRate.val(numeral(currRate).format("0.000000"));
            invoiceLinePrice.val(numeral(price).format());
            invoiceLineNetto.val(numeral(Math.round(netto * 100) / 100).format());
            invoiceLineTax.val(numeral(Math.round(tax * 100) / 100).format());
            invoiceLineGross.val(numeral(Math.round(gross * 100) / 100).format());
            invoiceLineBaseNetto.val(numeral(Math.round(baseNetto * 100) / 100).format());
            invoiceLineBaseTax.val(numeral(Math.round(baseTax * 100) / 100).format());
            invoiceLineBaseGross.val(numeral(Math.round(baseGross * 100) / 100).format());
        });

    //edit line
    invoiceLineListTBody.on("click", "a.edit-invoice-line", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        var row = $(this).closest("tr");

        var lineStatus = row.children().eq(0);

        if (lineStatus.text() === "Cancelled") {
            alert("Nie można edytować anulowanej linii");
        } else {
            numeral.defaultFormat("0.00");

            invoiceLineId.val(row.attr("data-Id"));
            invoiceLineLineNumber.val(row.children().eq(1).text().trim());
            invoiceLineItemName.val(row.children().eq(2).text().trim());
            invoiceLineDescription.val(row.children().eq(3).text().trim());
            invoiceLineQuantity.val(numeral(row.children().eq(4).text().trim()).value());
            invoiceLinePrice.val(numeral(row.children().eq(5).text().trim()).format());
            invoiceLineCurrency.val(row.children().eq(6).text().trim());
            invoiceLineCurrencyRate.val(numeral(row.children().eq(7).text().trim()).format("0.000000"));
            invoiceLineTaxRate.val(numeral(row.children().eq(8).text().trim()).format());
            invoiceLineNetto.val(numeral(row.children().eq(9).text().trim()).format());
            invoiceLineTax.val(numeral(row.children().eq(10).text().trim()).format());
            invoiceLineGross.val(numeral(row.children().eq(11).text().trim()).format());
            invoiceLineBaseNetto.val(numeral(row.children().eq(12).text().trim()).format());
            invoiceLineBaseTax.val(numeral(row.children().eq(13).text().trim()).format());
            invoiceLineBaseGross.val(numeral(row.children().eq(14).text().trim()).format());
            invoiceLineBudgetId.val(row.children().eq(15).attr("data-Id")).change();

            $("html, body").animate({ scrollTop: 0 }, "slow");
            setInvoiceLineAddBtnEditAction();
        }
    });

    invoiceLineListTBody.on("click", "a.cancel-invoice-line", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        var row = $(this).closest("tr");
        var lineStatus = row.children().eq(0);

        if (lineStatus.text() !== "Cancelled") {

            var line = {
                lineId: row.attr("data-Id"),
                invoiceId: invoiceLineInvoiceId.val()
            };

            $.ajax(
                {
                    url: "https://localhost:5001/Invoice/CancelLineJson",
                    data: line,
                    type: "POST",
                    dataType: "json",
                    success: function (result) {
                        if (result.success) {
                            lineStatus.text("Cancelled");

                            alertify.notify('Anulowano linię faktury', 'success', 10);
                        } else {
                            alert(result.errors);
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        //alert(xhr.status);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent("Wystąpił błąd: " + errorThrown).show();
                    }
                });
        } else {
            alert("Linia została już anulowana");
        }
    });

    function submitInvoiceAttachmentFormCallback(event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        $(this).validate();

        if ($(this).valid()) {
            $.ajax(
                {
                    url: "https://localhost:5001/Attachment/AddJson",
                    data: new FormData(this),
                    type: "POST",
                    dataType: "json",
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.success) {

                            var attachment = {
                                Id: result.Id,
                                FileName: result.FileName,
                                FileDescription: result.FileDescription,
                                DocumentType: result.DocumentType,
                                DocumentId: result.DocumentId
                            };

                            addAttachmentToList(attachment);

                            //do przerobienia na this
                            invoiceAttachmentForm[0].reset();
                            
                            alertify.notify('Dodano nowy załącznik', 'success', 10);

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

    invoiceAttachmentForm.submit(submitInvoiceAttachmentFormCallback);

    function prepareNewRowForAttachmentFromTemplate(attachment) {
        var newRow = attachmentTemplateRow.clone();
        newRow.removeClass(["template", "template-invoice-attachment-row"]);

        newRow.attr("data-id", attachment.Id);
        newRow.children().eq(0).text(attachment.FileName);
        newRow.children().eq(1).text(attachment.FileDescription);

        var actions = newRow.children().eq(2).find("a");
        actions.each(function(index, element) {
            var href = $(element).attr("href");
            $(element).attr("href", href + "?attachmentId=" + attachment.Id + "&documentType=" + attachment.DocumentType + "&documentId=" + attachment.DocumentId);
        });

        return newRow;
    }

    function addAttachmentToList(attachment) {
        var newRow = prepareNewRowForAttachmentFromTemplate(attachment);

        newRow.appendTo(attachmentTemplateRow.parent());
    }

    invoiceAttachmentsListTBody.on("click", "a.delete-attachment", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        var attachmentRow = $(this).closest("tr");
        var attachmentId = attachmentRow.attr("data-id");
        var documentType = "Invoice";
        var documentId = invoiceId.val();

        var attachmentData = {
            attachmentId: attachmentId,
            documentType: documentType,
            documentId: documentId
        };

        console.log(attachmentData);

        $.ajax(
            {
                url: "https://localhost:5001/Attachment/DeleteJson",
                data: attachmentData,
                type: "POST",
                dataType: "json",
                success: function (result) {
                    if (result.success) {

                        attachmentRow.remove();
                        alertify.notify('Usunięto załącznik', 'success', 10);

                    } else {
                        var div = createValidationSummary(result.errors);
                        //invoiceLineAddForm.prepend(div);
                        alertify.alert("Przepraszamy, wystąpił problem:", "").setContent(div[0]).show();
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    //alert(xhr.status);
                    alertify.alert("Przepraszamy, wystąpił problem:", "").setContent("Wystąpił błąd: " + errorThrown).show();
                }
            });
    });
});