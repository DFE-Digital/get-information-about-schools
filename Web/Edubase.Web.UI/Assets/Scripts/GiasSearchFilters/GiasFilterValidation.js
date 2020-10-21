var moment = require('moment');

const GiasFilterValidation = {
  validateRadiusFilter: function() {
    const errorSummary = $('#js-filter-error-summary');
    let fieldValue = $('#RadiusInMiles').val();

    errorSummary.addClass('hidden');
    errorSummary.find('.summary-radius-error').addClass('hidden');
    if (fieldValue.substr(0, 1) === '.') {
      fieldValue = '0' + fieldValue;
    }

    return  /^[0-9]{1,2}(\.[0-9]{1,2})?$/.test(fieldValue);
  },
  validateDate: function(dateObj) {
    let day = dateObj.day;
    let month = dateObj.month;
    let year = dateObj.year;

    if (day.length < 2) {
		day = "0" + day;
	}
	
	if (month.length < 2) {
		month = "0" + month;
	}

	return !moment(month + "/" + day + "/" + year, 'MM/DD/YYYY',true).isValid();
  },

  validateDateFilters: function(dateFilterId) {
    let canSubmit = true;
    const self = this;
    const errorSummary = $('#js-filter-error-summary');
    const $dateFilter = $('#' + dateFilterId);
    errorSummary.addClass('hidden');
    window.document.title = "Search results - GOV.UK";
    errorSummary.find('.' + dateFilterId + '-error').addClass('hidden');
    $dateFilter.find('.form-group').slice(0, 2).removeClass('error');
    $dateFilter.find('.govuk-error-message').addClass('hidden');

    let validFromDate = true;
    let validToDate = true;
    let dateObj = {};
    const fromDateFields = $dateFilter.find('.search-from-date .govuk-input');
    const toDateFields = $dateFilter.find('.search-to-date .govuk-input');

    const fromDateValues = $.map(fromDateFields, function (field) {
      if (field.value.trim() !== '') {
        return field.value;
      }
    });

    const toDateValues = $.map(toDateFields, function (field) {
      if (field.value.trim() !== '') {
        return field.value;
      }
    });

    if (fromDateValues.length > 0 && fromDateValues.length < 3) {
      validFromDate = false;
    }
    if (toDateValues.length > 0 && toDateValues.length < 3) {
      validToDate = false;
    }

    if (fromDateValues.length === 3 && validFromDate) {
      dateObj.day = fromDateValues[0];
      dateObj.month = fromDateValues[1];
      dateObj.year = fromDateValues[2];

      validFromDate = !self.validateDate(dateObj);
    }

    if (toDateValues.length === 3 && validToDate) {
      dateObj.day = toDateValues[0];
      dateObj.month = toDateValues[1];
      dateObj.year = toDateValues[2];

      validToDate = !self.validateDate(dateObj);
    }

    if (!validFromDate || !validToDate) {
      canSubmit = false;
      errorSummary.removeClass('hidden');
      errorSummary.find('.' + dateFilterId + '-error').removeClass('hidden');
      errorSummary.focus();
      window.document.title = "Error: Search results - GOV.UK";
    }

    const fromSection = $('#' + dateFilterId + ' .search-from-date');
    const toSection = $('#' + dateFilterId + ' .search-to-date');

    if (!validFromDate) {
      fromSection.find('.govuk-error-message').not('.date-range-error').removeClass('hidden');
      fromSection.find('.govuk-input').addClass('govuk-input--error');
    } else {
      fromSection.find('.govuk-error-message').not('.date-range-error').addClass('hidden');
      fromSection.find('.govuk-input').removeClass('govuk-input--error');
    }

    if (!validToDate) {
      toSection.find('.govuk-error-message').not('.date-range-error').removeClass('hidden');
      toSection.find('.govuk-input').addClass('govuk-input--error');
    } else {
      toSection.find('.govuk-error-message').not('.date-range-error').addClass('hidden');
      toSection.find('.govuk-input').removeClass('govuk-input--error');
    }
    return canSubmit;
  },

  validateAgeFilter: function(ageFilterId) {
    let canSubmit = true;
    const errorSummary = $('#js-filter-error-summary');
    const $ageFilter = $('#' + ageFilterId);
    errorSummary.addClass('hidden');
    errorSummary.find('.summary-range-error').addClass('hidden');
    $ageFilter.find('.date-group').removeClass('error');
    $ageFilter.find('.govuk-error-message').addClass('hidden');

    const fields = $ageFilter.find('.govuk-input');
    let valid = true;
    $.each(fields,
      function (m, field) {
        if (field.value !== '' && isNaN(field.value)) {
          valid = false;
          canSubmit = false;
        }
      });

    if (!valid) {
      $ageFilter.find('.govuk-error-message').removeClass('hidden');
      errorSummary.removeClass('hidden');
      errorSummary.find('.' + ageFilterId + '-error').removeClass('hidden');
      errorSummary.focus();
      window.document.title = "Error: Search results - GOV.UK";
    }

    return canSubmit;
  },


  clearDateFilterErrors: function(dateFilterId) {
    const errorSummary = $('#js-filter-error-summary');
    const $filter = $('#' + dateFilterId);
    errorSummary.addClass('hidden');
    window.document.title = "Search results - GOV.UK";
    errorSummary.find('.' + dateFilterId + '-error').addClass('hidden');
    $filter.find('.form-group').removeClass('error');
    $filter.find('.error-message').addClass('hidden');
    $filter.find('input').val('');
  },

  clearAgeFilterErrors: function(ageFilterId) {
    const errorSummary = $('#js-filter-error-summary');
    const $filter = $('#' + ageFilterId);
    errorSummary.addClass('hidden');
    errorSummary.find('.' + ageFilterId + '-error').addClass('hidden');
    $filter.find('.date-group').removeClass('error');
    $filter.find('.error-message').addClass('hidden');
    $filter.find('input').val('');
  }

}


export default GiasFilterValidation;
