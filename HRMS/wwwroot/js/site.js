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

const ReportType = {
    PDF: 1,
    EXCEL: 2,
    WORD: 3
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
    STUDENTSTAFF: 2,
    STUDENTCOLLEGE: 3,
    SELF: 4
}

const QuestionType = {
    NUMERICAL: 1,
    OPTIONAL: 2,
    TOPIC: 3,
    OPTIONALTOPIC: 5
}

const LeaveType = {
    ANNUAL: 1,
    SICK: 2,
    MATERNITY: 3,
    UNPAID: 4
}

$(document).ready(function () {
    $('.fade-in').hide().fadeIn(2000);

    resources = $.getJSON(`/Culture/General/${culture}.json`);

    if (culture == 'sq-AL') {
        $("input[type='text']").prop('spellcheck', false);
        $('textarea').prop('spellcheck', false);
    } else {
        $("input[type='text']").prop('spellcheck', true);
        $('textarea').prop('spellcheck', true);
    }

    if (Notification.permission === 'default') {
        $('#allow_notification').parent().removeClass('d-none');
    }
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
        window.location.href = '/Home/Index';
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
    show_loading();
    $.post('/Home/ChangeRole', {
        ide: ide
    }, function (data) {
        hide_loading();
        handle_success(data, SubmitPathType.RELOAD, "");
        setTimeout(function () {
            window.location.href = '/Home/Index';
        }, 2000);
    });
}

function change_mode(e) {
    show_loading();
    $.post('/Home/ChangeMode', {
        mode: $(e).is(':checked')
    }, function (data) {
        hide_loading();
        handle_success(data, SubmitPathType.RELOAD, "");
        $('[aria-labelledby="swal2-title"]').css('width', '12em');
    });
}

function format_currency(data) {
    return parseFloat(data, 10).toFixed(2);
}

function get_notifications(row_number) {
    if (row_number != null) {
        var x = String(row_number);
    }
    connection.invoke('Notifications', x);
}

function mark_as_read(ide) {
    $.post('/Home/MarkAsReadNotification', {
        ide: ide
    }, function (data) {
        connection.invoke('Notifications', "0");
    });
}

function delete_notification(ide) {
    $.post('/Home/DeleteNotification', {
        ide: ide
    }, function (data) {
        connection.invoke('Notifications', "0");
    });
}

function mark_all_as_read() {
    $.post('/Home/MarkAsReadAllNotification', function (data) {
        connection.invoke('Notifications', "0");
    });
}

function delete_all_notification() {
    $.post('/Home/DeleteAllNotification', function (data) {
        connection.invoke('Notifications', "0");
    });
}

function change_notification_mode(e) {
    if ($(e).is(':checked')) {
        Notification.requestPermission().then(function (permission) {
            if (permission == "granted") {
                subscribeUser();
            }
        });
    } else {
        unsubscribeUser();
    }
}

function display_notification(message, title, icon, url, target, type) {
    $('#recent_notification').load('/Home/RecentNotifications')
    $('#notifications_container').load('/Home/Notifications')

    var content = {
        message: message,
        title: title,
        icon: 'icon ' + icon,
        url: url,
        target: target
    };

    var notification_type = "";
    if (type == 1) {
        notification_type = "success"
    } else if (type == 2) {
        notification_type = "info"
    } else if (type == 3) {
        notification_type = "warning"
    } else if (type == 4) {
        notification_type = "danger"
    }

    var notify = $.notify(content, {
        type: notification_type,
        allow_dismiss: true,
        newest_on_top: true,
        mouse_over: true,
        showProgressbar: true,
        spacing: 10,
        timer: 2000,
        placement: {
            from: 'top',
            align: 'right'
        },
        offset: {
            x: '30',
            y: '30'
        },
        delay: '1000',
        z_index: '10000',
        animate: {
            enter: 'animated ' + 'bounce',
            exit: 'animated ' + 'pulse'
        }
    });
}
