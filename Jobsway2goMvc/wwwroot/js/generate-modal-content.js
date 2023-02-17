function UpdateGeneralTwoButtonsModal(body, callback) {
    const confirm = $('#generalModalTwoButtons');
    confirm.find('.modal-header');
    confirm.find('.modal-title').text('Confirm');
    confirm.find('.modal-body').html(body);
    confirm.modal('show');


    confirm.find('#btn-cancel').html('Cancel').off('click').click(() => {
        confirm.modal('hide');
        callback(false);
    });

    confirm.find('#btn-submit').html('Submit').off('click').click((event) => {
        event.preventDefault();
        event.stopPropagation();
        callback(true);
        confirm.modal('hide');

    });

}