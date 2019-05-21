
(function ($) {
    'use strict';

    var suggestionTemplate =
        "<div class='filter-suggestion multiple-choice multiple-choice--smaller multiple-choice--list'><input type='checkbox' class='js-filter-input filter-clone' id='clone-{0}' {2}/>" +
        "<label for='clone-{0}' class='js-filter-label'>{1}</label></div>";

    var defaults = {
        suggestionTemplate: suggestionTemplate
    };

    function SearchWithin(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    function throttle(fn, delay) {
        var t = null;
        return function () {
            var context = this;
            var args = arguments;
            clearTimeout(t);
            t = setTimeout(function () {
                fn.apply(context, args);
            }, delay);
        };
    }

    SearchWithin.prototype = {
        regenerateItems: function() {
            var filterData = [];
            $(this.el).find('.multiple-choice--list').each(function (n, elem) {
                var temp = {};
                temp.id = $(elem).find('.js-filter-input').prop('id');
                temp.text = $.trim($(elem).find('.js-filter-label').text());
                temp.checked = $(elem).find('.trigger-result-update').is(':checked');

                filterData.push(temp);

            });

            this.dataList = filterData;
        },
        init: function () {
            var $el = $(this.el);
            var opts = this.opts;
            var searchField = $el.find('.filter-search');
            var clearButton = $el.find('.field-clear');
            var originalFilters = $el.find('.js-filter-input');

            $el.find('.options-container').css({ minHeight: '200px' });

            var self = this;
            // helpers
            var renderSuggestions = function (options, userText) {
                var html = '';
                for (var i = 0, len = options.length; i < len; i++) {
                    var opt = options[i];
                    var optionText = opt.text.replace(new RegExp(userText, 'gi'), function(match) {
                        return '<span>' + match + '</span>';
                    });
                    var suggestion = opts.suggestionTemplate.replace(/\{0\}/g, opt.id).replace('{1}', optionText);
                    if (opt.checked) {
                        suggestion = suggestion.replace('{2}', 'checked="checked"');
                    } else {
                        suggestion = suggestion.replace('{2}', '');
                    }

                    html += suggestion;
                }
                $el.find('fieldset').addClass('hidden');
                $el.find('.suggestion-target').html(html);
                clearButton.removeClass('hidden');
            };

            var removeSuggestions = function () {
                $el.find('.suggestion-target').html('');
                $el.find('fieldset').removeClass('hidden');
                clearButton.addClass('hidden');
                searchField.val('');
            }

            this.regenerateItems();


            //attach events
            searchField.on('keyup', function () {
                if (this.value.trim().length > 1) {
                    var searchVal = new RegExp(this.value, 'i');
                    var options = self.dataList.filter(function (suggestion) {
                        if (searchVal.test(suggestion.text)) {
                            return suggestion;
                        }
                    });

                    throttle(renderSuggestions(options, this.value), 200);
                } else if (this.value.trim().length === 0) {
                    removeSuggestions();
                }
            });

            clearButton.on('click', function (e) {
                e.preventDefault();
                removeSuggestions();
            });

            //trigger click on the real filter
            $el.on('change', '.filter-clone', function (e) {
                var listId = this.id.replace('clone-', '');
                var realInput = $('#' + listId);
                var radioChecked = realInput.is(':checked');

                realInput.click();

                self.dataList.filter(function (item) {
                    if (item.id === listId) {
                        item.checked = !radioChecked;
                    }
                });
            });

            // maintain state on the datalist when the original filters are updated
            originalFilters.on('change', function () {
                var radioChecked = $(this).is(':checked');
                var radioId = this.id;
                self.regenerateItems();
                self.dataList.filter(function (item) {
                    if (item.id === radioId) {
                        item.checked = radioChecked;
                    }
                });
            });

        }
    }

    $.fn.searchWithinFilters = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'searchWithin')) {
                $.data(this, 'searchWithin', new SearchWithin(this, opts));
            }
        });
    }
}($));
