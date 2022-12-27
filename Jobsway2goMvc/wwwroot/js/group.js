$(document).ready(function() {
    $("#IsPublic").change(function () {

        const Id = $(".switch-handle").data('id');
        const isPublic = $(this).is(':checked');
        
        const url = '/Groups/UpdatePrivacy/' + Id;
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(isPublic),
            contentType: "application/json",
            success: function (response) {
                //console.log('success ', response);
            },
            error: function (error) {
                //console.log('error ', error);
            }
        });

        return false;
    });
});