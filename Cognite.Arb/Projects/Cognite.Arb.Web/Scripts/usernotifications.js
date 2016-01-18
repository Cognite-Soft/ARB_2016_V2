(function () {
    $(function () {
        if (arbCogniteCurrentUserNotifications != undefined && arbCogniteCurrentUserNotifications != '') {
            ShowNotifications(arbCogniteCurrentUserNotifications);
            BindCloseAction();
        }
    });

    function ShowNotifications(notifications)
    {
        for (var i = 0; i < notifications.length; i++) {
            ShowNotification(notifications[i]);
        }
    }

    function ShowNotification(notification) {
        var message = BuildNotificationMessage(notification);
        jSuccess(message, { autoHide: false });
        $('div.jSuccess').css('cursor', 'auto');
    }

    function BuildNotificationMessage(notification) {
        var message =
            '<div class="user-notification-container" data-notificationid="' + notification.Id + '">' +
                '<p class="user-notification-header">' +
                    '<b>' + notification.From + '</b> sent you a message at ' + notification.DateTime + '.' +
                '</p>' +
                '<p class="user-notification-message">' + notification.Message + '</p>' +
                '<p class="user-notification-reply">You can send a reply: <a href="mailto:' + notification.FromMail + '">' + notification.FromMail + '</a></p>' +
                '<p class="user-notification-footer">Click <a class="user-notification-close">here</a> to dismiss</p>'
            '</div>';

        return message;
    }

    function BindCloseAction() {
        $('div.jSuccess .user-notification-close').click(DismissNotification);
    }

    function DismissNotification(e) {
        var sender = $(e.target);
        var notification = sender.closest('div.user-notification-container');
        var notificationId = notification.attr('data-notificationid');
        DismissNotificationPost(notificationId);
        notification.parent().remove();
    }

    function DismissNotificationPost(id) {
        var url = '/Notification/Dismiss/' + id;
        $.post(url);
    }
})();