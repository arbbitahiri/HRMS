// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var resources = null;

const ErrorStatus = {
    SUCCESS: 1,
    ERROR: 2,
    WARNING: 3,
    INFO: 4
}

const MethodType = {
    GET: 1,
    POST: 2,
    PUT: 3
}

const LookUpTable = {
    DOCUMENT: 1
}

const SubmitPathType = {
    RELOAD: 1,
    NORELOAD: 2,
    PATH: 3
}

const StatusType = {
    APPROVED: 1,
    REJECTED: 2,
    PENDING: 3
}

const EvaluationType = {
    MANAGER: 1,
    STUDENT: 2,
    SELF: 3
}

const QuestionType = {
    NUMERICAL: 1,
    OPTIONAL: 2,
    TOPIC: 3
}

$(document).ready(function () {
    $('.fade-in').hide().fadeIn(2000);

    resources = $.getJSON(`/Culture/General/${culture}.json`);
});

function show_loading() {
    $('#mdl_load').modal('show');
}

function hide_loading(timeout = 0) {
    setTimeout(function () {
        $('#mdl_load').modal('hide');
    }, timeout);
}

function handle_success(data, path_type, path) {
    if (data.status == ErrorStatus.SUCCESS) {
        Swal.fire({
            icon: 'success',
            title: data.title,
            text: data.description,
            timer: 2500,
            showConfirmButton: false
        }).then((result) => {
            if (path_type == SubmitPathType.RELOAD) {
                window.location.reload();
            } else if (path_type == SubmitPathType.PATH) {
                window.location.href = path;
            } else { }
        });
    } else if (data.status == ErrorStatus.WARNING) {
        Swal.fire({
            icon: 'warning',
            title: data.title,
            text: data.description,
            confirmButtonText: 'Okay'
        });
    } else if (data.status == ErrorStatus.ERROR) {
        Swal.fire({
            icon: 'error',
            title: data.title,
            text: data.description,
            confirmButtonText: 'Okay'
        });
    } else if (data.status == ErrorStatus.INFO) {
        Swal.fire({
            icon: 'info',
            title: data.title,
            text: data.description,
            confirmButtonText: 'Okay'
        });
    }
}

$(document).on('submit', 'form:not(.noLoading)', function () {
    show_loading();
    $(this).find('button[type="submit"]').attr('disabled', 'disabled');
});

$(document).ajaxComplete(function () {
    hide_loading(500);
    $(this).find('button[type="submit"]').removeAttr('disabled', 'disabled');
});

$(document).ajaxError(function (error) {
    if (error.handleObj.handler.arguments[1].status == 403) {
        Swal.fire({
            icon: 'error',
            title: resources.responseJSON["AccessDenied"],
            text: resources.responseJSON["AccessDeniedLong"],
            confirmButtonText: resources.responseJSON["Okay"]
        });
    } else if (error.handleObj.handler.arguments[1].status == 0) {
        //It is not an error. It's just abortion of an ajax call from another ajax call.
    } else if (error.handleObj.handler.arguments[1].status == 307) {
        window.location.href = '/Home/Index';
    } else {
        Swal.fire({
            icon: 'error',
            title: resources.responseJSON["Error"],
            text: resources.responseJSON["ErrorProcessingData"],
            confirmButtonText: resources.responseJSON["Okay"]
        });
    }
});

$(document).on('invalid-form.validate', 'form', function () {
    hide_loading();
});

$(document).ajaxStart(function () {
    Pace.restart();
});

function change_role(ide) {
    $.post('/Home/ChangeRole', {
        ide: ide
    }, function (data) {
        window.location.href = '/Home/Index';
        handle_success(data, SubmitPathType.RELOAD, "");
    });
}

function change_mode(e) {
    $.post('/Home/ChangeMode', {
        mode: $(e).is(':checked')
    }, function (data) {
        handle_success(data, SubmitPathType.RELOAD, "");
        $('[aria-labelledby="swal2-title"]').css('width', '12em');
    });
}
