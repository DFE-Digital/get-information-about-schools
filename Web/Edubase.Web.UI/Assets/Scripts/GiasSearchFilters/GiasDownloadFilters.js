import supportsHistory from '../GiasHelpers/supportsHistory';
import GiasFilterValidation from './GiasFilterValidation'
import Vue from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
import { initAll } from 'govuk-frontend';

const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

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
    updateErrors: function (errObj) {
      this.errors = [];
      if (errObj) {
        this.errors.push(errObj);
      }
    }
  }
});

const GiasDownloadResults = {
	init: function () {
		$('#select-all').on('click', (e)=> {
			e.preventDefault();
			check(true);
		});

		$('#clear-all').on('click', (e)=> {
			e.preventDefault();
			check(false);
		});

		$('#downloadSelected').on('click', (e)=> {
			var anyChecked = false;
			$('input:checkbox').each(function(){
			  if (this.checked) {
				anyChecked = true;
			  }
			});

			if (anyChecked == false) {
			  e.preventDefault();
			  $(this).okCancel({
				ok: function(){
				  this.closeModal();
				},
				cancel: null,
				title: 'No files selected',
				content: 'Please select at least one file to download.',
				immediate: true
			  });
			  $(this).removeData('okCancel');
			}
		});

		function check(source) {
		  $('input:checkbox').prop('checked',source);
		}
	},

	reload: function() {
		const $updatedSection = document.getElementById('results-container');
		initAll({scope: $updatedSection });
	}
}

const GiasDownloadFilters = {
  bannerDefaultText: "Select date of files to download",
  errorMessages: [
    'Please enter a date',
    'Please enter a valid date',
    'Please check the date(s) you have entered'
  ],

  getResults: function () {
    const pageBanner = $('#downloadsTitle');
    const resultsContainer = $('#results-container');
    const resultsNotification = $('#results-notification');
    const resultsUrl = '/Downloads/results-js';
    const searchParams = $('#download-filters').serialize();
    let today = new Date();

    resultsContainer.html('<div class="gias-wait-mask gias-wait-mask--inline"><div class="lds-spinner"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div><span class="govuk-visually-hidden">Please wait</span></div>');
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
          pageBanner.text(GiasDownloadFilters.bannerDefaultText);
        } else {
          pageBanner.text("Files available to download from " + parseInt($('#FilterDate_Day').val(), 10) + " " + monthNames[$('#FilterDate_Month').val() - 1] + " " + $('#FilterDate_Year').val());
        }

        resultsContainer.removeClass('pending-results-update');
        resultsNotification.html('');
		GiasDownloadResults.init();
		GiasDownloadResults.reload();
      },
      error: function () {
        $('#ajax-error-message').removeClass('hidden');
        resultsContainer.removeClass('pending-results-update').html('');
        resultsNotification.html('');
      }
    });

  },
  init: function () {
    const $radios = $('#download-radios').find('.govuk-radios__input');

    $radios.on('change', function () {
      if ($('#search-type-Latest').is(':checked')) {
        GiasDownloadFilters.getResults();
      }
    });

    $('#filter-apply').on('click', function (e) {
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
      today.setHours(0, 0, 0, 0);


      const dateContainsEmpty = [inputYear, inputMonth, inputDay].filter((dateElement) => {
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

      if (GiasFilterValidation.validateDate({day: inputDay, month: inputMonth, year: inputYear})) {
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

      let userDate = new Date(inputYear, inputMonth - 1, inputDay);
      userDate.setHours(0, 0, 0, 0);

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

	GiasDownloadResults.init();
  }
}

export default GiasDownloadFilters;
