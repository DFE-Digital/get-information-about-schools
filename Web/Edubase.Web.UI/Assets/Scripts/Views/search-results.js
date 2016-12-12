

// set up search filters and associated interactions
(function() {
    var $clearLinks = $('#EditSearchCollapse').find('.clear-selections');

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