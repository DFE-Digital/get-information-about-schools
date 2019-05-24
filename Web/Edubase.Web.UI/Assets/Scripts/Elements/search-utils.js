DfE = window.DfE || {};

DfE.searchUtils = (function () {
    var selectedLas = [];
    var searchType = DfE.Util.QueryString.get('searchtype');
    var errorSummary = $('#js-filter-error-summary');

    return {
        getUrlParam: function (name) {
            var p = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,
                function (m, key, value) {
                    p[key] = value;
                });

            if (name) {
                return p[name];
            }
            return p;
        },
        updateSearchedLas: function () {
            var laCount = 0;
            selectedLas = [];
            $('#option-select-local-authority').find('.trigger-result-update').filter(':checked').each(function () {
                var label = $(this).parent().clone();
                label.find('span, input').remove();
                var text = label.text();
                selectedLas.push('&lsquo;<span class="bold-small">' + $.trim(text) + '</span>&rsquo;');
                laCount++;
            });
            selectedLas = selectedLas.join(', ');
            var lastComma = selectedLas.lastIndexOf(',');
            if (laCount > 1) {
                selectedLas = selectedLas.substring(0, lastComma) +
                    ' and ' +
                    selectedLas.substring(lastComma + 1, selectedLas.length);
            }

            $('#la-list').html(selectedLas);
        },
        updateGovernorRoles: function () {
            var selectedGovRoles = [];
            $('#option-select-role-type').find('.trigger-result-update').filter(':checked').each(function () {
                var label = $(this).parent().clone();
                label.find('span, input').remove();
                var text = label.text();
                selectedGovRoles.push('&lsquo;<span class="bold-small">' + $.trim(text) + '</span>&rsquo;');
            });
            selectedGovRoles = selectedGovRoles.join(', ');
            var lastComma = selectedGovRoles.lastIndexOf(',');
            selectedGovRoles = selectedGovRoles.substring(0, lastComma) +
                ' and ' +
                selectedGovRoles.substring(lastComma + 1, selectedGovRoles.length);
            $('.governor-roles').html(selectedGovRoles);
        },

        validateDateFilters: function (dateFilterId) {
            var canSubmit = true;
            var self = this;
            errorSummary.addClass('hidden');
            window.document.title = "Search results - GOV.UK";
            errorSummary.find('.' + dateFilterId + '-error').addClass('hidden');
            $('#' + dateFilterId).find('.form-group').slice(0, 2).removeClass('error');
            $('#' + dateFilterId).find('.error-message').addClass('hidden');

            var validFromDate = true;
            var validToDate = true;
            var dateObj = {};
            var fromDateFields = $('#' + dateFilterId).find('.search-from-date .form-control');
            var toDateFields = $('#' + dateFilterId).find('.search-to-date .form-control');

            var fromDateValues = $.map(fromDateFields, function (field) {
                if (field.value.trim() !== '') {
                    return field.value;
                }
            });

            var toDateValues = $.map(toDateFields, function (field) {
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
                $('#' + dateFilterId + ' .search-from-date').closest('.form-group').slice(0, 2).addClass('error');
                $('#' + dateFilterId + ' .search-from-date').find('.error-message').not('.date-range-error').removeClass('hidden');
            }

            if (!validToDate) {
                $('#' + dateFilterId + ' .search-to-date').closest('.form-group').slice(0, 2).addClass('error');
                $('#' + dateFilterId + ' .search-to-date').find('.error-message').not('.date-range-error').removeClass('hidden');
            }

            return canSubmit;
        },
        clearDateFilterErrors: function(dateFilterId){
            errorSummary.addClass('hidden');
            window.document.title = "Search results - GOV.UK";
            errorSummary.find('.' + dateFilterId + '-error').addClass('hidden');
            $('#' + dateFilterId).find('.form-group').removeClass('error');
            $('#' + dateFilterId).find('.error-message').addClass('hidden');
            $('#' + dateFilterId).find('input').val('');
        },

        validateDate: function (dateObj) {
            var day = dateObj.day,
                month = dateObj.month,
                year = dateObj.year,

                dateError = false,
                months31 = [0, 2, 4, 6, 7, 9, 11];

            if (isNaN(day) || isNaN(month) || isNaN(year)) {
                dateError = true;
            }

            var isLeap = new Date(year, 1, 29).getMonth() === 1;

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
        validateAgeFilters: function (ageFilterId) {
            var canSubmit = true;
            errorSummary.addClass('hidden');
            errorSummary.find('.summary-range-error').addClass('hidden');
            $('#' + ageFilterId).find('.date-group').removeClass('error');
            $('#' + ageFilterId).find('.error-message').addClass('hidden');

            var fields = $('#' + ageFilterId).find('.form-control');
            var valid = true;
            $.each(fields,
                function (m, field) {
                    if (field.value !== '' && isNaN(field.value)) {
                        valid = false;
                        canSubmit = false;
                    }
                });

            if (!valid) {
                $('#' + ageFilterId).find('.date-group').addClass('error');
                $('#' + ageFilterId).find('.error-message').removeClass('hidden');
                errorSummary.removeClass('hidden');
                errorSummary.find('.' + ageFilterId + '-error').removeClass('hidden');
                errorSummary.focus();
                window.document.title = "Error: Search results - GOV.UK";
            }

            return canSubmit;
        },
        clearAgeFilterErrors: function(ageFilterId){
            errorSummary.addClass('hidden');
            errorSummary.find('.' + ageFilterId + '-error').addClass('hidden');
            $('#' + ageFilterId).find('.date-group').removeClass('error');
            $('#' + ageFilterId).find('.error-message').addClass('hidden');
            $('#' + ageFilterId).find('input').val('');
        },
        validateRadiusFilter: function () {
            var filters = $('#EditSearchCollapse').find('.radius-filter');
            var canSubmit = true;
            errorSummary.addClass('hidden');
            errorSummary.find('.summary-radius-error').addClass('hidden');
            filters.each(function (n, elem) {
                $(elem).find('.form-group').removeClass('error');
                $(elem).find('.error-message').addClass('hidden');

                var fields = $(elem).find('.form-control');
                var valid = true;
                $.each(fields,
                    function (m, field) {
                        if (field.value.substr(0, 1) == ".") {
                            field.value = "0" + field.value;
                        }

                        if (!field.value.match(/^[0-9]{1,2}(\.[0-9]{1,2})?$/)) {
                            valid = false;
                            canSubmit = false;
                        }
                    });

                if (!valid) {
                    $(elem).find('.form-group').addClass('error');
                    $(elem).find('.error-message').removeClass('hidden');
                    errorSummary.removeClass('hidden');
                    errorSummary.find('.summary-radius-error').removeClass('hidden');
                    return;
                }

                if (n + 1 === filters.length && canSubmit) {

                    if (DfE.searchMap.currentView === 'map') {
                        DfE.searchResults.setSearchParams();
                        DfE.searchMap.getSearchData();
                    } else {
                        DfE.searchMap.dataRefreshRequired = true;
                        DfE.searchResults.setSearchParams();
                        DfE.searchResults.getResults();
                    }

                }
            });
        }
    }
}());
