
(function ($) {
    'use strict';
    var defaults = {
        radioClass: '.panel-toggle',
        panelClass: '.expanding-search-field',
        openPanelClass: 'selected-search'
    }


    function ExpandingSearchForm(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    ExpandingSearchForm.prototype = {
        init: function () {
            var $el = $(this.el),
                opts = this.opts,
                $radios = $el.find(opts.radioClass),
                $labels = $radios.parent();

            //start with a clean slate
            $radios.prop('checked', false);
            $el.find(opts.panelClass).attr('aria-hidden', true);
            

            /**
             * @private
             * @param {} $elem 
             * @returns {} 
             */
            function openPanel($elem) {
                $elem.css({ display: 'block' })
                    .attr('aria-hidden', false);

                $elem.find('.form-control').slice(0, 1).focus();
            }

            // attach events
            $radios.on('change', function () {
                $el.find(opts.panelClass)
                    .css('display', 'none')
                    .attr('aria-hidden', false);

                $radios.not(this).prop('checked', false);
                $labels.not($(this).parent()).removeClass(opts.openPanelClass);
                $el.find(opts.panelClass).attr('aria-hidden', true);


                openPanel($(this).parent().siblings(opts.panelClass));
                $(this).parent().addClass(opts.openPanelClass);

            });

        }
    }

    $.fn.searchForm = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'searchForm')) {
                $.data(this, 'searchForm', new ExpandingSearchForm(this, opts));
            }
        });
    }

}($));


DfE.Views.schoolSearch = $('#content').find('.search-type-list');

if (DfE.Views.schoolSearch.length) {
    DfE.Views.schoolSearch.searchForm();
}

