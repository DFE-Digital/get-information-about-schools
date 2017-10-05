(function () {
    var openState = true;
    var resultsPanel = $('#results-panel');
    var filterPanel = $('#filter-container');
    var filterError = false;
    var downloadLink = $('.download-link');
    var downloadBaseUrl = '#/?';
    var filterIntent = null;
    var searchParams = '';
    var plsWait = '<div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';

    function validateFilters() {
        var filters = $('.date-filters').filter(':visible');
        var canSubmit = true;
        var rangeError = false;
        filters.each(function (n, elem) {
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


            if (fromDateValues.length === 3 && validDate) {
                dateObj.day = fromDateValues[0];
                dateObj.month = fromDateValues[1] - 1;
                dateObj.year = fromDateValues[2];

                validDate = !DfE.searchUtils.validateDate(dateObj);
            }

            if (toDateValues.length === 3 && validDate) {
                dateObj.day = toDateValues[0];
                dateObj.month = toDateValues[1] - 1;
                dateObj.year = toDateValues[2];

                validDate = !DfE.searchUtils.validateDate(dateObj);
            }

            if (!validDate) {
                $(elem).find('.form-group').slice(0, 2).addClass('error');
                $(elem).find('.error-message').removeClass('hidden');
                canSubmit = false;

            } else {
                var fromDate = new Date(fromDateValues[2], fromDateValues[1], fromDateValues[0]);
                var toDate = new Date(toDateValues[2], toDateValues[1], toDateValues[0]);
                if (toDate < fromDate) {
                    canSubmit = false;
                    rangeError = true;
                }
            }



            if (n + 1 === filters.length) {
                if (!canSubmit) {
                    $('#date-filter').find('.form-group').addClass('error');
                    if (rangeError) {
                        $('#date-filter').find('.date-range-error').slice(0, 1).removeClass('hidden');
                    } else {
                        $('#date-filter').find('.error-message').slice(0, 1).removeClass('hidden');
                    }
                    //
                }
                filterError = canSubmit;

            }
        });

    }

    function getResults() {
        $('#ajax-error-message').addClass('hidden');
        filterPanel.find('input').prop('disabled', 'disabled');
        filterPanel.find('.filter-clear').addClass('clear-disabled');
        if (GOVUK.support.history()) {
            history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
        }
        $.ajax({
            url: '/some-dark-void-probably-damp-too/results-js',
            data: searchParams,
            success: function (data, status, xhr) {
                resultsPanel.html(data);
                downloadLink.removeClass('hidden');

                downloadLink.attr('href', downloadBaseUrl + searchParams);
                resultsPanel.removeClass('pending-results-update');
                filterPanel.find('input').removeAttr('disabled');
                filterPanel.find('.filter-clear').removeClass('clear-disabled');
                if (Number(xhr.getResponseHeader("x-count")) === 0) {
                    downloadLink.addClass('hidden');
                }
            },
            error: function () {
                $('#ajax-error-message').removeClass('hidden');
                resultsPanel.removeClass('pending-results-update').html('');
                filterPanel.find('input').removeAttr('disabled');
                downloadLink.addClass('hidden');
            }
        });
    }

    function toggleFilters() {
        openState = !openState;
        if (openState) {
            $(this).text('Hide filters');
        } else {
            $(this).text('Show filters');
        }
        $(this).toggleClass('filters-closed');
        filterPanel.toggleClass('hidden');
        resultsPanel.toggleClass('column-full column-two-thirds');
    }


    function bindEvents() {
        $('#filter-toggle').on('click', function (e) {
            e.preventDefault();
            toggleFilters();
        });

        $('#date-type-filter').on('change', function () {
            $('#date-filter-type-label').text('Date ' + $('#date-type-filter option:selected').text().toLowerCase());
        });

        filterPanel.find('.trigger-result-update').on('change', function () {
            window.clearTimeout(filterIntent);
            validateFilters();

            if (filterError) {
                resultsPanel.addClass('pending-results-update');
                filterIntent = window.setTimeout(function () {
                    resultsPanel.html(plsWait);
                    searchParams = $('#change-history-filters').serialize();

                    getResults();
                }, 1200);
            }
            
        });

        filterPanel.find('.form-control').on('focus', function () {
            window.clearTimeout(filterIntent);
        });

        filterPanel.find('.filter-button').on('click', function (e) {
            e.preventDefault();
            window.clearTimeout(filterIntent);
            validateFilters();

            if (filterError) {
                resultsPanel.html(plsWait);
                searchParams = $('#change-history-filters').serialize();
                getResults();
            }
        });
    }

    
    bindEvents();
    

}());