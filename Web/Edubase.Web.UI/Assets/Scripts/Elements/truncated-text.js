(function($) {
    'use strict';
    var defaults = {
        maxCharacters: 40,
        minScreenWidth: '640'
    };

    function TruncateText(el, options) {
        this.el = el;
        this.opts = $.extend({}, defaults, options);

        this.init();
    }

    TruncateText.prototype = {
        init :function() {
            var $el = $(this.el),
                opts = this.opts,
                self = this,
                tooltip = '<div id="edubase-tooltip" class="edubase-tooltip truncated-tooltip hidden" aria-hidden="true">' +
                    '<span class="pointer-shadow"></span><span class="pointer"></span>' +
                    '<div id="tooltip-content" class="tooltip-content" tabindex="-1"></div></div>',

                showTooltip = function (text) {
                    if (!$('#edubase-tooltip').hasClass('hidden')) {
                        $(window).trigger('tooltipShift');
                    }

                    $('#tooltip-content').text(text);
                    $('#edubase-tooltip')
                        .removeClass('hidden')
                        .css({
                            display: 'block',
                            left: '-1000em'
                        });

                    var width = $('#edubase-tooltip').width() + 40;


                    $('#edubase-tooltip').css({
                        display: 'block',
                        top: $el.offset().top + $el.height() + 12 + 'px',
                        left: $el.offset().left + $el.width() - width + 'px'

                    });

                },
                hideTooltip = function () {
                    $('#edubase-tooltip').addClass('hidden');
                },

                originalText = $.trim($el.text());

            this.clicks = 0;

            if ($(window).width() < opts.minScreenWidth) {
                return;
            }

            if (!document.getElementById('edubase-tooltip')) {
                $(document.body).append(tooltip);
            }

            if (originalText.length <= opts.maxCharacters) {
                return;
            } else {
                $el.originalText = originalText;
                $el.addClass('truncated-content');
                $el.text(originalText.substring(0, opts.maxCharacters) + '...');
            }

            $(window).on('tooltipShift', function() {
                self.clicks = 0;
            });

            $el.on('click', function () {
                if (self.clicks % 2 === 0) {
                    showTooltip($el.originalText);
                    self.clicks ++;
                } else {
                    hideTooltip();
                    self.clicks = 0;
                }
            });

            $('.tooltip-close').on('click', function() {
                hideTooltip();
            });
        },
        resetClickCount: function() {
            this.clicks = 0;
        }
    };

    $.fn.truncate = function(options) {
        return this.each(function() {
            if (!$.data(this, 'truncate')) {
                $.data(this, 'truncate', new TruncateText(this, options));
            }
        });
    }

}($));

$('.truncate').truncate();
