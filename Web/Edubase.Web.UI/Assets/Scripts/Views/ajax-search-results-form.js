(function () {
    var filterLimit = 200;
    var $filters = $('#filter-form').find(':checkbox, select > option');
    var searchType = DfE.Util.QueryString.get('searchtype');
    var errorSummary = $('#js-filter-error-summary');
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

    function validateDate(dateObj) {
        var day = dateObj.day,
            month = dateObj.month,
            year = dateObj.year,

            dateError = false,
            months31 = [0, 2, 4, 6, 7, 9, 11];

        if (isNaN(day) || isNaN(month) || isNaN(year)) {
            dateError = true;
        }

        var isLeap = new Date(year, 1, 29).getMonth() === 1;

        if (isLeap && month === 1) {
            if (day > 29) {
                dateError = true;
            }
        } else if (month === 1) {
            if (day > 28) {
                dateError = true;
            }
        }

        if (months31.indexOf(month - 1)) {
            if (day < 1 || day > 31) {
                dateError = true;
            }
        } else {
            if (day < 1 || day > 30) {
                dateError = true;
            }
        }

        if (month < 0 || month > 11) {
            dateError = true;
        }

        return dateError;
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
            errorSummary.addClass('hidden');
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

                if (!suppressPushState && GOVUK.support.history()) {
                        window.history.pushState({ queryString: queryString, formState: captureFormState() }, null, window.location.href.split('?')[0] + "?" + queryString);
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

        function validateAgeFilters() {
            var filters = $('#EditSearchCollapse').find('.age-filter').filter(':visible');
            var canSubmit = true;
            errorSummary.addClass('hidden');
            errorSummary.find('.summary-range-error').addClass('hidden');
            filters.each(function(n, elem) {
                $(elem).find('.date-group').removeClass('error');
                $(elem).find('.error-message').addClass('hidden');

                var fields = $(elem).find('.form-control');
                var valid = true;
                $.each(fields,
                    function(m, field) {
                        if (field.value !== '' && isNaN(field.value)) {
                            valid = false;
                            canSubmit = false;
                        }
                    });

                if (!valid) {
                    $(elem).find('.date-group').addClass('error');
                    $(elem).find('.error-message').removeClass('hidden');
                    errorSummary.removeClass('hidden');
                    errorSummary.find('.summary-range-error').removeClass('hidden');
                    return;
                } 

                if (n + 1 === filters.length && canSubmit) {
                    refreshResults();
                }                    
            });
        }

        function validateDateFilters() {
            var filters = $('#EditSearchCollapse').find('.date-filters').filter(':visible');
            var canSubmit = true;
            errorSummary.addClass('hidden');
            errorSummary.find('.summary-date-error').addClass('hidden');
            filters.each(function(n, elem) {
                $(elem).find('.form-group').slice(0, 2).removeClass('error');
                $(elem).find('.error-message').addClass('hidden');

                var validDate = true;
                var dateObj = {};
                var fromDateFields = $(elem).find('.search-from-date .form-control');
                var toDateFields = $(elem).find('.search-to-date .form-control');

                var fromDateValues = $.map(fromDateFields, function (field) {
                    if (field.value.trim() !== '') {
                        return field.value;
                    }
                });


                var toDateValues = $.map(toDateFields, function (field) {
                    if (field.value.trim() !== '') {
                        return field.value;
                    }
                });

                if (fromDateValues.length > 0 && fromDateValues.length < 3) {
                    validDate = false;
                }
                if (toDateValues.length > 0 && toDateValues.length < 3) {
                    validDate = false;
                }

                if (fromDateValues.length === 0 && toDateValues.length === 0) {
                    validDate = false;
                }

                if (fromDateValues.length === 3 && validDate) {
                    dateObj.day = fromDateValues[0];
                    dateObj.month = fromDateValues[1] - 1;
                    dateObj.year = fromDateValues[2];

                    validDate = !validateDate(dateObj);
                }

                if (toDateValues.length === 3 && validDate) {
                    dateObj.day = toDateValues[0];
                    dateObj.month = toDateValues[1] - 1;
                    dateObj.year = toDateValues[2];

                    validDate = !validateDate(dateObj);
                }

                if (!validDate) {
                    $(elem).find('.form-group').slice(0, 2).addClass('error');
                    $(elem).find('.error-message').removeClass('hidden');
                    canSubmit = false;
                    errorSummary.removeClass('hidden');
                    errorSummary.find('.summary-date-error').removeClass('hidden');
                }

                if (n + 1 === filters.length && canSubmit) {
                    refreshResults();
                } 
            });
        }

        $('.age-filter').find('.filter-button').on('click',
            function (e) {
                e.preventDefault();
                validateAgeFilters();                
            });

        $('.date-filters').find('.filter-button').on('click',
            function (e) {
                e.preventDefault();
                validateDateFilters();                
            });

        $('#clear-filters').on('click', function(e) {
            e.preventDefault();
            $('#filter-form').find('input[type="text"]').val('');
            $('#filter-form').find('.clear-selections').click();
        });

        if (GOVUK.support.history()) {
            window.addEventListener('popstate', function (event) {
                var state = event.state ? event.state : initialState;
                restoreFormState(state.formState);
                window.setTimeout(function() {
                    refreshResults(state.queryString, true);
                },1000);
                
                
            }.bind(this));
        }
    });

})();