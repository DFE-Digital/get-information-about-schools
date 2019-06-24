(function () {
    var openState = true;
    var resultsContainer = $('#results-container');
    var resultsNotification = $('#results-notification');
    var filterPanel = $('#filter-container');
    var filterError = false;
    var downloadLink = $('.download-link');
    var downloadBaseUrl = '/ChangeHistory/Search/Download/?';
    var filterIntent = null;
    var searchParams = '';
    var plsWait = '<div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';
    var $sortLinks = $('#changes-table thead').find('a');

     function okClick() {
        this.closeModal();
     }
     function attachOkCancel () {
         $('#content').find('.download-link').okCancel({
            ok: okClick,
            cancel: null,
            title: 'Too many records',
            content: 'Please filter your search to fewer than 20,000 changes.',
            triggerEvent: 'click'
        });
    }

    function getResults() {
        resultsContainer.html(plsWait);
        resultsNotification.html('Please wait, loading search results');
        $('#ajax-error-message').addClass('hidden');
        filterPanel.find(':input').prop('disabled', 'disabled');
        filterPanel.find('.filter-clear').addClass('clear-disabled');


        var resultsUrl = isEstabSearch
            ? '/ChangeHistory/Search/Establishments/results-js'
            : '/ChangeHistory/Search/Groups/results-js';
        if (GOVUK.support.history()) {
            history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
        }
        $.ajax({
            url: resultsUrl,
            data: searchParams,
            success: function (data, status, xhr) {
                resultsContainer.html(data);
                downloadLink.removeClass('hidden');

                downloadLink.attr('href', downloadBaseUrl + searchParams);
                resultsContainer.removeClass('pending-results-update');
                filterPanel.find(':input').removeAttr('disabled');
                filterPanel.find('.filter-clear').removeClass('clear-disabled');

                var count;
                if (xhr.getResponseHeader("x-count")) {
                    count = Number(xhr.getResponseHeader("x-count"));
                }

                // Notifications
                if (count > 0) {
                    resultsNotification.html('Search results loaded. ' + count + ' records found.');
                } else {
                    downloadLink.addClass('hidden');
                    resultsNotification.html('Search results loaded. No records found.');
                }

                if (count > 19999) {
                    attachOkCancel();
                } else if ($('#content').find('.download-link').data().hasOwnProperty('okCancel')) {
                    $('#content').find('.download-link').data().okCancel.unbind();
                }
            },
            error: function () {
                $('#ajax-error-message').removeClass('hidden');
                resultsContainer.removeClass('pending-results-update').html('');
                filterPanel.find(':input').removeAttr('disabled');
                downloadLink.addClass('hidden');
            }
        });
    }

    function toggleFilters() {
        openState = !openState;
        if (openState) {
            $('#filter-toggle').text('Hide filters to show user information');
            $('#changes-table').addClass('table-tight');
          } else {
            $('#filter-toggle').text('Show filters');
            $('#changes-table').removeClass('table-tight');
        }

        $('#filters-open-state').val(openState);

        $sortLinks.each(function () {
            var href = $(this).attr('href');
            if (href.indexOf('filtersopen=') > -1) {
                $(this).attr('href', href.substr(0,href.indexOf('filtersopen=')) + 'filtersopen=' + openState);
            } else {
                $(this).attr('href', href + '&filtersopen=' + openState);
            }
        });
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
            var canSubmit = DfE.searchUtils.validateDateFilters('date-filter');

            if (canSubmit) {
                resultsContainer.html(plsWait);
                searchParams = $('#change-history-filters').serialize();
                getResults();
            }
        });

        var resultCount = Number(resultsContainer.find('.pagination p').slice(0,1).text().split(' ').slice(-1));
        if (resultCount > 19999) {
            attachOkCancel();
        }
    }


    bindEvents();


}());
