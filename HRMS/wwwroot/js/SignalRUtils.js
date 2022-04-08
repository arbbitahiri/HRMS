var userChat = null;

const notification_type = {
    SUCCESS: 1,
    INFO: 2,
    WARNING: 3,
    ERROR: 4,
    QUESTION: 5
}

try {
    var userChat = sessionStorage.ChatUser;
    if (userChat != null && userChat != "null") {
        var chatTemp = JSON.parse(userChat);
        $.each(chatTemp, function (i, v) {
            if (v.gid == false) {
                if (v.Minimize == false) {
                    $.get('/General/Chat/_Chat?u=' + v.connid, function (data) {
                        $('.kt_chat_cont').append(data).show();
                    });
                } else {
                    $.get('/General/Chat/_Chat?u=' + v.connid, function (data) {
                        $('.kt_chat_cont').append(data).show();
                        $('.kt_chat_max_main[data-cid="' + v.connid + '"]').hide();
                        $('.kt_chat_min_main[data-cid="' + v.connid + '"]').show();
                    });
                }
            } else {
                $.get('/General/Chat/_ChatGroup?u=' + v.connid, function (data) {
                    $('.kt_chat_cont').append(data).show();
                });
            }
        })
    }
} catch (e) { }

Object.defineProperty(WebSocket, 'OPEN', { value: 1, });

// #region => Notification

connection.on("Notification", function (n) {
    debugger
    get_notifications();
    display_notification(n.description, n.title, n.url, n.target, n.notificationType, n.background);
});

connection.on("DisplayNotification", function (recent_notification, no_load_more, load_more) {
    $('#notifications_list').empty();

    if (recent_notification.length > 0) {
        $.each(recent_notification, function (i, v) {
            var n_icon = v.icon == null ? "fas fa-exclamation" : v.icon;
            var is_read_color = v.read ? "" : "background-color: #f4f6f9;";
            var read = v.language == 1 ? "Shëno si të lexuar" : "Mark as read";
            var unread = v.language == 1 ? "Shëno si të pa lexuar" : "Mark as unread";
            var n_delete = v.language == 1 ? "Largo" : "Delete";
            var is_read_text = v.read ? unread : read;
            var icon_read = v.read ? "far" : "fas";

            var notification = '<div class="dropdown-divider"></div>' +
                '<div class="dropdown-item" style="' + is_read_color + '">' +
                '<div class="d-flex flex-column flex-grow-1 font-weight-bold">' +
                '<div class="d-flex justify-content-between mb-2">' +
                '<a href="' + v.url + '" class="float-left text-dark">' +
                '<i class="' + n_icon + ' mr-2"></i>' + v.title +
                '</a>' +
                '<div class="float-right">' +
                '<a style="cursor: pointer;" class="text-dark mr-2" onclick="mark_as_read(\'' + v.notificationIde + '\', this)" data-toggle="tooltip" title="" data-placement="left" data-original-title="' + is_read_text + '">' +
                '<i class="' + icon_read + ' fa-bookmark"></i>' +
                '</a>' +
                '<a style="cursor: pointer;" class="text-dark" onclick="delete_notification(\'' + v.notificationIde + '\')" data-toggle="tooltip" title="" data-placement="left" data-original-title="' + n_delete + '">' +
                '<i class="far fa-times-circle"></i>' +
                '</a>' +
                '</div>' +
                '</div>' +
                '<div class="text-muted text-sm mb-1" style="text-overflow: ellipsis;overflow: hidden;white-space: nowrap;">' +
                v.description +
                '</div>' +
                '<span class="float-right text-muted text-sm" style="justify-content: end;display: flex;">' + v.daysAgo + '</span>' +
                '</div>' +
                '</div>';

            $('#notifications_list').append(notification);
        });

        var load_more_list;
        if (no_load_more == false) {
            load_more_list = '<div class="dropdown-divider"></div>' +
                '<a href="/Identity/Account/Manage/Notifications" style="cursor: pointer;" class="dropdown-item dropdown-footer"><b>' + load_more + '</b></a>';
        } else {
            load_more_list = '';
        }
        $('#notifications_list').append(load_more_list);
    } else {
        var notification = '<div class="dropdown-divider"></div>' +
            '<div class="dropdown-item">' +
            '<div class="d-flex flex-column flex-grow-1 font-weight-bold">' +
            '<div class="d-flex justify-content-between m-2">' +
            '<a href="#" class="float-left text-dark">' +
            '<i class="fas fa-info-circle mr-2"></i></i>Nuk keni njoftime'
            '</a>' +
            '</div>' +
            '</div>' +
            '</div>';

        $('#notifications_list').append(notification);
    }
});

