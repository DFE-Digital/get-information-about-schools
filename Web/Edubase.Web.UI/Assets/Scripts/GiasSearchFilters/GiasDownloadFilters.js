import supportsHistory from '../GiasHelpers/supportsHistory';
import GiasFilterValidation from './GiasFilterValidation'
import Vue from 'vue';
import errorSummary from '../GiasVueComponents/ErrorSummary';

// 'little' Vue app to manage the error summary
const clientErrorSummary = new Vue({
  el: '#js-error-summary',
  components: {
    errorSummary,
  },
  data: {
    errors: []
  },
  methods: {
    updateErrors: function(errObj){
      this.errors = [];
      if (errObj) {
        this.errors.push(errObj);
      }
    }
  }
});

const GiasDownloadFilters = {
  bannerDefaultText: "Select date of files to download",
  errorMessages: [
    'Please enter a date',
    'Please check the date(s) you have entered',
    'Please enter a valid date'
  ],
  getResults: function(){
	const pageBanner = $('#downloadsTitle');
    const resultsContainer = $('#results-container');
    const resultsNotification = $('#results-notification');
    const resultsUrl = '/Downloads/results-js';
    const searchParams = $('#download-filters').serialize();
	  let today = new Date();

    resultsContainer.html('<div class="progress-indicator"><span class="govuk-visually-hidden">Please wait</span></div>');
    resultsNotification.html('Please wait, loading search results');

	if ($('input[name=SearchType]:checked').val() === "Latest" ||
		($('#FilterDate_Day').val() === '' && $('#FilterDate_Month').val() === '' && $('#FilterDate_Year').val() === '')) {
		$('#FilterDate_Day').val(today.getUTCDate());
		$('#FilterDate_Month').val(today.getUTCMonth() + 1);
		$('#FilterDate_Year').val(today.getUTCFullYear());
	}

    if (supportsHistory()) {
      history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
    }

    $.ajax({
      url: resultsUrl,
      data: searchParams,
      success: function (data, status, xhr) {
        resultsContainer.html(data);

      if (resultsContainer.find('#no-downloads-available').length !== 0) {
        pageBanner.html(GiasDownloadFilters.bannerDefaultText);
      } else {
        pageBanner.html("Files available to download from " + $('#FilterDate_Day').val() + "/" + $('#FilterDate_Month').val() + "/" + $('#FilterDate_Year').val());
      }

        resultsContainer.removeClass('pending-results-update');
        resultsNotification.html('');

      },
      error: function () {
        $('#ajax-error-message').removeClass('hidden');
        resultsContainer.removeClass('pending-results-update').html('');
        resultsNotification.html('');
      }
    });
  },
  init: function() {
    const $radios = $('#download-radios').find('.govuk-radios__input');

    $radios.on('change', function(){
      console.log($('#search-type-Latest').is(':checked'));
      if ($('#search-type-Latest').is(':checked')) {

        GiasDownloadFilters.getResults();
      }
    });


    $('#filter-apply').on('click', function(e){
      e.preventDefault();
      let today = new Date();
      const dateFilter = $('#filterDate');
      const inputYear = $('#FilterDate_Year').val();
      const inputMonth = $('#FilterDate_Month').val();
      const inputDay = $('#FilterDate_Day').val();
      const errorMessage = dateFilter.find('.govuk-error-message').not('.date-range-error').not('.range-error')

	    const pageBanner = $('#downloadsTitle');

      let message = '';

      errorMessage.addClass('hidden');
      dateFilter.removeClass('govuk-form-group--error');
      clientErrorSummary.updateErrors();

      today.setHours(0,0,0,0);


      const dateContainsEmpty = [inputYear, inputMonth, inputDay].filter((dateElement)=>{
        return dateElement === ''
      }).length > 0;

      if (dateContainsEmpty) {
        message = GiasDownloadFilters.errorMessages[0];
        errorMessage.removeClass('hidden').text(message);
        dateFilter.addClass('govuk-form-group--error');
        pageBanner.html(GiasDownloadFilters.bannerDefaultText);
        clientErrorSummary.updateErrors({
          href: '#FilterDate_Day',
          message: message
        });
        return;
      }

      if (GiasFilterValidation.validateDate({ day: inputDay, month: inputMonth, year: inputYear})) {
        message = GiasDownloadFilters.errorMessages[1];
        errorMessage.removeClass('hidden').text(message);
        dateFilter.addClass('govuk-form-group--error');
		    pageBanner.html(GiasDownloadFilters.bannerDefaultText);
        clientErrorSummary.updateErrors({
          href: '#FilterDate_Day',
          message: message
        });
        return;
      }

      let userDate = new Date(inputYear, inputMonth-1, inputDay);
      userDate.setHours(0,0,0,0);

      if (userDate.getTime() <= today.getTime()) {
        GiasDownloadFilters.getResults();
      } else {
        message = GiasDownloadFilters.errorMessages[2];
        errorMessage.removeClass('hidden');
        dateFilter.addClass('govuk-form-group--error');
		    pageBanner.html(GiasDownloadFilters.bannerDefaultText);

		    errorMessage.text(message);
        clientErrorSummary.updateErrors({
          href: '#FilterDate_Day',
          message: message
        });
      }
    });
  }
}

export default GiasDownloadFilters;
