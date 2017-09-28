DfE = window.DfE || {};

DfE.searchResults = (function () {
    var $filterForm = $('#filter-form');
    var $filters = $filterForm.find(':checkbox, select');
    var $clearLinks = $filterForm.find('.filter-clear');
    var $resultsContainer = $('#results-container');
    var $downloadLink = $('.download-link');
    var searchType = null;
    var filterLimit = 200;
    var filterIntent = null;
    var downloadBaseUrl = '/Establishments/Search/PrepareDownload?';
    var $additionalFilters = $('#EditSearchCollapse').find('.additional-search-critera');
    var $additionalFilterClear = $('#additional-filter-wrap').find('.additional-filter-clear');
    var $textFieldFilters = $('#EditSearchCollapse').find('input[type="text"]');
    var $extraFiltersLink = $('#EditSearchCollapse').find('.add-filters-link');
    var optionTemplate = '<div class="filter-wrap"><input type="checkbox" value="#{0}" data-alias="{2}" id="ctrl-{0}" class="additional-filter-type filter-input" /><label  for="ctrl-{0}" class="filter-radio">{1}</label></div>';
    var searchParams = deDupeParams($filterForm.find(':input').filter(function (n, ele) {
        return ele.value !== '';
    }).serialize());

    var plsWait = '<div class="progress-indicator"><span class="visually-hidden">Please wait</span></div>';

    function deDupeParams(qs) {
        var paramArray = qs.split('&');
        return paramArray.sort().filter(function(item, pos, ary) {
            return !pos || item !== ary[pos - 1];
        }).join('&');

    }

    return {
        params: function() {
            return searchParams;
        },
        setupAdditionalFilters: function () {
            var optionsFragment = '';
            var self = this;

            $additionalFilters.each(function (n, elem) {
                var elemId = $(elem).prop('id'),
                    elemText = $(elem).find('.option-select-label').text(),
                    dataAlias = $(elem).data().bindAlias;
                optionsFragment += optionTemplate.replace(/\{0\}/g, elemId).replace('{1}', elemText)
                    .replace('{2}', dataAlias);

            });

            $('#filter-type-target').append(optionsFragment);

            $extraFiltersLink.on('click',
                function (e) {
                    e.preventDefault();
                    $('#additional-filter-wrap').removeClass('hidden');
                    $('#filter-addtional-controls').addClass('hidden');
                    $('#filter-submit').addClass('hidden');
                });

            $('#filter-refine').on('click',
                function (e) {
                    e.preventDefault();
                    $('#additional-filter-wrap').addClass('hidden');
                        $extraFiltersLink.removeClass('hidden');

                    var $selectedFilters = $('#additional-filter-wrap').find('input:checked');

                    $selectedFilters.length === $additionalFilters.length
                        ? $extraFiltersLink.text('Remove addtional filters')
                        : $extraFiltersLink.text('+ Add filters');

                    $additionalFilters.addClass('hidden');

                    var $ele = $('#selected-search-filters');
                    $ele.val("");

                    $selectedFilters.each(function () {
                        var idToShow = $(this).val();
                        $(idToShow).removeClass('hidden');
                        var bindAlias = $(idToShow).data('bind-alias');
                        var specifier = $ele.val() + bindAlias;
                        $ele.val(specifier);
                    });
                    $('#filter-addtional-controls').removeClass('hidden');
                });

            $('#add-filter-cancel').on('click',
                function (e) {
                    e.preventDefault();
                    $('#additional-filter-wrap').addClass('hidden');
                    $('#filter-addtional-controls').removeClass('hidden');
                    $('#filter-submit').removeClass('hidden');
                });

            if (document.getElementById('selected-search-filters')) {
                var aliases = $('#selected-search-filters').val().split('');
                $.each(aliases,
                    function (n, alias) {
                        $('.additional-search-critera[data-bind-alias="' + alias + '"]').removeClass('hidden');
                        $('#filter-type-target').find('[data-alias="' + alias + '"]').prop('checked', true);
                    });
            }

            $additionalFilterClear.on('click',
                function (e) {
                    e.preventDefault();
                    var selectedFilters = $(this)
                        .next('.options-container')
                        .find('input')
                        .filter(function (n, item) {
                            return $(item).prop('checked');
                        });

                    $(selectedFilters).each(function (n, el) {
                        var optionsPanel = $(el.value);
                        $(this).click();
                        optionsPanel.find('input:checked').click();
                    });

                });

            $('#additional-filter-wrap').find('.additional-filter-type').on('change',
                function () {
                    if (!$(this).is(':checked')) {
                        $(this.value).find('input:checked').click();
                    }
                });
            
        },

        setupGovUkSelects: function () {
            $('#EditSearchCollapse').find('.govuk-option-select')
                .map(function () {
                    return new GOVUK.OptionSelect({ $el: $(this) });
                });

            $('#EditSearchCollapse').find('.govuk-option-select')
                .each(function () {
                    if ($(this).find('.js-selected-counter-text').text().length &&
                        !$(this).hasClass('age-filter') &&
                        !$(this).hasClass('date-filters')) {
                        $(this).find('.clear-selections').addClass('active-clear');
                    }
                });

            $clearLinks.on('click',
                function (e) {
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
                            if ($(this).parents('.govuk-option-select').hasClass('nested-filter-options')) {
                                selectedFilters.prop('checked', false);
                                $(this).next('.options-container').find('.filter-radio').removeClass('partial-selection');

                            }
                        }
                    }
                });

            $('.govuk-option-select').on('countUpdated',
                function (e, d) {
                    if (d.selectedCount) {
                        $(this).find('.filter-clear, .additional-filter-clear').addClass('active-clear');
                    } else {
                        $(this).find('.filter-clear, .additional-filter-clear').removeClass('active-clear');
                    }
                });

            
        },
        disableFilters: function () {
            $filters.prop('disabled', true);
            $('#filter-form').find('.active-clear').addClass('clear-disabled');
            $('#filter-form').find('input[type="text"]').prop('disabled', true);
            $('#filter-addtional-controls a').addClass('hidden');
        },
        enableFilters: function () {
            $filters.prop('disabled', false);
            $('#filter-form').find('.active-clear').removeClass('clear-disabled');
            $('#filter-form').find('input[type="text"]').prop('disabled', false);
            $('#filter-addtional-controls a').removeClass('hidden');
        },

        getResults : function() {
            DfE.searchResults.disableFilters();
            

            $resultsContainer.html(plsWait);
            if (GOVUK.support.history()) {
                history.pushState({}, null, window.location.href.split('?')[0] + '?' + searchParams);
            }

            $.ajax({
                url: 'Search/results-js',
                data: searchParams,
                success: function (data, status, xhr) {
                    $resultsContainer.html(data);
                    $downloadLink.removeClass('hidden');

                    $downloadLink.attr('href', downloadBaseUrl + searchParams);
                    $resultsContainer.removeClass('pending-results-update');
                    
                    if (Number(xhr.getResponseHeader("x-count")) === 0) {
                        $downloadLink.addClass('hidden');
                    }

                    if (DfE.searchMap.currentView !== 'map') {
                        DfE.searchResults.enableFilters();
                    }
                    
                },
                error: function(xhr) {
                    DfE.searchResults.enableFilters();
                }
            });
        },

        bindEvents: function () {
            var self = this;
            $(document).on("change", ".trigger-result-update", function () {
                var currentInput = this;
                var chxVal = $(this).val();
                var chxName = $(this).prop('name');
                var isChecked = this.checked;
                var filterCount = $filters.filter(':checked, :selected').length;

                if (filterCount >= filterLimit) {
                    $(this).okCancel({
                        cancel: null,
                        ok: function () {
                            return true;
                        },
                        immediate: true,
                        title: 'Filter limit reached',
                        content:
                            'You\'ve selected too many filters. You can either reduce the number of filters, or download the data and filter it offline.'
                    });
                    $(this).removeData('okCancel');
                    $(this).prop('checked', false);

                } else {

                var similarInput = $('#filter-form').find('.trigger-result-update[name="' + chxName + '"]').filter(function (n, input) {
                if (input.value === chxVal) {
                            return input;
                        }
                    }).not(currentInput);

                    similarInput.prop('checked', isChecked);
                    if (isChecked) {
                        similarInput.parents('.nested-items').find('.filter-group-title').next('label').addClass('partial-selection');
                    } else {

                        var siblingChxCount = similarInput.parents('.filter-group').find('.filter-input').filter(':checked').length;
                        if (siblingChxCount === 0) {
                            similarInput.parents('.nested-items').find('.filter-group-title').next('label').removeClass('partial-selection');
                            similarInput.parents('.nested-items').find('.filter-group-title').prop('checked', false);
                        } else {
                            similarInput.parents('.nested-items').find('.filter-group-title').next('label').addClass('partial-selection');
                        }

                    }

                    if (filterIntent) {
                        window.clearTimeout(filterIntent);
                    }
                    if (DfE.searchMap != null && DfE.searchMap.scriptsLoaded) {
                        DfE.searchMap.clearPoints();
                    }

                    $resultsContainer.addClass('pending-results-update');
                    searchParams = deDupeParams($filterForm.find(':input').filter(function(n, ele) {
                        return ele.value !== '';
                    }).serialize());

                    filterIntent = window.setTimeout(function () {
                        self.getResults();
                        if (DfE.searchMap.currentView === 'map') {
                            DfE.searchMap.getSearchData();
                        } else {
                            DfE.searchMap.dataRefreshRequired = true;
                        }
                        if (searchType === 'ByLocalAuthority') {
                            DfE.searchUtils.updateSearchedLas();
                        }
                        if (searchType === 'Governor') {
                            DfE.searchUtils.updateGovernorRoles();
                        }

                    }, 1500);
                }
            });

            $('.age-filter').find('.filter-button').on('click',
                function (e) {
                    e.preventDefault();
                    DfE.searchUtils.validateAgeFilters();
                });

            $('.date-filters').find('.filter-button').on('click',
                function (e) {
                    e.preventDefault();
                    DfE.searchUtils.validateDateFilters();
                });

            $('#clear-filters').on('click', function (e) {
                e.preventDefault();
                window.clearTimeout(filterIntent);
                $('#filter-form').find('input[type="text"]').val('');
                $('#filter-form').find('.clear-selections').click();
            });
        },

        init: function() {
            searchType = DfE.Util.QueryString.get('searchtype');
            var self = this;
            if (searchType === 'ByLocalAuthority') {
                DfE.searchUtils.updateSearchedLas();
            }


            self.setupGovUkSelects();
            self.setupAdditionalFilters();
            self.bindEvents();
            DfE.searchMap.bindActions();

        }
    }
})();

