(function () {
    var closestRowSelector = "div.row";
    var scheduleInputsSelector = "input.schedulecell";
    var scheduleValueInputsSelector = "input.schedulecell-hidden";
    var scheduleInputContainerSelector = "div.large-4.columns";
    var scheduleCompleterSelector = "input.schedulecell";

    $(function () {
        Initialize();
    });

    function Initialize() {
        BindAutoComplete();
        BindDataChange();
    }

    function BindAutoComplete() {
        $(scheduleInputsSelector).each(function () {
            var input = $(this);
            var users = scheduleAvailableUsers;
            var valueSelector = '#' + input.attr('data-value-id');
            var valueInput = $(valueSelector);

            input.autocomplete({
                minLength: 0,
                source: function(req, response) {
                    var re = $.ui.autocomplete.escapeRegex(req.term);
                    var matcher = new RegExp( "^" + re, "i" );
                    response($.grep(users, function(item){return matcher.test(item.Value); }) );
                },
                select: function (e, ui) {
                    valueInput.val(ui.item.Id).change();
                    input.val(ui.item.Value);
                    return false;
                },
            })
            .on('focus', function(event) {
                $(this).autocomplete("search", this.value);;
            })
            .autocomplete("instance")._renderItem = function (ul, item) {
                return $("<li>")
                  .append("<a>" + item.Value + "</a>")
                  .appendTo(ul);
            };
        });
    }

    function BindDataChange() {
        var inputs = $(scheduleValueInputsSelector);
        inputs.change(UpdateCell);
    }

    function UpdateCell(e) {
        var sender = $(e.target);
        DisableInput(sender);

        var row = sender.closest(closestRowSelector);
        var form = sender.closest("form");

        var cellIndex = parseInt(sender.attr("data-index"));
        var rowIndex = parseInt(row.attr("data-index"));
        var val = sender.val();
        var verificationToken = form.find('input[name="__RequestVerificationToken"]').val();
        var url = '/Schedule/Update';

        $.ajax({
            type: "POST",
            url: url,
            data: { row: rowIndex, col: cellIndex, value: val, __RequestVerificationToken: verificationToken },
            success: function (data) {
                if (data.IsSucceeded)
                    jSuccess(data.Result);
                else
                    jError(data.Result);

                EnableInput(sender);
            },
        });
    }

    function DisableInput(target) {
        var input = $(target);
        var container = input.closest(scheduleInputContainerSelector);
        var completer = container.find(scheduleCompleterSelector);
        completer.attr("disabled", true);
        CreateProcessBar(completer);
    }

    function CreateProcessBar(target) {
        var input = $(target);
        var container = input.closest(scheduleInputContainerSelector);
        var loading = $('<img class="loading-gif" src="/Content/Images/loading.gif" alt="Loading..." />');
        loading.appendTo(container);
    }

    function EnableInput(target) {
        var input = $(target);
        var container = input.closest(scheduleInputContainerSelector);
        var completer = container.find(scheduleCompleterSelector);
        completer.removeAttr("disabled");
        RemoveProcessBar(input);
    }

    function RemoveProcessBar(target) {
        var input = $(target);
        var container = input.closest(scheduleInputContainerSelector);
        var loading = container.find('img');
        loading.remove();
    }
})();