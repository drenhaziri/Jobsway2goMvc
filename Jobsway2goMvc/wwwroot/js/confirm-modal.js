function getDefaultToastrOptions () {
    return {
        timeOut: 5000,
        positionClass: "toast-bottom-left"
    }
}

$('a[id^="deleteAction"]').click(function (e) {
    e.preventDefault();
    const name = $(this).data('name');
    const deleteUrl = $(this).data('delete-action-url');
    const reloadUrl = $(this).data('reload-url');

    const question = `Do you want to delete ${name}?`;
    UpdateGeneralTwoButtonsModal(question, (answer) => {

        if (answer) {

            $.ajax({
                type: "POST",
                url: deleteUrl,
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


