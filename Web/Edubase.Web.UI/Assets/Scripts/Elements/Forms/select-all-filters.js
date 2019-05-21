(function($) {

    var defaults = {
    };

    function SelectAll(el, opts) {
        this.el = el;
        this.opts = $.merge({}, defaults, opts);

        this.init();
    }

    SelectAll.prototype = {
        init: function() {
            var $el = $(this.el);
            var $childOptions = $el.parents('fieldset').find('.trigger-result-update');
            var self = this;
            this.childOptionCount = $childOptions.length;

            $el.on('change', function() {
                if (this.checked) {
                    if ($el.parents('.govuk-option-select').hasClass('nested-filter-options')) {
                        $el.parents('fieldset').find('.filter-group-title').prop('checked', true);
                    }
                    $childOptions.prop('checked', true);

                } else {
                    if ($el.parents('.govuk-option-select').hasClass('nested-filter-options')) {
                        $el.parents('fieldset').find('.filter-group-title').prop('checked', false);
                    }
                    $childOptions.prop('checked', false);
                }
                $childOptions.slice(0, 1).trigger('change');
            });

            $childOptions.on('change', function() {
                var count = $childOptions.filter(function(n, input) {
                    return input.checked;
                }).length;

                if (count === self.childOptionCount || count === 0) {
                    $el.next('.js-filter-label').removeClass('partial-selection');
                } else {
                    $el.next('.js-filter-label').addClass('partial-selection');
                }
            });
        }
    };


    $.fn.selectAll = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'selectAll')) {
                $.data(this, 'selectAll', new SelectAll(this, opts));
            }
        });
    }

}($));

$('.select-all').selectAll();
