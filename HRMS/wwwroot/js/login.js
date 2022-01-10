const ErrorStatus = {
    SUCCESS: 1,
    ERROR: 2,
    WARNING: 3,
    INFO: 4
}

function show_loading() {
    $('#mdl_load_login').modal('show');
}

function hide_loading(timeout = 0) {
    setTimeout(function () {
        $('#mdl_load_login').modal('hide');
    }, timeout);
}

$(document).on('submit', 'form:not(.noLoading)', function () {
    show_loading();
    $(this).find('button[type="submit"]').attr('disabled', 'disabled');
});

$(document).ajaxComplete(function () {
    hide_loading(500);
    $(this).find('button[type="submit"]').removeAttr('disabled', 'disabled');
});