﻿(function ($) {
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

                validHashes = $.map($tabs, function (elem) {
                    if ($(elem).attr('href').indexOf('#') === 0) {
                        return $(elem).attr('href').split('#')[1];
                    }                    
                }),

            intialTabSelection = $.inArray(window.location.hash.replace('#', ''), validHashes);
            
            

            $el.addClass('tab-count-' + $tabs.length);
            
            if (typeof validHashes === 'undefined' || validHashes.length === 0) {
                return;
            }

            $tabbedContent.attr('tab-index', 0);
            $tabbedContent.addClass('hidden-tab-content').attr('aria-hidden', true);

            $tabbedContent.each(function (n) {
                var $tabContent = $(this);
                var hasPagination = $tabContent.find('.pagination').length > 0;
                
                if (hasPagination) {
                    $tabContent.find('.pagination-links a').each(function () {
                        var linkHref = $(this).prop('href');
                        $(this).prop('href', linkHref + '#' + $tabContent.attr('id'));
                    });
                }
            });

            $('.tab-manipulator').on('click', function (e) {
                e.preventDefault();
                var hash = $(this).attr('href').replace('#', ''),
                    hasTab = $.inArray(hash, validHashes);            
                if (hasTab > -1) {
                    $tabs.eq(hasTab).click();
                    $(window).scrollTop($tabs.offset().top);
                }
            });

            $tabs.on('click', function (e) {                                
                var targetContent = $(this).attr('href');
                if (targetContent.indexOf('#') !== 0) {
                    return true;
                }
                e.preventDefault();
                location.replace(targetContent);
                
                $tabs.removeClass(opts.selectedTabClass);

                $tabbedContent.addClass('hidden-tab-content')
                    .attr('aria-hidden', true);                

                $(this).addClass(opts.selectedTabClass);

                $(targetContent).removeClass('hidden-tab-content')
                    .attr('aria-hidden', false)
                    .focus();

                $(window).trigger({ type: 'tabChange', tab: targetContent });
            });
            
            // only process the hash if it's for a valid
            if (window.location.hash !== '' && intialTabSelection > -1) {
                $tabs.eq(intialTabSelection).click();
            } else {
                if ($('.' + opts.selectedTabClass).length === 0) {
                    $tabs.slice(0, 1).addClass(opts.selectedTabClass);
                    $tabbedContent.slice(0, 1).removeClass('hidden-tab-content').attr('aria-hidden', false);
                } 
                
            }

            // 'prevent' scrolling to page anchors on load
            setTimeout(function() {
                window.scrollTo(0, 0);
            },0);
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

