DfE = window.DfE || {};

DfE.searchUtils = (function () {
    var selectedLas = [];
    var searchType = DfE.Util.QueryString.get('searchtype');
    var errorSummary = $('#js-filter-error-summary');
        
    return {
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

        validateDateFilters: function () {
            var filters = $('#EditSearchCollapse').find('.date-filters').filter(':visible');
            var canSubmit = true;
            var self = this;
            errorSummary.addClass('hidden');
            errorSummary.find('.summary-date-error').addClass('hidden');
            filters.each(function(n, elem) {
                $(elem).find('.form-group').slice(0, 2).removeClass('error');
                $(elem).find('.error-message').addClass('hidden');

                var validDate = true;
                var dateObj = {};
                var fromDateFields = $(elem).find('.search-from-date .form-control');
                var toDateFields = $(elem).find('.search-to-date .form-control');

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
                    validDate = false;
                }
                if (toDateValues.length > 0 && toDateValues.length < 3) {
                    validDate = false;
                }


                if (fromDateValues.length === 3 && validDate) {
                    dateObj.day = fromDateValues[0];
                    dateObj.month = fromDateValues[1] - 1;
                    dateObj.year = fromDateValues[2];

                    validDate = !self.validateDate(dateObj);
                }

                if (toDateValues.length === 3 && validDate) {
                    dateObj.day = toDateValues[0];
                    dateObj.month = toDateValues[1] - 1;
                    dateObj.year = toDateValues[2];

                    validDate = !self.validateDate(dateObj);
                }

                if (!validDate) {
                    $(elem).find('.form-group').slice(0, 2).addClass('error');
                    $(elem).find('.error-message').not('.date-range-error').removeClass('hidden');
                    canSubmit = false;
                    errorSummary.removeClass('hidden');
                    errorSummary.find('.summary-date-error').removeClass('hidden');
                }

                if (n + 1 === filters.length && canSubmit) {
                    if (DfE.searchMap.currentView === 'map') {
                        DfE.searchMap.getSearchData();
                    } else {
                        DfE.searchMap.dataRefreshRequired = true;
                        
                        DfE.searchResults.setSearchParams();
                        DfE.searchResults.getResults();
                    }
                    
                } 
            });
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
        validateAgeFilters: function () {
            var filters = $('#EditSearchCollapse').find('.age-filter').filter(':visible');
            var canSubmit = true;
            errorSummary.addClass('hidden');
            errorSummary.find('.summary-range-error').addClass('hidden');
            filters.each(function (n, elem) {
                $(elem).find('.date-group').removeClass('error');
                $(elem).find('.error-message').addClass('hidden');

                var fields = $(elem).find('.form-control');
                var valid = true;
                $.each(fields,
                    function (m, field) {
                        if (field.value !== '' && isNaN(field.value)) {
                            valid = false;
                            canSubmit = false;
                        }
                    });

                if (!valid) {
                    $(elem).find('.date-group').addClass('error');
                    $(elem).find('.error-message').removeClass('hidden');
                    errorSummary.removeClass('hidden');
                    errorSummary.find('.summary-range-error').removeClass('hidden');
                    return;
                }

                if (n + 1 === filters.length && canSubmit) {                    
                    
                    if (DfE.searchMap.currentView === 'map') {
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
