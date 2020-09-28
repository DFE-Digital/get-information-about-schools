(function () {
    var openState = true;
    var resultsContainer = $('#results-container');
    var resultsNotification = $('#results-notification');
  var filterPanel = $('#download-filters');
    var filterIntent = null;
    var searchParams = '';
    var plsWait = '<div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';

    function getResults() {
        resultsContainer.html(plsWait);
        resultsNotification.html('Please wait, loading search results');
        $('#ajax-error-message').addClass('hidden');
        filterPanel.find(':input').prop('disabled', 'disabled');

        var resultsUrl = '/Downloads/results-js';
        if (GOVUK.support.history()) {
            history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
        }
        $.ajax({
            url: resultsUrl,
            data: searchParams,
            success: function (data, status, xhr) {
                resultsContainer.html(data);
                resultsContainer.removeClass('pending-results-update');
                filterPanel.find(':input').removeAttr('disabled');
            },
            error: function () {
                $('#ajax-error-message').removeClass('hidden');
                resultsContainer.removeClass('pending-results-update').html('');
                filterPanel.find(':input').removeAttr('disabled');
            }
        });
    }

    function toggleFilters() {
        openState = !openState;
        if (openState) {
            $('#filter-toggle').text('Hide filters');
          } else {
            $('#filter-toggle').text('Show filters');
        }

        $('#filters-open-state').val(openState);

        $('#filter-toggle').toggleClass('filters-closed');
        filterPanel.toggleClass('hidden');
        resultsContainer.toggleClass('column-full column-two-thirds');
        $('.js-reveal-when-no-filter').toggleClass('visuallyhidden');
    }


    function bindEvents() {

        DfE.searchResults.setupGovUkSelects();

        $('#filter-toggle').on('click', function (e) {
            e.preventDefault();
            toggleFilters();
        });

        if (DfE.searchUtils.getUrlParam('filtersopen') === 'false') {
            $('#filter-toggle').click();
        }

        $('#date-type-filter').on('change', function () {
            $('#date-filter-type-label').text('Date ' + $('#date-type-filter option:selected').text().toLowerCase());
            $(this).parents('.govuk-option-select').find('.clear-selections')
                .css({ left: $('#date-filter-type-label').width() + 12 + 'px' });
        });

        filterPanel.find('.trigger-result-update').on('change', function () {
            window.clearTimeout(filterIntent);
            var canSubmit = DfE.searchUtils.validateDateFilters('date-filter');

            if (canSubmit) {
                resultsContainer.addClass('pending-results-update');
                filterIntent = window.setTimeout(function () {
                    searchParams = $('#download-filters').serialize();
                    getResults();
                }, 1200);
            }

        });

        filterPanel.find('.form-control').on('focus', function () {
            window.clearTimeout(filterIntent);
        });

        filterPanel.find('.button').on('click', function (e) {
            e.preventDefault();
            window.clearTimeout(filterIntent);
            var canSubmit = DfE.searchUtils.validateDateFilters('date-filter');

            if (canSubmit) {
                resultsContainer.html(plsWait);
                searchParams = $('#download-filters').serialize();
                getResults();
            }
        });
    }
  
    bindEvents();
}());
