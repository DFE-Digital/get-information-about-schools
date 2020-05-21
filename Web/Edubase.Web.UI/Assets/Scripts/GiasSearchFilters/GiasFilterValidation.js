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
    const day = dateObj.day;
    const month = dateObj.month;
    const year = dateObj.year;

    let dateError = false;
    const months31 = [0, 2, 4, 6, 7, 9, 11];

    if (isNaN(day) || isNaN(month) || isNaN(year)) {
      dateError = true;
    }

    const isLeap = new Date(year, 1, 29).getMonth() === 1;

    if (isLeap && month === 1) {
      if (day > 29) {
        dateError = true;
      }
    } else if (month === 1) {
      if (day > 28) {
        dateError = true;
      }
    }

    if (months31.indexOf(month - 1)) {
      if (day < 1 || day > 31) {
        dateError = true;
      }
    } else {
      if (day < 1 || day > 30) {
        dateError = true;
      }
    }

    if (month < 0 || month > 11) {
      dateError = true;
    }

    return dateError;
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
    $dateFilter.find('.error-message').addClass('hidden');

    console.log('validating date!');

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
      dateObj.month = fromDateValues[1] - 1;
      dateObj.year = fromDateValues[2];

      validFromDate = !self.validateDate(dateObj);
    }

    if (toDateValues.length === 3 && validToDate) {
      dateObj.day = toDateValues[0];
      dateObj.month = toDateValues[1] - 1;
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

    if (!validFromDate) {
     // $('#' + dateFilterId + ' .search-from-date').closest('.govuk-form-group').slice(0, 2).addClass('govuk-form-group--errorerror');
      $('#' + dateFilterId + ' .search-from-date').find('.govuk-error-message').not('.date-range-error').removeClass('hidden');
    }

    if (!validToDate) {
      //$('#' + dateFilterId + ' .search-to-date').closest('.govuk-form-group').slice(0, 2).addClass('govuk-form-group--error');
      $('#' + dateFilterId + ' .search-to-date').find('.govuk-error-message').not('.date-range-error').removeClass('hidden');
    }

    return canSubmit;
  },

  validateAgeFilter: function(ageFilterId) {
    console.log('validate!!');
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
     // $ageFilter.find('.date-group').addClass('govuk-form-group--error');
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
