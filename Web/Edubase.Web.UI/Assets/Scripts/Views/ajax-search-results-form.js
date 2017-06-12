(function () {
    var filterLimit = 200;
    var $filters = $('#filter-form').find(':checkbox, select > option');
    var searchType = DfE.Util.QueryString.get('searchtype');
    var selectedLas = [];
   

    function updateSearchedLas() {
        selectedLas = [];
        $('#option-select-local-authority').find('.trigger-result-update').filter(':checked').each(function () {
            var label = $(this).parent().clone();
            label.find('span, input').remove();
            var text = label.text();
            selectedLas.push('&lsquo;<span class="bold-small">' + $.trim(text) + '</span>&rsquo;');
        });
        
        selectedLas = selectedLas.join(', ');
        var lastComma = selectedLas.lastIndexOf(',');
        selectedLas = selectedLas.substring(0, lastComma) +
            ' and ' +
            selectedLas.substring(lastComma + 1, selectedLas.length);
        $('#la-list').html(selectedLas);
    }
    if (searchType === 'ByLocalAuthority') {
        updateSearchedLas();
    }

    function updateGovernorRoles() {
        var selectedGovRoles = [];
        $('#option-select-role-type').find('.trigger-result-update').filter(':checked').each(function() {
            var label = $(this).parent().clone();
            label.find('span, input').remove();
            var text = label.text();
            selectedGovRoles.push('&lsquo;<span class="bold-small">' + $.trim(text) + '</span>&rsquo;');
        });
        selectedGovRoles = selectedGovRoles.join(', ');
        var lastComma = selectedGovRoles.lastIndexOf(',');
        selectedGovRoles = selectedGovRoles.substring(0, lastComma) +
            ' and ' +
            selectedGovRoles.substring(lastComma + 1, selectedGovRoles.length);
        $('.governor-roles').html(selectedGovRoles);
    }

    function captureFormState() {
        var state = {};
        $filters.each(function (i) {
            var $ele = $(this);
            var id = $ele.attr("id");
            if (id) {
                if ($ele.is("input")) state[id] = $ele.prop("checked");
                else if ($ele.is("option")) state[id] = $ele.prop("selected");
            }
        });
        return state;
    }

    function restoreFormState(state) {
        $filters.each(function (i) {
            var $ele = $(this);
            if ($ele.is("input")) $ele.prop("checked", state[$ele.attr("id")]);
            else if ($ele.is("option")) $ele.prop("selected", state[$ele.attr("id")]);
        });
    }

    function captureInitialState() {
        var queryString = window.location.search;
        if (queryString && queryString.length > 1) queryString = queryString.substring(1);
        return { queryString: queryString, formState: captureFormState() };
    }

    $(function () {
        var $resultsElement = $("#results-container");
        if ($resultsElement.length === 0) {
            return;
        }

        $("#filter-submit").hide();
        var initialState = captureInitialState();
        var progressHtml = '<br/><br/><br/><div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';
        var $blanket = $("div.blanket");
        var downloadUrl = "search/preparedownload?";

        var ci = null;
        var refreshResults = function (queryString, suppressPushState) { // suppressPushState == when popping state/clicking browser->back
            $resultsElement.html(progressHtml);
            $blanket.show();
            captureFormState();
            queryString = queryString ? queryString : $("form").serialize();
            $.get("Search/results-js?" + queryString, function (html, textStatus, jqXHR) {
                var count = jqXHR.getResponseHeader("x-count");
                $(window).trigger({
                    type: 'ajaxResultLoad',
                    count: count
                });

                $("span.count").html(count);
                if (parseInt(count, 10) === 0) {
                    $("a.download-link").addClass('hidden');
                    window.scrollTo(0,0);
                } else {
                    $("a.download-link").removeClass('hidden');
                }

                $blanket.hide();
                $resultsElement.html(html);

                if (!suppressPushState) {
                    var parser = document.createElement('a');
                    parser.href = window.location.href;
                    if (GOVUK.support.history()) {
                        window.history.pushState({ queryString: queryString, formState: captureFormState() }, null, parser.pathname + "?" + queryString);
                    }
                }
                $("a.download-link").attr("href", downloadUrl + queryString);
            });
        };
        $(document).on("change", ".trigger-result-update", function () {
            var filterCount = $filters.filter(':checked, :selected').length;
            if (filterCount >= filterLimit) {
                $(this).okCancel({
                    cancel: null,
                    ok: function () {
                        return true;
                    },
                    immediate: true,
                    title: 'Filter limit reached',
                    content:
                        'You\'ve selected too many filters. You can either reduce the number of filters, or download the data and filter it offline.'
                });
                $(this).removeData('okCancel');
                $(this).prop('checked', false);

            } else {
                if (ci) window.clearTimeout(ci);
                ci = setTimeout(refreshResults, 200); // when the clear button is clicked on the filters, multiple events come through; so using timer to prevent extraneous requests
                if (searchType === 'ByLocalAuthority') {
                    updateSearchedLas();
                }
                if (searchType === 'Governor') {
                    updateGovernorRoles();
                }

            }
           
           
        });

        if (GOVUK.support.history()) {
            window.addEventListener('popstate', function (event) {
                var state = event.state ? event.state : initialState;
                restoreFormState(state.formState);
                refreshResults(state.queryString, true);
            }.bind(this));
        }
    });

})();