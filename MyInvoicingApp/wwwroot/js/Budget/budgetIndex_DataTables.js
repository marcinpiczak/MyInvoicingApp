$(document).ready(function() {
    var budgetListTable = $("table#budget-list-table");

    console.log(budgetListTable);

    var dataTable = budgetListTable.DataTable({
        "language": {
            "processing": "Przetwarzanie...",
            "search": "Szukaj:",
            "lengthMenu": "Pokaż _MENU_ pozycji",
            "info": "Pozycje od _START_ do _END_ z _TOTAL_ łącznie",
            "infoEmpty": "Pozycji 0 z 0 dostępnych",
            "infoFiltered": "(filtrowanie spośród _MAX_ dostępnych pozycji)",
            "infoPostFix": "",
            "loadingRecords": "Wczytywanie...",
            "zeroRecords": "Nie znaleziono pasujących pozycji",
            "emptyTable": "Brak danych",
            "paginate": {
                "first": "Pierwsza",
                "previous": "Poprzednia",
                "next": "Następna",
                "last": "Ostatnia"
            },
            "aria": {
                "sortAscending": ": aktywuj, by posortować kolumnę rosnąco",
                "sortDescending": ": aktywuj, by posortować kolumnę malejąco"
            }
        },
        "columnDefs": [
            { "orderable": false, "targets": 9 }
        ]
    });

    new $.fn.dataTable.Buttons(dataTable, {
        buttons: [
            {
                extend: "excel",
                text: "<i class='far fa-file-excel fa-lg'></i>",
                className: "btn btn-default",
                titleAttr: "eksport do Excel"
            },
            {
                extend: "pdf",
                text: "<i class='far fa-file-pdf fa-lg'></i>",
                className: "btn btn-default",
                titleAttr: "eksport do PDF"
            },
            'colvis'
        ]
    });

    var test = $("#budget-list-table_length");
    console.log(test);

    dataTable.buttons().container().appendTo($(test, dataTable.table().container()));
});