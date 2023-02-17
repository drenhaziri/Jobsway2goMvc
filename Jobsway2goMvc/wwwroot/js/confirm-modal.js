function getDefaultToastrOptions () {
    return {
        timeOut: 5000
    }
}

$('a[id^="deleteAction"]').click(function (e) {
    e.preventDefault();
    const name = $(this).data('name');
    const deleteUrl = $(this).data('delete-action-url');
    const reloadUrl = $(this).data('reload-url');

    const question = `Do you want to delete ${name}?`;
    const token = $('input[name="__RequestVerificationToken"]').val();

    UpdateGeneralTwoButtonsModal(question, (answer) => {

        if (answer) {

            $.ajax({
                type: "POST",
                url: deleteUrl,
                headers:{
                    "RequestVerificationToken": token
                },
                data: {
                    __RequestVerificationToken: token
                },
                success: function(data) {
                    toastr.success(`${name} deleted successfully`, '', getDefaultToastrOptions());
                    setTimeout(() => {
                        window.location.href = reloadUrl;
                    }, 1000)
                },
                error: function(err) {
                    toastr.error(`${name} could not be deleted`, '', getDefaultToastrOptions());
                    setTimeout(() => {
                        window.location.reload();
                    }, 1000)

                }
            });
        }
    });
});


