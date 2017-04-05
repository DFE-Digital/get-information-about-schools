(function($) {
    var defaults = {
        contentTarget: '#tooltip-content',

    }

    function ToolTip(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    function tooltipScaffold() {
        var tooltip = '<div id="edubase-tooltip" class="edubase-tooltip hidden" aria-hidden="true"><span class="pointer"></span>' +
            '<a href="#" class="tooltip-close" title="close tool tip"><span class="visuallyhidden">Close</span></a>' +
            '<div id="tooltip-content" class="tooltip-content" tab-index="0"></div></div>';

        $('#content').append(tooltip);
    }

    ToolTip.prototype = {
        init: function () {
           tooltipScaffold();
           var el = this.el,
               opts = this.opts,            
               $toolTip = $('#edubase-tooltip');

            $(el).on('click', function(e) {
                e.preventDefault();
                var contentSelector = $(this).attr('href');
                var content = $(contentSelector).html();
                var offSet = $(this).offset();

                $(opts.contentTarget).html(content);

                $toolTip.css({ top: offSet.top + 45 + 'px', left: offSet.left - 26 + 'px' });

                $toolTip.removeClass('hidden')
                    .attr('aria-hidden', false);
                $('#tooltip-content').focus();
            });

            $toolTip.find('.tooltip-close').on('click', function(e) {
                e.preventDefault();
                $toolTip.addClass('hidden')
                .attr('aria-hidden', true);
            });

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

$('#content').find('.icon-help').tooltip();