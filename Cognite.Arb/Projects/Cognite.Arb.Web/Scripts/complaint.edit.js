(function () {
    var membersInputsSelector = "input.panelmember";
    var deleteAllegationButtonsSelector = "a.delete-allegation";
    var deleteDateAndDetailsButtonsSelector = "a.delete-dateanddetail";

    $(function () {
        Initialize();
    });

    function Initialize() {
        BindAutoComplete();
        BindAllegationDelete();
        BindDatesAndDetailsDelete();
    }

    function BindAutoComplete() {
        $(membersInputsSelector).each(function () {
            var input = $(this);
            var users = panelmembersAvailableUsers;
            var valueSelector = '#' + input.attr('data-value-id');
            var valueInput = $(valueSelector);

            input.autocomplete({
                minLength: 0,
                source: function (req, response) {
                    var re = $.ui.autocomplete.escapeRegex(req.term);
                    var matcher = new RegExp("^" + re, "i");
                    response($.grep(users, function (item) { return matcher.test(item.Value); }));
                },
                select: function (e, ui) {
                    valueInput.val(ui.item.Id).change();
                    input.val(ui.item.Value);
                    return false;
                },
            })
            .on('focus', function (event) {
                $(this).autocomplete("search", this.value);;
            })
            .autocomplete("instance")._renderItem = function (ul, item) {
                return $("<li>")
                  .append("<a>" + item.Value + "</a>")
                  .appendTo(ul);
            };
        });
    }

    function BindAllegationDelete() {
        var deleteButtons = $(deleteAllegationButtonsSelector);
        deleteButtons.click(DeleteAllegationButtonClick);
    }

    function DeleteAllegationButtonClick(e) {
        var sender = $(e.target).closest(deleteAllegationButtonsSelector);
        var form = sender.closest('form');
        var allegationId = sender.attr('data-alleg-id');
        var syncId = $('input.syncronization-id').val();
        var url = '/Complaints/DeleteAllegation/' + allegationId;
        var verificationToken = form.find('input[name="__RequestVerificationToken"]').val();

        $.post(
            url,
            { syncId: syncId, __RequestVerificationToken: verificationToken },
            function (data) {
                ProcessData(data, sender);
            });
    }
    
    function BindDatesAndDetailsDelete() {
        var deleteButtons = $(deleteDateAndDetailsButtonsSelector);
        deleteButtons.click(DeleteDateAndDetailsButtonClick);
    }

    function DeleteDateAndDetailsButtonClick(e) {
        var sender = $(e.target).closest(deleteDateAndDetailsButtonsSelector);
        var form = sender.closest('form');
        var dateAndDetailId = sender.attr('data-dateanddet-id');
        var syncId = $('input.syncronization-id').val();
        var url = '/Complaints/DeleteDateAndDetail/' + dateAndDetailId;
        var verificationToken = form.find('input[name="__RequestVerificationToken"]').val();

        $.post(
            url,
            { syncId: syncId, __RequestVerificationToken: verificationToken },
            function (data) {
                ProcessData(data, sender);
            });
    }
    
    function ProcessData(data, sender) {
        if (data.IsSucceeded)
            window.location.href = window.location.href;
        else {
            $(sender).remove();
            jError(data.Result);
        }
    }
})();