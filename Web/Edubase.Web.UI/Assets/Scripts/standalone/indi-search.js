(function() {

    var filterForm = $('#indi-filter-form');
    var clearLinks = filterForm.find('.filter-clear');
    var resultsContainer = $('#results-container');
    var filterIntent;
    var downloadLink = $('.download-link');
    var downloadBaseUrl = '/independent-schools/download?';
    var plsWait = '<div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';
    var searchParams;
    var filterError = false;

    $('#filter-submit').addClass('hidden');

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
                var toDate = new Date(toDateValues[2],toDateValues[1], toDateValues[0]);
                if (toDate < fromDate) {
                    canSubmit = false;                    
                    rangeError = true;
                }
            }

           

            if (n + 1 === filters.length ) {
                if (!canSubmit) {
                    $('#date-filter').find('.form-group').addClass('error');               
                    if (rangeError) {
                        $('#date-filter').find('.date-range-error').slice(0,1).removeClass('hidden');
                    } else {
                        $('#date-filter').find('.error-message').slice(0, 1).removeClass('hidden'); 
                    }
                    //
                }
                filterError = canSubmit;

            }
        });
        
    }

    var getResults = function () {
        filterForm.find('input').prop('disabled', 'disabled');
        clearLinks.addClass('clear-disabled');
        if (GOVUK.support.history()) {
            history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
        }
        $.ajax({
            url: '/independent-schools/results-js',
            data: searchParams,
            success: function (data, status, xhr) {
                resultsContainer.html(data);
                downloadLink.removeClass('hidden');

                downloadLink.attr('href', downloadBaseUrl + searchParams);
                resultsContainer.removeClass('pending-results-update');
                filterForm.find('input').removeAttr('disabled');
                clearLinks.removeClass('clear-disabled');
                if (Number(xhr.getResponseHeader("x-count")) === 0) {
                    downloadLink.addClass('hidden');
                }
            }
        });
    }

    clearLinks.on('click', function (e) {
            e.preventDefault();
            if (!$(this).hasClass('clear-disabled')) {
                var $govUkSelect = $(this).parents('.govuk-option-select');
                if ($govUkSelect.hasClass('date-filters') || $govUkSelect.hasClass('age-filter')) {
                    $govUkSelect.find('input[type="text"]').val('');

                } else {
                    var selectedFilters = $(this)
                        .next('.options-container')
                        .find('input')
                        .filter(function (n, item) {
                            return $(item).prop('checked');
                        });

                    selectedFilters.click();
                    $(this).removeClass('active-clear');
                   
                }
            }
        });

    filterForm.find('.trigger-result-update').on('change', function() {
        window.clearTimeout(filterIntent);
         validateFilters();

         if (filterError){
            resultsContainer.addClass('pending-results-update');
            filterIntent = window.setTimeout(function () {
                resultsContainer.html(plsWait);
                searchParams = $('#filter-form-ind').serialize();
            
                getResults();
            }, 1200);
        }

        
        
    });

    filterForm.find('.form-control').on('focus', function() {
        window.clearTimeout(filterIntent);
    });

    filterForm.find('.filter-button').on('click', function(e) {
        e.preventDefault();
        window.clearTimeout(filterIntent);
        validateFilters();

        if (filterError) {            
            resultsContainer.html(plsWait);
            searchParams = $('#filter-form-ind').serialize();
            getResults();
        }
    });

    $('#set-saver').on('click',
        function(e) {
            e.preventDefault();
            var params = $('#option-select-local-authority').find(':input').serialize() + '&referrer=results&Mode=' + document.getElementById('Mode').value;
            window.location = '/independent-schools/predefined-local-authority-sets/create?' + params;

        });

}());
