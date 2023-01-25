const url = "searches.json";

$.getJSON(url, function (data) {
    var items = [];
    $.each(data, function (key, val) {
        if (val.length != 0) {
            for (var i = 0; i < val.length; i++) {
                items.push("<li>" + val[i] + "</li>");
                console.log(val);
            }
        }
        else {
            console.log("No elements");
        }
    });

    $("<ul/>", {
        "id": "listView",
        html: items.join("")
    }).appendTo("body");
});