connection.on('UnreadNotification', function (data) {
    if (data > 0) {
        $('#notification_count').text(data).show();
    } else {
        $('#notification_count').hide();
    }
});

// #endregion

// #region => Chat methods

connection.on('NewMessage', function (msg, sender, name, dt, img, ago, rec, type, emo) {
    $('.kt_chat_status[data-cid="' + sender + '"]').find('span').first().removeClass('label-danger').addClass('label-success');
    $('.kt_chat_status[data-cid="' + sender + '"]').find('span').last().text('Online');
    $('.kt_chat_list_status[data-cid="' + sender + '"]').removeClass('bg-success bg-warning bg-danger').addClass('bg-success');

    var newMsg = $('<div></div>', {
        class: 'd-flex flex-column mb-1 align-items-start',
        html: $('<div></div>', {
            class: 'd-flex align-items-center',
            html: $('<div></div>', {
                class: 'symbol symbol-circle symbol-40 mr-3',
                html: $('<img/>', {
                    alt: 'Pic',
                    src: img
                })
            })
        })
    })

    if (type == 1) {
        var newClass = 'mt-2 rounded text-dark-50 font-weight-bold text-left w-90 wrap-word';
        if (emo) {
            newClass += ' font-size-h1 ';
        } else {
            newClass += ' font-size-lg p-3 bg-light-success font-size-lg';
        }
        $($(newMsg).find('div').first()).append($('<div></div>', {
            class: newClass,
            text: msg
        }))
    } else if (type == 2) {
        newMsg.append($('<img/>', {
            src: '#',
            dataSrc: '/General/Chat/GetImage?path=' + msg,
            class: 'mt-2 rounded rounded-lg asyncLoad max-h-150px max-w-200px'
        }))
    } else if (type == 3) {
        newMsg.append($('<div></div>', {
            class: 'mt-2 rounded p-3 bg-light-success text-dark-50 font-weight-bold font-size-lg text-left w-90 wrap-word',
            html: $('<a></a>', {
                text: msg,
                target: '_blank',
                href: '/General/Chat/GetFile?p=' + msg
            })
        }))
    }

    $('.kt_left_msg_usr[data-cid="' + sender + '"]').text(msg);
    $('.kt_curr_message[data-cid="' + sender + '"]').find('.messages').append(newMsg);
    $(newMsg).find('.asyncLoad').attr('src', $(newMsg).find('.asyncLoad').attr('datasrc'))

    $(".kt_curr_message[data-cid='" + sender + "']").scrollTop($(".kt_curr_message[data-cid='" + sender + "']").prop("scrollHeight"));
})

connection.on('UnReadMessages', function (res) {
    $('#unread_msg').text(res + ' ' + resources.responseJSON["unread"]);
    if (res > 0) {
        $('#kt_chat_count').text(res).show();
    } else {
        $('#kt_chat_count').hide();
    }
})

connection.on('Status', function (res) {
    if (res.status == 0) {
        $('.kt_chat_status[data-cid="' + res.id + '"]').find('span').first().removeClass('label-success').addClass('label-danger');
        $('.kt_chat_status[data-cid="' + res.id + '"]').find('span').last().text('Appear away');
        $('.kt_chat_list_status[data-cid="' + res.id + '"]').removeClass('bg-success bg-warning bg-danger').addClass('bg-danger');
    } else {
        $('.kt_chat_status[data-cid="' + res.id + '"]').find('span').first().removeClass('label-danger').addClass('label-success');
        $('.kt_chat_status[data-cid="' + res.id + '"]').find('span').last().text('Online');
        $('.kt_chat_list_status[data-cid="' + res.id + '"]').removeClass('bg-success bg-warning bg-danger').addClass('bg-success');
    }
})

