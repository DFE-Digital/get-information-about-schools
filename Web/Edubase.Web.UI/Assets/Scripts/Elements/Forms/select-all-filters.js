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
                   $childOptions.prop('checked', true);
               } else {
                   $childOptions.prop('checked', false);
                }
                $childOptions.trigger('change');
            });

            $childOptions.on('change', function() {
                var count = $childOptions.filter(function(n, input) {
                    return input.checked;
                }).length;

                if (count === self.childOptionCount || count === 0) {
                    $el.next('.filter-radio').removeClass('partial-selection');
                } else {
                    $el.next('.filter-radio').addClass('partial-selection');
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