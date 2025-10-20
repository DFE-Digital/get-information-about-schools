import supportsHistory from '../GiasHelpers/supportsHistory';
import GiasFilterValidation from '../GiasSearchFilters/GiasFilterValidation';
import OptionSelect from '../GiasSearchFilters/OptionSelect';
import GiasFilterToggle from '../GiasSearchFilters/GiasFilterToggle';
const _throttle = require('lodash.throttle');

(function() {
  const filterForm = $('#indi-filter-form');
  const clearLinks = filterForm.find('.filter-clear');
  const $resultsContainer = $('#results-container');
  const $resultsNotification = $('#results-notification');
  let filterIntent;
  const downloadLink = $('.download-link');
  const downloadBaseUrl = '/independent-schools/download?';
  const plsWait = '<div class="gias-wait-mask gias-wait-mask--inline"><div class="lds-spinner"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div><span class="govuk-visually-hidden">Please wait</span></div>';
  let searchParams;
  let filterError = false;

  $('#filter-submit').addClass('hidden');

  const getResults = function () {
    $resultsContainer.html(plsWait);
    $resultsNotification.html('Please wait, loading search results');
    filterForm.find('input').prop('disabled', 'disabled');
    clearLinks.addClass('clear-disabled');
    $('#gias-mobile-filter-submit').find('.mobile-count').remove();
    $('#gias-mobile-filter-submit').append("<span class='gias-button-loader' id='button-loader'>&nbsp;</span>");
    if (supportsHistory()) {
      history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
    }
    $.ajax({
      url: '/independent-schools/results-js',
      data: searchParams,
      success: function (data, status, xhr) {
        $resultsContainer.html(data);
        downloadLink.removeClass('hidden');

        downloadLink.attr('href', downloadBaseUrl + searchParams);
        $resultsContainer.removeClass('pending-results-update');
        filterForm.find('input').removeAttr('disabled');
        clearLinks.removeClass('clear-disabled');
        var count;
        if (xhr.getResponseHeader("x-count")) {
          count = xhr.getResponseHeader("x-count");
        }
        if (count > 0) {
          $resultsNotification.html('Search results loaded. ' + count + ' establishments found.');
        } else {
          downloadLink.addClass('hidden');
          $resultsNotification.html('Search results loaded. No establishments found.');
        }
        $('#button-loader').remove();
        $('#gias-mobile-filter-submit').append("<span class='mobile-count'> ("+ count+")</span>");
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

    const canSubmit = GiasFilterValidation.validateDateFilters('date-filter');

    if (canSubmit){
      $resultsContainer.addClass('pending-results-update');
      filterIntent = window.setTimeout(function () {
        searchParams = $('#filter-form').find(':input').serialize();
        getResults();
      }, 1200);
    }

  });

  filterForm.find('.govuk-input').on('focus', function() {
    window.clearTimeout(filterIntent);
  });

  filterForm.find('.filter-button').on('click', function(e) {
    e.preventDefault();
    window.clearTimeout(filterIntent);
    const canSubmit = GiasFilterValidation.validateDateFilters('date-filter');

    if (canSubmit) {
      $resultsContainer.html(plsWait);
      searchParams = $('#filter-form').find(':input').serialize();
      getResults();
    }
  });

  filterForm.find('.govuk-option-select').optionSelect();


  $('#set-saver').on('click',
    function(e) {
      e.preventDefault();
      const params = $('#option-select-local-authority').find(':input').serialize() + '&referrer=results&Mode=' + document.getElementById('Mode').value;
      window.location = '/independent-schools/predefined-local-authority-sets/create?' + params;

    });

  $('#clear-filters, #clear-filters-additional').on('click', (e)=> {
    e.preventDefault();
    window.clearTimeout(filterIntent);
    filterForm.find('input[type="text"]').val('');
    const selectedFilters = filterForm
      .find('.options-container .trigger-result-update')
      .filter(function (n, item) {
        return $(item).prop('checked');
      });
    selectedFilters.prop('checked', false);

    filterForm.find('.govuk-option-select').each(function(n, container){
      $(container).find('.trigger-result-update').slice(0, 1).trigger('change');
    });
  });

}());

let popupFilters;
if ($(window).width() < 835) {
  popupFilters = new GiasFilterToggle();
}

window.addEventListener('resize',
  _throttle(function(){
    if($(window).width() < 835) {
      if (typeof popupFilters === 'undefined') {
        popupFilters = new GiasFilterToggle();
      } else {
        if(!popupFilters.initialised) {
          popupFilters.init();
        }
      }
    } else {
      if (typeof popupFilters !== 'undefined') {
        popupFilters.destroy()
      }
    }
  }, 250));
