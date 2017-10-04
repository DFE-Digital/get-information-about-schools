(function() {

    var filterForm = $('#indi-filter-form');
    var clearLinks = filterForm.find('.filter-clear');
    var resultsContainer = $('#results-container');
    var filterIntent;
    var downloadLink = $('.download-link');
    var downloadBaseUrl = '/independent-schools/download?';
    var plsWait = '<div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';
    var searchParams;

    $('#filter-submit').addClass('hidden');

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
        resultsContainer.addClass('pending-results-update');
        filterIntent = window.setTimeout(function () {
            resultsContainer.html(plsWait);
            searchParams = $('#filter-form-ind').serialize();
            
            getResults();
        }, 1200);
        
    });

    filterForm.find('.form-control').on('focus', function() {
        window.clearTimeout(filterIntent);
    });

    filterForm.find('.filter-button').on('click', function(e) {
        e.preventDefault();
        window.clearTimeout(filterIntent);
        resultsContainer.html(plsWait);
        searchParams = $('#filter-form-ind').serialize();
        getResults();
    });

    $('#set-saver').on('click',
        function(e) {
            e.preventDefault();
            var params = $('#option-select-local-authority').find(':input').serialize();
            window.location = '/independent-schools/predefined-local-authority-sets/create?' + params;

        });

}());
