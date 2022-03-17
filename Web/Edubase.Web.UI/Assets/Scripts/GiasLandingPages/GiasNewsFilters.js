import supportsHistory from '../GiasHelpers/supportsHistory';
import Vue from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
import { initAll } from 'govuk-frontend';


const GiasNewsFilters = {
  getResults: function () {
    const $dropdown = $('#news-year');
	
	window.location.href = window.location.pathname+"?"+$.param({'year':$dropdown.val()})
  },
  init: function () {
    const $dropdown = $('#news-year');

    $dropdown.on('change', function () {
		GiasNewsFilters.getResults();
    });
  }
}

export default GiasNewsFilters;
