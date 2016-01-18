(function () {
    var autoCompleterSelector = ".autocompleteTagAsset";
    var completerClass = "autocompleteContainer";

    $(function () {
        Initialize();
    });

    function Initialize() {
        InitializeAutoCompleter();
    }

    function InitializeAutoCompleter() {
        $(autoCompleterSelector).each(function () {
            var suggester = $(this);
            var defaultValue = suggester.closest('div.tags').find('input.data-cases').val();

            var suggest = suggester.magicSuggest({
                allowFreeEntries: false,
                autoSelect: false,
                cls: completerClass,
                placeholder: 'Start typing to assign a case...',
                data: userManagementAssignCases,
                displayField: 'Value',
                valueField: 'Id',
                expandOnFocus: true,
                highlight: false,
                hideTrigger: true,
                sortDir: 'asc',
                sortOrder: 'Value',
                maxSuggestions: 5,
            });

            SetValues(suggest, defaultValue);
            
            $(suggest).on('blur', function () {
                var value = this.getValue();
                var jsonValue = JSON.stringify(value);
                UpdateAssignment(this, jsonValue);
            });
        });
    }

    function SetValues(suggest, defaultValue) {
        var values = JSON.parse(defaultValue.replace(/'/g, '"'));
        var selection = [];

        for (var i = 0; i < userManagementAssignCases.length; i++) {
            var currentCase = userManagementAssignCases[i];

            var selected = false;
            for (var j = 0; j < values.length; j++) {
                if (values[j] == currentCase.Id)
                {
                    selected = true;
                    break;
                }
            }

            if (selected)
                selection.push(currentCase);
        }

        suggest.setSelection(selection);
        suggest.collapse();
    }
    
    function UpdateAssignment(suggest, value) {
        var container = suggest.container;
        var userId = container.closest('div.tags').attr('data-userid');
        var form = container.closest('form');
        var verificationToken = form.find('input[name="__RequestVerificationToken"]').val();
        var url = "/UserManagement/UpdateUserCaseAssignment";

        DisableSuggest(suggest);

        $.post(
            url,
            { userId: userId, value: value, __RequestVerificationToken: verificationToken },
            function (data) {
                if (data.IsSucceeded)
                    UpdateSucceeded(data.Result, suggest);
                else
                    UpdateError(data.Result, suggest);

                EnableSuggest(suggest);
            });
    }

    function UpdateSucceeded(result, suggest) {
        var valueInput = suggest.container.closest('div.tags').find('input.data-cases');
        valueInput.val(JSON.stringify(suggest.getValue()));
        jSuccess(result);
    }

    function UpdateError(result, suggest) {
        var prevVal = suggest.container.closest('div.tags').find('input.data-cases').val();
        SetValues(suggest, prevVal);
        jError(result);
    }

    function DisableSuggest(suggest) {
        suggest.collapse();
        suggest.disable();
        suggest.container.find('span.ms-close-btn').hide();
        CreateProcessBar(suggest.container);
    }

    function CreateProcessBar(container) {
        var loading = $('<img class="loading-gif" src="/Content/Images/loading.gif" alt="Loading..." />');
        loading.appendTo(container);
    }

    function EnableSuggest(suggest) {
        suggest.enable();
        suggest.container.find('span.ms-close-btn').show();
        RemoveProcessBar(suggest.container);
    }

    function RemoveProcessBar(container) {
        var loading = container.find('img.loading-gif');
        loading.remove();
    }
})();