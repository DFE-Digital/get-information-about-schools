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
                tooltip = '<div id="edubase-tooltip" class="edubase-tooltip truncated-tooltip" aria-hidden="true">' +
                    '<span class="pointer-shadow"></span><span class="pointer"></span>' +
                    '<a href="#" class="tooltip-close" title="close tool tip"><span class="visuallyhidden">Close</span></a>' +
                    '<div id="tooltip-content" class="tooltip-content" tab-index="-1"></div></div>',
                
                showTooltip = function (text) {
                    
                    $('#tooltip-content').text(text);
                    $('#edubase-tooltip').css({
                        display: 'block',
                        left: '-1000em'
                    });

                    var width = $('#edubase-tooltip').width() + 20;


                    $('#edubase-tooltip').css({
                        display: 'block',
                        top: $el.offset().top + $el.height() + 12 + 'px',
                        left: $el.offset().left + $el.width() - width + 'px'

                    });

                },
                hideTooltip = function () {
                    $('#edubase-tooltip').css({ display: 'none' });
                },

                originalText = $.trim($el.text());

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


            $el.on('click', function () {
                showTooltip($el.originalText);
            });

            $('.tooltip-close').on('click', function() {
                hideTooltip();
            });

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