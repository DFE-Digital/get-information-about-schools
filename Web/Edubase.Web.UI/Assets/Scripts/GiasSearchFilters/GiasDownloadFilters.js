import supportsHistory from '../GiasHelpers/supportsHistory';
import GiasFilterValidation from './GiasFilterValidation';

const GiasDownloadFilters = {
  bannerDefaultText: "Select date of files to download",
	
  getResults: function(){
	const pageBanner = $('#downloadsTitle');
    const resultsContainer = $('#results-container');
    const resultsNotification = $('#results-notification');
    const resultsUrl = '/Downloads/results-js';
    const searchParams = $('#download-filters').serialize();

    resultsContainer.html('<div class="progress-indicator"><span class="govuk-visually-hidden">Please wait</span></div>');
    resultsNotification.html('Please wait, loading search results');

    if (supportsHistory()) {
      history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
    }

    $.ajax({
      url: resultsUrl,
      data: searchParams,
      success: function (data, status, xhr) {
        resultsContainer.html(data);
		
		if (resultsContainer.find('#no-downloads-available').length != 0) {
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

// TODO validation needs effort + summary needs handling in the HTML and JS
    $('#filter-submit').on('click', function(e){
      e.preventDefault();
      let today = new Date();
      const errorMessage = $('#date-filter').find('.govuk-error-message').not('.date-range-error').not('.range-error')
      const dateIsValid = GiasFilterValidation.validateDateFilters('date-filter');
	  const pageBanner = $('#downloadsTitle');

      errorMessage.addClass('hidden');
      today.setHours(0,0,0,0);

      if (!dateIsValid) {
        errorMessage.removeClass('hidden');
		pageBanner.html(GiasDownloadFilters.bannerDefaultText);
        return;
      }

      let userDate = new Date($('#FilterDate_Year').val(), $('#FilterDate_Month').val()-1, $('#FilterDate_Day').val());
      userDate.setHours(0,0,0,0);

      if (userDate.getTime() <= today.getTime()) {
        GiasDownloadFilters.getResults();
      } else {
        errorMessage.removeClass('hidden');
		pageBanner.html(GiasDownloadFilters.bannerDefaultText);
      }
    });
  }
}

export default GiasDownloadFilters;
