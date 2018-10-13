$(document).ready(function() {
    var table = $("table");

    //var tableLength = table.children().first().find("th").length;

    var addBtn = $(".my-add-btn");

    var btnText = addBtn.html();
    var btnHref = addBtn.attr("href");
    var btnClass = "btn btn-success";
    if (addBtn.length === 0) {
        btnClass = "template";
    }
    

    $.extend(true, $.fn.dataTable.defaults, {
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.19/i18n/Polish.json"
        },
        "dom": "<'row'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4 text-center'B><'col-sm-12 col-md-4'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        buttons: {
            buttons: [
                {
                    text: btnText,
                    className: btnClass,
                    action: function (e, dt, node, config) {
                        window.location.href = btnHref;
                    }
                },
                {
                    extend: "excel",
                    text: "<i class='far fa-file-excel fa-lg'></i>",
                    className: "btn btn-default",
                    titleAttr: "eksport tabeli do Excel"
                },
                {
                    extend: "pdf",
                    text: "<i class='far fa-file-pdf fa-lg'></i>",
                    className: "btn btn-default",
                    titleAttr: "eksport tabeli do PDF",
                    orientation: 'landscape'
                },
                {
                    extend: "colvis",
                    text: "widoczność kolumn"
                }
            ],
            dom: {
                button: {
                    className: 'btn'
                }
            }
        }
        //"columnDefs": [
        //    { "orderable": false, "targets": tableLength - 1 }
        //]
    });

    table.DataTable({
        "order": []
        //,"scrollX": true
    });
});