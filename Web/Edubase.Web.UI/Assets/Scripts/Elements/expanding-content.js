/**
 * utility for toggling content
 * from a link click
 *
 *  - assumes content is initially hidden (css)
 *  - by default will initialise on '.expanding-content-wrapper'
 *
 *  - basic HTML structure -
 *
 *  <div class='expanding-content-wrapper'>
 *      <a class='expanding-content-trigger'>Open something</a>
 *      <div class='expanding-content'>
 *          ~ HTML ~
 *      </div>
 *  </div>
 *
 */
(function ($) {
    'use strict';
    /**
     * default config
     */
    var defaults = {
        triggerElem: '.expanding-content-trigger',
        contentElem: '.expanding-content',
        openedText: 'Hide details',
        closedText: 'Show details',
        updateText: true
    };
    /**
     * @constructor
     * @param {} el - the container for the expandable component
     * @param {} opts - user instance options
     * @returns {}
     */
    function Expandable(el, opts) {
        this.el = el;
        this.defaults = defaults;
        this.metaData = $(el).data();
        this.opts = $.extend({}, defaults, this.metaData, opts);

        this.init();
    }

    Expandable.prototype = {
        /**
         * initialistaion
         * @public
         * @returns {}
         */
        init: function () {
            var $el = $(this.el),
                opts = this.opts,
                $expander = $el.find(opts.contentElem),
                $trigger = $el.find(opts.triggerElem),
                $contentArea = $el.find(opts.contentElem),
                self = this;

            // no js - content is expanded, aria requires set up
            $contentArea.css({ display: 'none' }).attr('aria-hidden', true);
            // make the first child element of the content focusable
            $contentArea.children().slice(0, 1).attr('tabindex', 0);
            $trigger.css({ display: 'inline-block' });

            $trigger.on('click', function (e) {
                e.preventDefault();
                if ($expander.hasClass('exp-opened')) {
                    self.close();
                    if (opts.updateText) {
                        $trigger.text(opts.closedText);
                    }

                } else {
                    self.open();
                    if (opts.updateText) {
                        $trigger.text(opts.openedText);
                    }

                }
                $(this).toggleClass('trigger-open').blur();
            });
        },
        /**
         * opens the expandable content
         * @public
         */
        open: function () {
            $(this.el)
                .find(this.opts.contentElem)
                .attr('aria-hidden', false)
                .css('display', 'block')
                .addClass('exp-opened');

            $(this.el).children().slice(0, 1).focus();
        },
        /**
         * closes expandable content
         * @public
         */
        close: function () {
            $(this.el).find(this.opts.contentElem)
                .attr('aria-hidden', true)
                .css('display', 'none')
                .removeClass('exp-opened');
        }
    };

    // ensure that our expanding components are only bound once
    $.fn.expandable = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'expandable')) {
                $.data(this, 'expandable', new Expandable(this, opts));
            }
        });
    }

}($));

var $expanders = $('#content').find('.expanding-content-wrapper');
if ($expanders.length) {
    $expanders.expandable();
}
