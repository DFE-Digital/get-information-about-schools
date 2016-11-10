
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

            var self = this;
            $(function () {
                setTimeout(function () {
                    self.bindAutosuggest('#TextSearchModel_Text', '#TextSearchModel_AutoSuggestValue', self.getSchoolsSuggestionHandler);
                    self.bindAutosuggest('#TrustSearchModel_Text', '#TrustSearchModel_AutoSuggestValue', self.getTrustSuggestionHandler);
                    //self.bindAutosuggest('#LocalAuthoritySearchModel_Text', '#LocalAuthoritySearchModel_AutoSuggestValue', { data: window.localAuthorities, name: "name", value: "id" });
                }, 500);
                
            });
        },
        
        getSchoolsSuggestionHandler: function (keywords, callback) {
            var dataSuggestionUrl = $("#TextSearchModel_Text").attr("data-suggestion-url");
            return $.get(encodeURI(dataSuggestionUrl + keywords), function (response) {
                return callback(response);
            });
        },

        getTrustSuggestionHandler: function (keywords, callback) {
            var dataSuggestionUrl = $("#TrustSearchModel_Text").attr("data-suggestion-url");
            return $.get(encodeURI(dataSuggestionUrl + keywords), function (response) {
                return callback(response);
            });
        },

        bindAutosuggest: function (targetInputElementName, targetResolvedInputElementName, suggestionSource) {

            if ($(targetInputElementName).length == 0) {
                console.log("The input field '"+targetInputElementName+"' does not exist.");
                return;
            }

            var field = "text";
            var value = "id";
            var source = null;
            var minChars = 0;

            if (typeof (suggestionSource) === "function") { // remote source
                //console.log("suggestionSource is a function");
                minChars = 3;
                source = function (query, syncResultsFn, asyncResultsFn) {
                    return suggestionSource.call(self, query, asyncResultsFn);
                };
            } else if (typeof (suggestionSource) === "object") { // local data source

                if (!suggestionSource.data) { console.log("suggestionSource.data is null"); return; }
                if (!suggestionSource.name) { console.log("suggestionSource.name is null"); return; }
                if (!suggestionSource.value) { console.log("suggestionSource.value is null"); return; }
                //console.log("suggestionSource.data has " + suggestionSource.data.length + " items");

                minChars = 2;
                field = suggestionSource.name;
                value = suggestionSource.value;

                source = new Bloodhound({
                    datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d[field]); },
                    queryTokenizer: Bloodhound.tokenizers.whitespace,
                    local: suggestionSource.data
                });
                source.initialize();
            } else {
                console.log("Incompatible suggestionSource");
                return;
            }

            var templateHandler = function (suggestion) { return '<div><a href="javascript:">' + suggestion[field] + '</a></div>'; };

            $(targetInputElementName).typeahead({
                hint: false,
                highlight: true,
                highlightAliases: [
                    ["st. ", "st ", "saint "]
                ],
                minLength: minChars,
                classNames: {
                    menu: 'tt-menu form-control mtm',
                    highlight: 'bold-small'
                },
                ariaOwnsId: "arialist_" + DfE.Util.randomNumber()
            }, {
                display: field,
                limit: 10,
                source: source,
                templates: {
                    suggestion: templateHandler
                }
            });

            var currentSuggestionName = "";

            $(targetInputElementName).bind("typeahead:select", function (src, suggestion) {
                $(targetResolvedInputElementName).val(suggestion[value]);
                currentSuggestionName = suggestion[field];
            });

            $(targetInputElementName).bind("typeahead:autocomplete", function (src, suggestion) {
                $(targetResolvedInputElementName).val(suggestion[value]);
                currentSuggestionName = suggestion[name];
            });

            $(targetInputElementName).bind("input propertychange", function (event) {
                // When the user changes the value in the search having already selected an item, ensure the selection resets
                var currentValue = $(event.target).val();
                if (currentValue != currentSuggestionName) {
                    $(targetResolvedInputElementName).val("");
                }
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

