// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function DisplayGeneralNotification(message, title) {
    setTimeout(function () {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            showMethod: 'slideDown',
            timeOut: 4000
        };
        toastr.info(message, title);

    }, 1300);
}

function DisplayPersonalNotification(message, title) {
    setTimeout(function () {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            showMethod: 'slideDown',
            timeOut: 4000
        };
        toastr.success(message, title);

    }, 1300);
}

//Notification show on navbar

function updateNotifications() {
    $.ajax({
        type: "GET",
        url: "/Notification/Index",
        dataType: "json",
        success: function (data) {
            if (data && data.length > 0) {
                $("#notificationsContainer").empty();
                $.each(data, function (i, notification) {
                    var readButton = "<button class='btn btn-outline-secondary btn-sm' onclick='markAsRead(" + notification.id + ")'>Mark as Read</button>";
                    $("#notificationsContainer").append(readButton + "<a class='dropdown-item'>" + notification.message + "</a>");
                });
            } else {
                $("#notificationsContainer").empty();
                $("#notificationsContainer").append("<a class='dropdown-item'> No notifications to show</a>");
            }
        }
    });
}

function markAsRead(id) {
    $.ajax({
        type: "POST",
        url: "/Notification/markAsRead",
        data: { id: id },
        success: function (data) {
            if (data.success) {
                updateNotifications();
            }
        }
    });
}

function markedAsRead() {
    $.ajax({
        type: "GET",
        url: "/Notification/markedAsRead",
        dataType: "json",
        success: function (data) {
            if (data && data.length > 0) {
                $("#notificationsMarkedAsRead").empty();
                $.each(data, function (i, notification) {
                    $("#notificationsMarkedAsRead").append("<a class='dropdown-item'>" + notification.message + "</a>");
                });
            } else {
                $("#notificationsMarkedAsRead").empty();
                $("#notificationsMarkedAsRead").append("<a class='dropdown-item'> No notifications to show</a>");
            }
        }
    });
}

function getNotificationCount() {
    $.ajax({
        type: "GET",
        url: "/Notification/countNotifications",
        success: function (data) {
            $("#notificationCount").text(data);
        }
    });
}

$(document).ready(function () {
    getNotificationCount();
    updateNotifications();
    markedAsRead();
    setInterval(getNotificationCount, 5000);
    setInterval(updateNotifications, 5000);
    setInterval(markedAsRead, 5000);
});