connection.on('NewGroupMessage', function (msg, sender, name, dt, img, ago, rec, type, emo) {
    $('.kt_chat_status[data-cid="' + CSS.escape(rec) + '"]').find('span').first().removeClass('label-danger').addClass('label-success');
    $('.kt_chat_status[data-cid="' + CSS.escape(rec) + '"]').find('span').last().text('Online');
    $('.kt_chat_list_status[data-cid="' + CSS.escape(rec) + '"]').removeClass('bg-success bg-warning bg-danger').addClass('bg-success');

    if (sender != connectionId) {
        var newMsg = $('<div></div>', {
            class: 'd-flex flex-column mb-1 align-items-start',
            html: $('<div></div>', {
                class: 'd-flex align-items-center',
                html: $('<div></div>', {
                    class: 'symbol symbol-circle symbol-40 mr-3',
                    html: $('<img/>', {
                        alt: 'Pic',
                        src: img
                    })
                })
            })
        })

        if (type == 1) {
            var newClass = 'mt-2 rounded text-dark-50 font-weight-bold text-left w-90 wrap-word';
            if (emo) {
                newClass += ' font-size-h1 ';
            } else {
                newClass += ' font-size-lg p-3 bg-light-success font-size-lg';
            }
            $($(newMsg).find('div').first()).append($('<div></div>', {
                class: newClass,
                text: msg
            }))
        } else if (type == 2) {
            newMsg.append($('<img/>', {
                src: '#',
                dataSrc: '/General/Chat/GetImage?path=' + msg,
                class: 'mt-2 rounded rounded-lg asyncLoad max-h-150px max-w-200px'
            }))
        } else if (type == 3) {
            newMsg.append($('<div></div>', {
                class: 'mt-2 rounded p-3 bg-light-success text-dark-50 font-weight-bold font-size-lg text-left w-90 wrap-word',
                html: $('<a></a>', {
                    text: msg,
                    target: '_blank',
                    href: '/General/Chat/GetFile?p=' + msg
                })
            }))
        }

        $('.kt_left_msg_usr[data-cid="' + CSS.escape(rec) + '"]').text(msg);
        $('.kt_curr_message[data-cid="' + CSS.escape(rec) + '"]').find('.messages').append(newMsg);
        $(newMsg).find('.asyncLoad').attr('src', $(newMsg).find('.asyncLoad').attr('datasrc'))

        $(".kt_curr_message[data-cid='" + CSS.escape(rec) + "']").scrollTop($(".kt_curr_message[data-cid='" + CSS.escape(rec) + "']").prop("scrollHeight"));
    }
})

connection.on('SendRecent', function (res) {
    $('#chat-custom-loader').hide()
    $('#topbar_recent_chat').children().slice(1).remove()
    $.each(res, function (i, v) {
        var chat_style = ''
        if (v.read == false) {
            chat_style = "background-color: ghostwhite;"
        }
        var recChat = $('<div></div>', {
            onClick: v.groupId == null ? 'LoadUserChat("' + v.id + '")' : 'LoadGroupChat("' + v.gide + '")',
            class: 'd-flex justify-content-between mb-6 rounded-lg cursor-pointer',
            style: chat_style,
            html: $('<div></div>', {
                class: 'd-flex',
                html: $('<div></div>', {
                    class: 'symbol symbol-60 symbol-light-primary mr-5',
                    html: $('<span></span>', {
                        class: 'symbol-label',
                        html: $('<img/>', {
                            src: (v.groupId != null ? "../../../Media/youth.png" : v.img == null ? "../../../Media/user.png" : v.img),
                            style: 'height: inherit; width: inherit; border-radius:15px'
                        })
                    })
                })
            }).append($('<div></div>', {
                class: 'd-flex flex-column font-weight-bold',
                html: $('<a></a>', {
                    class: 'text-dark text-hover-primary mb-1 font-size-lg',
                    href: '#',
                    text: v.name
                })
            }).append($('<span></span>', {
                class: 'text-muted',
                text: v.message
            })))
        }).append($('<span></span>', {
            class: 'font-weight-bolder text-primary py-1 font-size-md align-self-end min-w-40px',
            text: v.differenceTime
        }))

        $('#topbar_recent_chat').append(recChat);
    })
})

// #endregion

connection.start().then(function () {
    //connection.invoke('Notifications');
});
