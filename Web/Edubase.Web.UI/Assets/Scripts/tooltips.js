(function($) {
    var defaults = {
        contentTarget: '#tooltip-content'

    }

    function ToolTip(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    function tooltipScaffold() {
        var tooltip = '<div id="edubase-tooltip" class="edubase-tooltip hidden" aria-hidden="true">' +
            '<span class="pointer-shadow"></span><span class="pointer"></span>' +
            '<div id="tooltip-content" class="tooltip-content" tabindex="0"></div>' +
            '<a href="#" class="tooltip-close" title="close tool tip"><span class="visuallyhidden">Close</span></a></div>';

        $('#full-content').after(tooltip);
    }

    ToolTip.prototype = {
        init: function () {
            if (!document.getElementById('edubase-tooltip')) {
                tooltipScaffold();
            }

            var el = this.el,
                opts = this.opts,
                self = this;

           this.$toolTip = $('#edubase-tooltip');

           var closeToolTip = function closeToolTip() {
                self.$toolTip.addClass('hidden')
                    .attr('aria-hidden', true);

                // Focus on the tooltip link after closing the tooltip window
                // so that keyboard users can carry on from where they left off
                $('.tooltip-open').focus();

                // Clearing all open tooltip links
                $('.tooltip-link').removeClass('tooltip-open');

                $('body').off('click', '#full-content', closeToolTip);
           }

            $(el).on('click', function(e) {
                // Clearing all open tooltip links
                $('.tooltip-link').removeClass('tooltip-open');

                // Marking the selected tooltip link to focus on when exiting
                $(el).addClass('tooltip-open');
                
                e.preventDefault();
                e.stopPropagation();
                var contentSelector = $(this).attr('href');
                var content = $(contentSelector).html();

                $(opts.contentTarget).html(content);

                self.positionToolTip();

                self.$toolTip.removeClass('hidden')
                    .attr('aria-hidden', false);

                // Focus on the tooltip after clicking so that
                // it'll get read out by screen readers
                $('#tooltip-content').focus();

                window.setTimeout(function() {
                    $('body').on('click', '#full-content', closeToolTip);
                }, 20);

            });

            this.$toolTip.find('.tooltip-close').on('click', function(e) {
                e.preventDefault();
                closeToolTip();

            });


        },
        positionToolTip: function() {
            var el = this.el,
                opts = this.opts,
                self = this,
                winWidth = $(window).width(),
                offset = $(el).offset(),
                toolTipWidth = (winWidth < 480) ? 310: 480,
                leftPos ;


            if (offset.left + toolTipWidth > winWidth) {
                leftPos = winWidth - toolTipWidth - 10;

                self.$toolTip.css({ top: offset.top + 40 + 'px', left: leftPos + 'px' });

                self.$toolTip.find('.pointer, .pointer-shadow')
                    .css({
                        left: -1 * (parseInt(self.$toolTip.css('left'), 10) - offset.left + 7) + 'px'
                    });

            } else {
                self.$toolTip.css({ top: offset.top + 40 + 'px', left: offset.left - 26 + 'px' });

                self.$toolTip.find('.pointer, .pointer-shadow').removeAttr('style');

            }
        }
    }

    $.fn.tooltip = function(opts) {
        return this.each(function() {
            if (!$.data(this, 'toolTip')) {
                $.data(this, 'toolTip', new ToolTip(this, opts));
            }
        });
    }

}($));

$('#content').find('.js-tooltip').tooltip();
