

// set up search filters and associated interactions
(function() {
    var $clearLinks = $('#EditSearchCollapse').find('.clear-selections'),
        $additionalFilters = $('#EditSearchCollapse').find('.additional-search-critera'),
        $extraFiltersLink = $('#EditSearchCollapse').find('.add-filters-link'),
        optionTemplate = '<label><input type="checkbox" value="#{0}" />{1}</label>',
        optionsFragment = '';
        
       
        $additionalFilters.each(function (n, elem) {
            var elemId = $(elem).prop('id'), elemText = $(elem).find('.option-select-label').text();
            optionsFragment += optionTemplate.replace('{0}', elemId).replace('{1}', elemText);

        });

    $('#filter-type-target').append(optionsFragment);



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

        var $selectedFilters = $('#additional-filter-wrap').find('input:checked');

        $selectedFilters.length === $additionalFilters.length
            ? $extraFiltersLink.text('Remove addtional filters')
            : $extraFiltersLink.text('Add filters');

        $additionalFilters.addClass('hidden');

        var $ele = $('#selected-search-filters');
        $ele.val("");

        $selectedFilters.each(function () {
            var idToShow = $(this).val();
            $(idToShow).removeClass('hidden');
            var bindAlias = $(idToShow).data('bind-alias'); // bind-alias is a one-letter specifier that the server-side uses to know which extra filters were selected, and can open them on the next page load.
            var specifier = $ele.val() + bindAlias;
            $ele.val(specifier); 
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