$(document).ready(function() {
    var body = $("div.container-fluid.body-content");

    //console.log(body);

    var menuFunctions = $("nav div.dropdown-menu");

    //console.log(menuFunctions);

    //function loadPartialView(event) {
    //    event.preventDefault();

    //    var href = $(this).attr("href");

    //    //console.log(this);
    //    //console.log(href);

    //    $.ajax(
    //        {
    //            url: "https://localhost:5001" + href,
    //            type: "GET",
    //            success: function (data, textStatus, xhr) {
    //                //body.html(data);
    //                //loadPartialForCancel();
    //                //console.log(data);
    //                var scripts = $("<div>").html(data).find("script");
    //                //var scripts = $(data).find("script");
    //                console.log(scripts);
    //            },
    //            error: function (xhr, textStatus, errorThrown) {
    //                console.log(xhr.status);
    //                alert("ERROR");
    //            }
    //        });
    //};

    //function loadPartialForCancel() {

    //    var cancel = $("div.my-cancel");

    //    console.log(cancel);

    //    cancel.one("click", "a", loadPartialView);
    //}

    //menuFunctions.on("click", "a.dropdown-item", loadPartialView);
});