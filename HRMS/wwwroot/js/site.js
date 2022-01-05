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

$(document).ready(function () {
    $('.fade-in').hide().fadeIn(2000);

    resources = $.getJSON(`/Culture/General/${culture}.json`);
});

function ShowLoading() {
    $('#mdl_load').modal('show')
}

function HideLoading(timeout = 0) {
    setTimeout(function () {
         $('#mdl_load').modal('hide')
    }, timeout);
}

$(document).on('submit', 'form:not(.noLoading)', function () {
    ShowLoading();
    $(this).find('button[type="submit"]').attr('disabled', 'disabled');
});

$(document).ajaxComplete(function () {
    HideLoading(500);
    $(this).find('button[type="submit"]').removeAttr('disabled', 'disabled')
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
    HideLoading();
});

$(document).ajaxStart(function () {
    Pace.restart();
});
