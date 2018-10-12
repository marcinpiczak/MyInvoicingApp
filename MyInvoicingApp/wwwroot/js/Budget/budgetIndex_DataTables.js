$(document).ready(function() {
    var budgetListTable = $("table#budget-list-table");

    var tableLength = budgetListTable.children().first().find("th").length;
    //console.log(budgetListTable.children()[0].children().length);

    //console.log(budgetListTable);

    var dataTable = budgetListTable.DataTable({
        "dom": "<'row'<'col-sm-12 col-md-5'l>B<'col-sm-12 col-md-5'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",

        buttons: [
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

        "columnDefs": [
            { "orderable": false, "targets": tableLength-1 }
        ]
    });

    //new $.fn.dataTable.Buttons(dataTable, {
    //    buttons: [
    //        {
    //            extend: "excel",
    //            text: "<i class='far fa-file-excel fa-lg'></i>",
    //            className: "btn btn-default",
    //            titleAttr: "eksport tabeli do Excel"
    //        },
    //        {
    //            extend: "pdf",
    //            text: "<i class='far fa-file-pdf fa-lg'></i>",
    //            className: "btn btn-default",
    //            titleAttr: "eksport tabeli do PDF",
    //            orientation: 'landscape'
    //        },
    //        {
    //            extend: "colvis",
    //            text: "widoczność kolumn"
    //        }
            
    //    ]
    //});

    var test = $("h2");
    //console.log(test);

    dataTable.buttons().container().appendTo($(test, dataTable.table().container()));
});