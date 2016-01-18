(function () {
    var allegationCommentButtonsSelector = "input.comment-allegation";
    var preliminaryDecisionCommentButtonSelector = "a.preliminary-decision-comment";
    var finalDecisionButtonsSelector = "a.final-decision-operation";
    var addPartiesCommentButtonSelector = "a.add-parties-comment";
    var partiesCommentTextSelector = "#party-comment";
    var partiesCommentsContainerSelector = "ul.parties-comments";

    var modelId = '';
    var verificationToken = '';

    $(function () {
        Initialize();
    });

    function Initialize() {
        modelId = $('input.model-id').val();
        verificationToken = $('input[name="__RequestVerificationToken"]').val();

        BindAllegationCommentActions();
        BindPreliminaryDecisionCommentActions();
        BindFinalDecisionActions();
        BindPartiesCommentAction();
        BindDiscussionsActions();
    }

    function BindAllegationCommentActions() {
        var buttons = $(allegationCommentButtonsSelector);
        buttons.click(AddAllegationComment);
    }

    function AddAllegationComment(e) {
        var sender = $(e.target);
        var allegation = sender.closest("li[data-allegation-id]");
        var commentType = GetCommentType(sender);
        var allegationId = allegation.attr("data-allegation-id");
        var text = allegation.find(".comment-allegation-text").val();
        if (text == undefined || text == '') return;
        var addText = allegation.find(".comment-allegation-text-add").val();
        AddAllegationCommentPost(allegationId, commentType, text, addText, allegation);
    }

    function GetCommentType(sender) {
        if (sender.hasClass("advice")) return "Advice";
        if (sender.hasClass("yes")) return "Yes";
        if (sender.hasClass("no")) return "No";
        return undefined;
    }

    function AddAllegationCommentPost(allegationId, commentType, text, addText, allegationContainer) {
        var actionUrl = GetActionUrl(commentType, allegationId);
        $.post(
            actionUrl,
            { text: text, addText: addText, __RequestVerificationToken: verificationToken },
            function (data) {
                ProcessData(data, commentType, text, addText, allegationContainer);
            });
    }

    function GetActionUrl(commentType, id) {
        var urlBase = "/Complaints/";
        if (commentType == "Advice")
            return urlBase + "AddAllegationCommentAdvice/" + id;
        if (commentType == "Yes")
            return urlBase + "AddAllegationCommentYes/" + id;
        if (commentType == "No")
            return urlBase + "AddAllegationCommentNo/" + id;
        return undefined;
    }

    function ProcessData(data, commentType, text, addText, allegationContainer) {
        if (data.IsSucceeded)
            CommentSucceeded(commentType, text, addText, allegationContainer);
        else
            CommentFailed(data.Result);
    }

    function CommentSucceeded(commentType, text, addText, allegationContainer) {
        var commentMarkup = '';

        if (window.location.href.toLowerCase().indexOf("/comments/") == -1) {
            commentMarkup = "<p><strong>" + commentType + "</strong></p><p>" + text + "</p><p>" + addText + "</p>";
        } else {
            var userName = $('#current-user').text();
            commentMarkup = '<div class="view">' +
                                '<div class="box mini feed-sub">' +
                                    '<div class="row">' +
                                        '<div class="large-2 columns center">' +
                                            '<img src="/Content/Images/user.gif"><br>' + 
                                            '<small>' + userName + '</small>' +
                                        '</div>' +
                                        '<div class="large-10 columns">' +
                                            '<p>' + text + '</p>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                            '</div>';
        }
        $(allegationContainer).find('.allegation-controls').html(commentMarkup);
    }

    function CommentFailed(message) {
        jError(message);
    }

    function BindPreliminaryDecisionCommentActions() {
        var button = $(preliminaryDecisionCommentButtonSelector);
        button.click(AddPreliminaryDecisionComment);
    }

    function AddPreliminaryDecisionComment(e) {
        var text = $('textarea.preliminary-decision-comment-text').val();
        SendPreliminaryDecisionComment(text, modelId);
    }

    function SendPreliminaryDecisionComment(text, modelId) {
        var url = "/Complaints/AddPreliminaryDecisionComment/" + modelId;
        $.post(
            url,
            { text: text, __RequestVerificationToken: verificationToken },
            function (data) {
                if (data.IsSucceeded)
                    window.location.href = window.location.href;
                else
                    jError(data.Result);
            });
    }

    function BindFinalDecisionActions() {
        var buttons = $(finalDecisionButtonsSelector);
        buttons.click(AddFinalDecision);
    }

    function AddFinalDecision(e) {
        var sender = $(e.target);
        var action = sender.attr('data-action');
        var url = GetFinalDecisionActionUrl(action);
        var comment = $('textarea.decision-changed-comment').val();
        $.post(
            url,
            { text: comment, __RequestVerificationToken: verificationToken },
            function (data) {
                if (data.IsSucceeded)
                    window.location.href = window.location.href;
                else
                    jError(data.Result);
            });
    }

    function GetFinalDecisionActionUrl(action) {
        return "/Complaints/AddFinalDecision" + action + "/" + modelId;
    }

    function BindPartiesCommentAction() {
        $(addPartiesCommentButtonSelector).click(AddPartiesComment);
    }

    function AddPartiesComment() {
        var text = $(partiesCommentTextSelector).val();
        if (text != undefined && text != '')
            SendPartiesComment(text);
    }

    function SendPartiesComment(text) {
        var url = "/Complaints/AddPartiesComment/" + modelId;

        $.post(
            url,
            { text: text, __RequestVerificationToken: verificationToken },
            function (data) {
                if (data.IsSucceeded)
                    AddPartiesCommentToDom(text);
                else
                    jError(data.Result);
            });
    }

    function AddPartiesCommentToDom(text) {
        var comment = $('<li class="view"><a href="" title="View Document"><i class="fa fa-file-text fa-fw"></i>' + text + '</a></li>');
        var container = $(partiesCommentsContainerSelector);

        comment.appendTo(container);
        $(partiesCommentTextSelector).val('');

        var def = container.find('li.default');
        if (def.length > 0)
            def.remove();
    }

    var newPostButtonSelector = "a.new-post-button";
    var replyButtonsSelector = "a.reply-button";

    function BindDiscussionsActions() {
        var newPostButton = $(newPostButtonSelector);
        var replyButtons = $(replyButtonsSelector);

        newPostButton.click(CreateNewPost);
        replyButtons.click(ReplyToPost);
    }

    function CreateNewPost(e) {
        var sender = $(e.target);
        var text = $('.new-post-area').val();
        if (text != undefined && text != '')
            SendNewPost(sender, text);
    }

    function SendNewPost(sender, text) {
        var url = "/Complaints/AddNewPost/" + modelId;

        $.post(
            url,
            { text: text, __RequestVerificationToken: verificationToken },
            function (data) {
                if (data.IsSucceeded)
                    window.location.href = window.location.href;
                else
                    jError(data.Result);
            });
    }

    function ReplyToPost(e) {
        var sender = $(e.target);
        var postId = sender.attr('data-comment-id');
        var text = $('#' + postId).find('textarea').val();
        if (text != undefined && text != '')
            SendReplyOnPost(postId, text);
    }

    function SendReplyOnPost(postId, text) {
        var url = "/Complaints/AddReplyToPost/" + modelId;

        $.post(
            url,
            { text: text, postId: postId, __RequestVerificationToken: verificationToken },
            function (data) {
                if (data.IsSucceeded)
                    window.location.href = window.location.href;
                else
                    jError(data.Result);
            });
    }
})();