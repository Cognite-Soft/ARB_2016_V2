(function () {
    var actionsSelector = "span.case-action";

    $(function () {
        Initialize();
    });

    function Initialize() {
        var buttons = $(actionsSelector);
        buttons.click(ActionClick);
    }

    function ActionClick(e) {
        e.preventDefault()

        var button = $(e.target);
        var action = button.attr("data-action");
        var caseId = button.attr("data-id");

        var form = button.closest("form");
        var verificationToken = form.find('input[name="__RequestVerificationToken"]').val();
        
        var url = "/Complaints/" + action + "/" + caseId;
        $.post(
            url,
            { __RequestVerificationToken: verificationToken },
            function (data) {
                window.location.href = window.location.href;
            }
        );

        return false;
    }
})();