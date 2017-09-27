

// set up search filters and associated interactions
(function() {
    var $clearLinks = $('#EditSearchCollapse').find('.filter-clear'),
        $additionalFilters = $('#EditSearchCollapse').find('.additional-search-critera'),
        $additionalFilterClear = $('#additional-filter-wrap').find('.additional-filter-clear'),
        $textFieldFilters = $('#EditSearchCollapse').find('input[type="text"]'),
        $extraFiltersLink = $('#EditSearchCollapse').find('.add-filters-link'),
        optionTemplate = '<div class="filter-wrap"><input type="checkbox" value="#{0}" data-alias="{2}" id="ctrl-{0}" class="additional-filter-type filter-input" /><label  for="ctrl-{0}" class="filter-radio">{1}</label></div>',
        optionsFragment = '';
        
       
        $additionalFilters.each(function (n, elem) {
            var elemId = $(elem).prop('id'),
                elemText = $(elem).find('.option-select-label').text(),
                dataAlias = $(elem).data().bindAlias;
            optionsFragment += optionTemplate.replace(/\{0\}/g, elemId).replace('{1}', elemText).replace('{2}', dataAlias);

        });

    $('#filter-type-target').append(optionsFragment);

    $textFieldFilters.on('change', function () {
        var $govUkSelect = $(this).parents('.govuk-option-select');
        var siblings = $govUkSelect.find('input');
        var hasValues = siblings.filter(function(n, field) {
                if (field.value.trim() !== '') {
                    return field.value;
                }
        }).length > 0;

        if (hasValues) {
            $govUkSelect.find('.clear-selections').addClass('active-clear');
        } else {
            $govUkSelect.find('.clear-selections').removeClass('active-clear');
        }
    });

    $extraFiltersLink.on('click', function(e) {
        e.preventDefault();
        $('#additional-filter-wrap').removeClass('hidden');
        $('#filter-addtional-controls').addClass('hidden');
        $('#filter-submit').addClass('hidden');
    });

    $('#filter-refine').on('click', function(e) {
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
            var bindAlias = $(idToShow).data('bind-alias'); // bind-alias is a one-letter specifier that the server-side uses to know which extra filters were selected, and can open them on the next page load.
            var specifier = $ele.val() + bindAlias;
            $ele.val(specifier); 
        });
        $('#filter-addtional-controls').removeClass('hidden');
    });

    $('#add-filter-cancel').on('click', function(e) {
        e.preventDefault();
        $('#additional-filter-wrap').addClass('hidden');
        $('#filter-addtional-controls').removeClass('hidden');
        $('#filter-submit').removeClass('hidden');
    });


    $('.govuk-option-select')
        .map(function() {
            return new GOVUK.OptionSelect({ $el: $(this) });
        });

    $('.govuk-option-select')
        .each(function() {
            if ($(this).find('.js-selected-counter-text').text().length && !$(this).hasClass('age-filter') && !$(this).hasClass('date-filters')) {
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

    $additionalFilterClear.on('click', function(e) {
        e.preventDefault();
        var selectedFilters = $(this)
            .next('.options-container')
            .find('input')
            .filter(function (n, item) {
                return $(item).prop('checked');
            });

        $(selectedFilters).each(function(n, el) {
            var optionsPanel = $(el.value);
            $(this).click();
            optionsPanel.find('input:checked').click();
        });

    });

    $('#additional-filter-wrap').find('.additional-filter-type').on('change',
        function() {
            if (!$(this).is(':checked')) {
                $(this.value).find('input:checked').click();
            }
        });

    $('.govuk-option-select')
        .on('countUpdated',
            function(e, d) {
                if (d.selectedCount) {
                    $(this).find('.filter-clear, .additional-filter-clear').addClass('active-clear');
                } else {
                    $(this).find('.filter-clear, .additional-filter-clear').removeClass('active-clear');
                }
            });

    if (document.getElementById('selected-search-filters')) {
        var aliases = $('#selected-search-filters').val().split('');
        $.each(aliases,
            function(n, alias) {
                $('.additional-search-critera[data-bind-alias="' + alias + '"]').removeClass('hidden');
                $('#filter-type-target').find('[data-alias="' + alias + '"]').prop('checked', true);
            });
    }
   


}());