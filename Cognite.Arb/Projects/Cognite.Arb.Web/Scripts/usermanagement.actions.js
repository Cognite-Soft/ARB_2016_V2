(function () {
    var actionsSelector = '.delete-user, .reset-passphrase-user, .reset-password-user';
    var fieldsSelector = '#FirstName, #LastName, #Email, #SelectedRole';
    var saveButtonsSelector = '.save-button';
    var cancelButtonsSelector = '.cancel-button';
    var addButtonSelector = 'input.add-user';

    $(function () {
        Initialize($('body'));
    });

    function Initialize(container) {
        BindActions(container);
        BindFields(container);
        BindSave(container);
        BindCancel(container);
        BindDisableAddButtonOnClick();
    }

    function ActionClick(e) {
        var target = $(e.target);
        var link = target.closest('.actions > a');
        var form = target.closest('form');

        var userId = link.attr('data-id');
        var action = link.attr('data-action');

        var url = '/UserManagement/' + action;
        var verificationToken = form.find('input[name="__RequestVerificationToken"]').val();

        $.post(
            url,
            { identifier: userId, __RequestVerificationToken: verificationToken },
            function (data) {
                var userRow = link.closest('form');
                ProcessResponceData(data, userRow);

                if (action == 'DeleteUser')
                    userRow.remove();
            });
    }

    function ProcessResponceData(data, userRow) {
        if (data.IsSucceeded) {
            jSuccess(data.Result);
        }
        else {
            jError(data.Result);
        }
    }

    function ShowSaveCancelButtons(e) {
        var sender = $(e.target);
        var form = sender.closest('form');
        form.find('.apply-buttons').show();
    }

    function UpdateClick(e) {
        e.preventDefault();
        var button = $(e.target);
        var userRow = button.closest('form');
        StartLoading(userRow);

        var formData = userRow.serialize();
        var url = '/UserManagement/Update';

        $.post(
            url,
            formData,
            function (data) {
                ProcessUpdateData(data, userRow);
            });
    }

    function StartLoading(userRow) {
        var form = $(userRow);
        var overlay = $('<div class="usermanagement-overlay"><img src="/Content/Images/loading.gif" alt="Loading..." /></div>');
        overlay.appendTo(form);
    }

    function ProcessUpdateData(data, userRow) {
        var dataObj = $(data);
        Initialize(dataObj);
        userRow.replaceWith(dataObj);
        jSuccess(userManagementUpdatedSuccessfully);
    }

    function CancelClick(e) {
        var form = $(e.target).closest('form');
        ResetFieldsValues(form);
        HideSaveCancelButtons(form);

        if (form.find('[data-valmsg-for="Email"]').length != 0)
            form.find('#Email').focus().blur();
    }

    function ResetFieldsValues(container) {
        var fields = container.find(fieldsSelector);
        fields.each(ResetFieldValue);
    }

    function ResetFieldValue() {
        var field = $(this);
        var def = field.attr('default');
        if (def != undefined) {
            field.val(def);
        }
    }

    function HideSaveCancelButtons(container) {
        $(container).find('.apply-buttons').hide();
    }

    function BindActions(container) {
        var elements = $(container).find(actionsSelector);
        elements.click(ActionClick);
    }

    function BindFields(container) {
        var elements = $(container).find(fieldsSelector);
        elements.change(ShowSaveCancelButtons)
                .keypress(ShowSaveCancelButtons);
    }

    function BindSave(container) {
        var elements = $(container).find(saveButtonsSelector);
        elements.click(UpdateClick)
    }

    function BindCancel(container) {
        var elements = $(container).find(cancelButtonsSelector);
        elements.click(CancelClick)
    }

    function BindDisableAddButtonOnClick() {
        var addButton = $(addButtonSelector);
        addButton.click(function () {
            var btn = $(this);
            var form = btn.closest("form");
            btn.attr("disabled", true);
            form.submit();
        });
    }
})();