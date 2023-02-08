$.ajax({
    type: "GET",
    dataType: "json",
    url: "js/searches.json",
    success: function (data) {
        $.each(data, function (key, val) {
            if (val.length != 0) {
                for (var i = 0; i < val.length; i++) {
                    $("#dropdown ul").append('<li class="list-group-item"><a href="">' + val[i] + '</a><i class="fa fa-times" ></i></li>');
                    console.log(val.length);
                }
            }
            else {
                console.log("No elements");
            }
        });
    }
});