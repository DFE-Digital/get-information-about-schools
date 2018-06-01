(function($) {

    var defaults = {
        parentControl: '.filter-group-title',
        expander: '.child-option-toggle',
        childItems: '.filter-group'
    };

    function NestedFilters(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);
        this.init();
    }

    NestedFilters.prototype = {
        init: function() {
            var $el = $(this.el), opts = this.opts;

            var parent = $el.find(opts.parentControl),
                trigger = $el.find(opts.expander),
                optionsPanel = $el.find('.filter-group'),
                childControls = optionsPanel.find('input'),
                checkedChildren = childControls.filter(function(n, elem) {
                    return elem.checked;
                });

            if (checkedChildren.length > 0) {
                parent.prop('checked', true);


                if (checkedChildren.length < childControls.length) {
                    parent.next('label').addClass('partial-selection');
                }
            }
            optionsPanel.addClass('hidden');

            trigger.on('click', function(e) {
                e.preventDefault();
                $(this).toggleClass('open-children');
                optionsPanel.toggleClass('hidden');
            });

            parent.on('change', function (e) {
                var optSelect = $(this).parents('.govuk-option-select');
                optSelect.find('.clear-selections').removeClass('active-clear');

                $(this).next('label').removeClass('partial-selection');

                if (this.checked) {
                    childControls.prop('checked', true);
                } else {
                    childControls.prop('checked', false);
                }
                childControls.change();

                var buf = [];
                optSelect.find('.trigger-result-update').filter(function (n, elem) {
                    return elem.checked;
                }).each(function (n, elem) {
                    if ($.inArray(elem.value, buf) === -1) {
                        buf.push(elem.value);
                    }
                    });

                var count = buf.length;

   
                var checkedString = '';
                if (count > 0) {
                    checkedString = count + ' selected';
                    optSelect.find('.clear-selections').addClass('active-clear');
                }

                window.setTimeout(function () {
                    optSelect.find('.js-selected-counter-text').text(checkedString);
                }, 0);
            });

            childControls.on('change', function(e) {
                var checkedCount = childControls.filter(function (n,ctrl) {
                    return ctrl.checked;
                }).length;

                if (checkedCount < childControls.length && checkedCount > 0) {
                    parent.next('label').addClass('partial-selection');
                    parent.prop('checked', true);
                } else {
                    parent.next('label').removeClass('partial-selection');
                    if (checkedCount === 0) {
                        parent.prop('checked', false);

                    }
                }
            });

        },
        //Set visual state of nested filters without triggering a result update
        setPartialState: function() { 
            var $el = $(this.el), opts = this.opts;
            var childControls = $el.find('.filter-group input');
            var parent = $el.find(opts.parentControl);
            var checkedChildren = childControls.filter(function (n, elem) {
                return elem.checked;
            });

            parent.next('label').removeClass('partial-selection');
           
            if (checkedChildren.length === 0) {
                parent.prop('checked', false);
            } else if (checkedChildren.length < childControls.length) {
                parent.prop('checked', true);
                parent.next('label').addClass('partial-selection');

            }
        }
    };

    $.fn.nestedFilters = function(opts) {
        return this.each(function() {
            if (!$.data(this, 'nestedFilters')) {
                $.data(this, 'nestedFilters', new NestedFilters(this, opts));
            }
        });
    }


}($));

if (document.getElementById('EditSearchCollapse')) {
    $('#EditSearchCollapse').find('.nested-items').nestedFilters();
}
