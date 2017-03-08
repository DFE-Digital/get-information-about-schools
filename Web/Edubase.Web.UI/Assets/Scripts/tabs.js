(function ($) {
    'use strict';
    var defaults = {
        tabClass: 'horizontal-tab',
        selectedTabClass: 'selected-tab',
        tabContentClass: 'tab-content',
        tabContentWrapper: 'tab-content-wrapper'
    };

    
    function Tabs(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    Tabs.prototype = {
        init: function () {
            var $el = $(this.el),
                opts = this.opts,
                $tabs = $el.find('.' + opts.tabClass),
                $tabbedContent = $el.find('.' + opts.tabContentClass),
                validHashes = $.map($tabs, function(elem) {
                    return $(elem).attr('href').split('#')[1];
                }),
                intialTabSelection = $.inArray(window.location.hash, validHashes);

            
            function setTabHeight() {
                var maxHeight = 0;
                $tabs.each(function() {
                    var tabHeight = $(this).outerHeight();
                    if (tabHeight > maxHeight) {
                        maxHeight = tabHeight;
                    }
                });
                return maxHeight;
            }

            if ($(window).width() > 641) {
                $tabs.height(setTabHeight());
            }
            
            if (typeof validHashes === 'undefined' || validHashes.length === 0) {
                return;
            }

            $tabbedContent.attr('tab-index', 0);
            $tabbedContent.addClass('hidden-tab-content').attr('aria-hidden', true);

            $('.tab-manipulator').on('click', function (e) {
                e.preventDefault();
                var hash = $(this).attr('href'),
                    hasTab = $.inArray(hash, validHashes);
                if (hasTab > -1) {
                    $tabs.eq(hasTab).click();
                    $(window).scrollTop($tabs.offset().top);
                }
            });

            $tabs.on('click', function (e) {
                
                e.preventDefault();
                var targetContent = $(this).attr('href');
                location.replace(targetContent);
                
                // undo previous selection
                $tabs.removeClass(opts.selectedTabClass);

                $tabbedContent.addClass('hidden-tab-content')
                    .attr('aria-hidden', true);                

                // new selection
                $(this).addClass(opts.selectedTabClass);

                $(targetContent).removeClass('hidden-tab-content')
                    .attr('aria-hidden', false)
                    .focus();
                
            });
            
            // only process the hash if it's for a valid
            if (window.location.hash && intialTabSelection > -1 && $tabs.filter(opts.selectedTabClass).length === 0) {
                $tabs.eq(intialTabSelection).click();
            } else {
                $tabs.slice(0, 1).addClass(opts.selectedTabClass);
                $tabbedContent.slice(0, 1).removeClass('hidden-tab-content').attr('aria-hidden', false);
            }
        }
    };


    $.fn.tabs = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'tabs')) {
                $.data(this, 'tabs', new Tabs(this, opts));
            }
        });
    }
}($));

