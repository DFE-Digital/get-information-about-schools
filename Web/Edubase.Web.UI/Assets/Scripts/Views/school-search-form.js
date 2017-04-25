DfE.Views.schoolSearch = {
   init: function () {
        'use strict';
       var self = this,
           jScriptVersion;

        /*@cc_on
            jScriptVersion = @_jscript_version
        @*/

        $('#content').find('.search-toggle-panel').radioToggle({ untoggle: true });

        if (typeof (jScriptVersion) === 'undefined' || jScriptVersion >= 9) {
            $(function () {
                setTimeout(function () {
                    self.bindAutosuggest('#TextSearchModel_Text',
                        '#TextSearchModel_AutoSuggestValue',
                        self.getSchoolsSuggestionHandler);

                    self.bindAutosuggest('#GroupSearchModel_Text',
                        '#GroupSearchModel_AutoSuggestValue',
                        self.getTrustSuggestionHandler);

                    self.bindAutosuggest('#LocalAuthorityToAdd',
                        '#LocalAuthoritySearchModel_AutoSuggestValue',
                        { data: window.localAuthorities, name: "name", value: "id" });
                }, 500);
            });
        }
    },
        
    getSchoolsSuggestionHandler: function (keywords, callback) {
        var dataSuggestionUrl = $("#TextSearchModel_Text").attr("data-suggestion-url");
        return $.get(encodeURI(dataSuggestionUrl + keywords), function (response) {
            return callback(response);
        });
    },

    getTrustSuggestionHandler: function (keywords, callback) {
        var dataSuggestionUrl = $("#GroupSearchModel_Text").attr("data-suggestion-url");
        return $.get(encodeURI(dataSuggestionUrl + keywords), function (response) {
            return callback(response);
        });
    },

    bindAutosuggest: function (targetInputElementName, targetResolvedInputElementName, suggestionSource) {

        if ($(targetInputElementName).length === 0) {
            console.log("The input field '"+targetInputElementName+"' does not exist.");
            return;
        }

            
        var field = "text";
        var value = "id";
        var source = null;
        var minChars = 0;
        var selectedLocalAuthorities = [];
        var selectedLaButtonTemplate = '<button type="submit" id="button-{1}" class="link-button font-small remove-suggest-la" name="LocalAuthorityToRemove" data-remove="{1}">{0}</button>',
            selectedLaHiddenTemplate = '<input type="hidden" name="d" value="{0}" id="{1}" />',
            re = /\{0\}/g,
            reId = /\{1}/g,
            addedLaCount = 0;

        function includeLa(la) {
            var idString = "la-" + la.id;
            var hiddenField = selectedLaHiddenTemplate.replace(re, la.id).replace(reId, idString);
            var rmButton = selectedLaButtonTemplate.replace(re, la.name).replace(reId, idString);
            var $inputField = $('.floating-text-field-wrap');
            var previouslySelected = $.inArray(la.id, selectedLocalAuthorities) > -1;

            console.log("previouslySelected " + previouslySelected);

            if (!previouslySelected) {
                selectedLocalAuthorities.push(la.id);
                
                addedLaCount++;
                $(rmButton).insertBefore($inputField);
                $('#la-id-target').append(hiddenField);
            } else {
                var button = $('#button-' + idString).detach();
                
                button.insertBefore($inputField);
            }
   
        }

        $('#la-id-target').on('click', '.remove-suggest-la', function (e) {
            e.preventDefault();
            $('#' + $(this).data().remove).remove();
            $(this).remove();
            addedLaCount--;
            addedLaCount > 0
                ? $('.selected-las').addClass('has-results')
                : $('.selected-las').removeClass('has-results');
        });
        
        $('#LocalAuthorityToAdd').on('focus', function () {
            $('#la-id-target').addClass('focused');

        }).on('blur', function () {
            $('#la-id-target').removeClass('focused');

        });

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

            if (targetInputElementName === '#LocalAuthorityToAdd') {
                    includeLa(suggestion);
                $(targetInputElementName).typeahead('val','');
            }
               

        });

        $(targetInputElementName).bind("typeahead:autocomplete", function (src, suggestion) {
            $(targetResolvedInputElementName).val(suggestion[value]);
            currentSuggestionName = suggestion[name];
        });

        $(targetInputElementName).bind("input propertychange", function (event) {
            // When the user changes the value in the search having already selected an item, ensure the selection resets
            var currentValue = $(event.target).val();
            if (currentValue !== currentSuggestionName) {
                $(targetResolvedInputElementName).val("");
            }
        });

        $(targetInputElementName).on('keydown', function(e) {
            if (e.which === 13) {
                e.preventDefault();
                console.log('Enter press...');
                $(".tt-suggestion:first-child").click();
            }
        });
    } 

};

if ($('#content').find('.has-typeahead').length > 0) {
    DfE.Views.schoolSearch.init();
}

var $blockLabels = $(".block-label input[type='radio'], .block-label input[type='checkbox']");
new GOVUK.SelectionButtons($blockLabels);