import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';
import GiasFilterValidation from "./GiasFilterValidation";
import supportsHistory from "../GiasHelpers/supportsHistory";
import QueryString from '../GiasHelpers/QueryString';

const giasSearchChangeHistory = (function() {
  let openState = true;
  const resultsContainer = $('#results-container');
  const resultsNotification = $('#results-notification');
  const filterPanel = $('#filter-container');
  let filterError = false;
  const downloadLink = $('.download-link');
  const downloadBaseUrl = '/ChangeHistory/Search/Download/?';
  let filterIntent = null;
  let searchParams = '';
  const plsWait = '<div class="gias-wait-mask gias-wait-mask--inline"><div class="lds-spinner"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div><span class="govuk-visually-hidden">Please wait</span></div>';
  const $sortLinks = $('#changes-table thead').find('a');

  function okClick() {
    this.closeModal();
  }

  function attachOkCancel() {
    $('#main-content').find('.download-link').okCancel({
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
    $('#gias-mobile-filter-submit').find('.mobile-count').remove();
    $('#gias-mobile-filter-submit').append("<span class='gias-button-loader' id='button-loader'>&nbsp;</span>");

    const resultsUrl = isEstabSearch
      ? '/ChangeHistory/Search/Establishments/results-js'
      : '/ChangeHistory/Search/Groups/results-js';

    if (supportsHistory()) {
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

        let count;
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
        } else if ($('#main-content').find('.download-link').data().hasOwnProperty('okCancel')) {
          $('#main-content').find('.download-link').data().okCancel.unbind();
        }
        $('#button-loader').remove();
        $('#gias-mobile-filter-submit').append("<span class='mobile-count'> ("+ count+")</span>");
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
    const $filterToggle = $('#filter-toggle');
    openState = !openState;
    if (openState) {
      $filterToggle.text('Hide filters');
    } else {
      $filterToggle.text('Show filters');
    }

    $('#filters-open-state').val(openState);

    $filterToggle.toggleClass('filters-closed');
    filterPanel.toggleClass('hidden');
  }


  return {
    bindEvents: function () {
      $('#filter-toggle').on('click', function (e) {
        e.preventDefault();
        toggleFilters();
      });


      $('#date-type-filter').on('change', function () {
        $('#date-filter-type-label').text('Date ' + $('#date-type-filter option:selected').text().toLowerCase());
        $(this).parents('.govuk-option-select').find('.clear-selections')
          .css({left: $('#date-filter-type-label').width() + 12 + 'px'});
      });

      filterPanel.find('.trigger-result-update').on('change', function () {
        window.clearTimeout(filterIntent);
        const canSubmit = GiasFilterValidation.validateDateFilters('date-filter');

        if (canSubmit) {
          resultsContainer.addClass('pending-results-update');
          filterIntent = window.setTimeout(function () {
            searchParams = $('#filter-form').find(':input').serialize();
            getResults();
          }, 1200);
        }

      });

      filterPanel.find('.govuk-input').on('focus', function () {
        window.clearTimeout(filterIntent);
      });

      filterPanel.find('.filter-button').on('click', function (e) {
        e.preventDefault();
        window.clearTimeout(filterIntent);
        const canSubmit = GiasFilterValidation.validateDateFilters('date-filter');

        if (canSubmit) {
          resultsContainer.html(plsWait);
          searchParams = $('#filter-form').find(':input').serialize();
          getResults();
        }
      });

      const resultCount = Number(resultsContainer.find('.pagination p').slice(0, 1).text().split(' ').slice(-1));
      if (resultCount > 19999) {
        attachOkCancel();
      }

      $('#clear-filters, #clear-filters-additional').on('click', (e)=> {
        e.preventDefault();
        window.clearTimeout(filterIntent);
        filterPanel.find('input[type="text"]').val('');
        const selectedFilters = filterPanel
          .find('.options-container .trigger-result-update')
          .filter(function (n, item) {
            return $(item).prop('checked');
          });
        selectedFilters.prop('checked', false);

        filterPanel.find('.govuk-option-select').each(function(n, container){
          $(container).find('.trigger-result-update').slice(0, 1).trigger('change');
        });
      });
    }

  }
}());

export default giasSearchChangeHistory;
