(function () {

    function captureFormState() {
        var state = {};
        $("input[type='checkbox'], select>option").each(function (i) {
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
        $("input[type='checkbox'], select>option").each(function (i) {
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
            if (ci) window.clearTimeout(ci);
            ci = setTimeout(refreshResults, 200); // when the clear button is clicked on the filters, multiple events come through; so using timer to prevent extraneous requests
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