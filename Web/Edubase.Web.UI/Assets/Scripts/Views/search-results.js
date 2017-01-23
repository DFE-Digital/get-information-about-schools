

// set up search filters and associated interactions
(function() {
    var $clearLinks = $('#EditSearchCollapse').find('.clear-selections'),
        $additionalFilters = $('#EditSearchCollapse').find('.additional-search-critera'),
        $extraFiltersLink = $('#EditSearchCollapse').find('.add-filters-link'),
        filterSelector = '<div id="additional-filter-wrap" class="hidden"><div class="govuk-option-select js-collapsible" data-closed-on-load="false" id="option-select-additional-filters">' +
                         '<button class="js-container-head" aria-expanded="true">' +
                         '<div class="option-select-label">Add more filters</div>' +
                         '<div class="js-selected-counter"></div></button>'+
                         '<a href="#" class="clear-selections">Clear</a>'+
                         '<div class="options-container">'+
                         '<div class="js-auto-height-inner"><fieldset>'+
                         '<legend class="visuallyhidden">Select an option below to filter the results</legend>';

    $additionalFilters.each(function(n, elem) {
       filterSelector += '<label><input type="checkbox" value="#' + $(elem).prop('id') +'" />' + $(elem).find('.option-select-label').text()+'</label>';
    });


    filterSelector += '</fieldset></div></div></div><input value="Add filters" class="button edubase-filter-submit" type="submit" id="filter-refine"></div>';

    $('#EditSearchCollapse').append($(filterSelector));

    $extraFiltersLink.on('click', function(e) {
        e.preventDefault();
        $('#additional-filter-wrap').removeClass('hidden');
        $extraFiltersLink.addClass('hidden');
        $('#filter-submit').addClass('hidden');
    });

    $('#filter-refine').on('click', function(e) {
        e.preventDefault();
        $('#additional-filter-wrap').addClass('hidden');
        $extraFiltersLink.removeClass('hidden');
        $('#filter-submit').removeClass('hidden');

        var $selectedFilters = $('#additional-filter-wrap').find('input:checked');

        $additionalFilters.addClass('hidden');
        $selectedFilters.each(function () {
            var idToShow = $(this).val();
            $(idToShow).removeClass('hidden');
        });

    });


    $('.govuk-option-select')
        .map(function() {
            return new GOVUK.OptionSelect({ $el: $(this) });
        });

    $('.govuk-option-select')
        .each(function() {
            if ($(this).find('.js-selected-counter').text().length) {
                $(this).find('.clear-selections').addClass('active-clear');
            }
        });

    $clearLinks.on('click',
        function(e) {
            e.preventDefault();
            var selectedFilters = $(this)
                .next('.options-container')
                .find('input')
                .filter(function(n, item) {
                    return $(item).prop('checked');
                })
                .click();
            $(this).removeClass('active-clear');
        });

    $('.govuk-option-select')
        .on('countUpdated',
            function(e, d) {
                if (d.selectedCount) {
                    $(this).find('.clear-selections').addClass('active-clear');
                } else {
                    $(this).find('.clear-selections').removeClass('active-clear');
                }
            });
}());