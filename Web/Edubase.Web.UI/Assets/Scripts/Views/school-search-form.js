DfE.Views.schoolSearch = {
    showWarning: function ($panel, message) {
        var warningTemplate = '<div class="warning-message"><p>{0}</p></div>';
        if ($panel.find('.warning-message').length > 0) {
            $panel.find('.warning-message p').html(message);

        } else {
            $panel.prepend(warningTemplate.replace('{0}', message));
        }
        
    },
    init: function () {
        'use strict';

        var self = this,
           jScriptVersion;
        self.addedLaCount = 0;
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


        $('#location-search-submit').on('click', function (e) {
            if ($.trim($('#LocationSearchModel_Text').val()) === '') {
                e.preventDefault();
                self.showWarning($('#searchby-location-ref'),
                    'Please enter a postcode, town or city to start a search');
            }
        });

        $('#name-search-submit').on('click', function (e) {
            var suggestionCount = $('#TextSearchModel_Text').nextAll('.tt-menu').find('.tt-suggestion').length;
            var numericValue = !isNaN ($('#TextSearchModel_Text').val());

            if ($.trim($('#TextSearchModel_Text').val()) === '') {
                e.preventDefault();
                self.showWarning($('#searchby-name-ref'),
                    'Please enter an establishment name, URN, LAESTAB or UKPRN to start a search');

            } else if (suggestionCount === 0 && !numericValue) {
                e.preventDefault();
                self.showWarning($('#searchby-name-ref'),
                    'We couldn’t find any establishments matching your search criteria');
            }
        });

        $('#la-search-submit').on('click', function (e) {
            var suggestionCount = $('#LocalAuthorityToAdd').nextAll('.tt-menu').find('.tt-suggestion').length;
            if (self.addedLaCount === 0) {
                e.preventDefault();

                if ($.trim($('#LocalAuthorityToAdd').val()) === '') {
                    return self.showWarning($('#searchby-la-ref'),
                        'Please enter a local authority to start a search');
                } else if (suggestionCount === 0) {
                    return self.showWarning($('#searchby-la-ref'),
                        'We couldn\'t find any local authorities matching your search criteria');
                }
            } else {

                if (suggestionCount === 0 && $.trim($('#LocalAuthorityToAdd').val()).length > 0) {
                    e.preventDefault();
                    return self.showWarning($('#searchby-la-ref'),
                        'We don’t recognise this local authority. Amend it or clear it to continue searching.');
                }
            }

        });

        $('#group-search-submit').on('click', function (e) {
            var suggestionCount = $('#GroupSearchModel_Text').nextAll('.tt-menu').find('.tt-suggestion').length;
            var numericValue = !isNaN($('#TextSearchModel_Text').val().replace(/\D/g, ""));

            if ($.trim($('#GroupSearchModel_Text').val()) === '') {
                e.preventDefault();
                return self.showWarning($('#group-search-container'),
                    'Please enter an establishment group to start a search');

            } else if (suggestionCount === 0 && !numericValue) {
                e.preventDefault();
                return self.showWarning($('#group-search-container'),
                    'We couldn’t find any establishment groups matching your search criteria');
            }
        });

        $('#governor-search-submit').on('click', function (e) {
            var fName = $.trim($('#forename').val());
            var sName = $.trim($('#surname').val());
            var roles = $('#governor-roles').find(':checkbox').filter(':checked');
            var gId = $.trim($('#GovernorSearchModel_Gid').val());

           if (fName === '' && sName === '' && roles.length === 0) {
                    e.preventDefault();
                    return self.showWarning($('#searchtype-gov-namerole-ref'),
                        'Please enter a governor to start a search');
                }                       
        });

        $('#governor-search-submit-1').on('click', function (e) {
            var gId = $.trim($('#GovernorSearchModel_Gid').val());

           if (gId === '') {
                e.preventDefault();
                return self.showWarning($('#searchtype-gov-refno-ref'),
                    'Please enter a governor ID to start a search');
            }                       
        });


    },

    getSchoolsSuggestionHandler: function (keywords, callback) {
        var dataSuggestionUrl = $("#TextSearchModel_Text").attr("data-suggestion-url");
        return $.get(encodeURI(dataSuggestionUrl + keywords), function (response) {
            if (document.getElementById('include-open-establishments-name').checked) {
                var openOnly = response.filter(function(suggestion) {
                    if (!suggestion.closed) {
                        return suggestion;
                    }
                });
                return callback(openOnly);
            }
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
            console.log("The input field '" + targetInputElementName + "' does not exist.");
            return;
        }

        var self = this;
        var field = "text";
        var value = "id";
        var source = null;
        var minChars = 0;
        var suggestionsOpen = false;
        var selectedLocalAuthorities = [];
        var selectedLaButtonTemplate =
                '<a id="button-{1}" class="link-button font-small remove-suggest-la" data-remove="{1}">{0}</a>',
            selectedLaHiddenTemplate = '<input type="hidden" name="d" value="{0}" id="{1}" />',
            re = /\{0\}/g,
            reId = /\{1}/g;

            

        function includeLa(la) {
            var idString = "la-" + la.id;
            var hiddenField = selectedLaHiddenTemplate.replace(re, la.id).replace(reId, idString);
            var rmButton = selectedLaButtonTemplate.replace(re, la.name).replace(reId, idString);
            var $inputField = $('.floating-text-field-wrap');
            var previouslySelected = $.inArray(la.id, selectedLocalAuthorities) > -1;

            $('#searchby-la-ref').find('.warning-message').remove();

            if (!previouslySelected) {
                selectedLocalAuthorities.push(la.id);

                self.addedLaCount++;
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
            self.addedLaCount--;
            self.addedLaCount > 0
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

        var templateHandler = function (suggestion) {
            var tmpl = '<div><a href="javascript:">' + suggestion[field] + '</span></a></div>';

            if (suggestion.hasOwnProperty('closed') && suggestion.closed) {
                tmpl = '<div><a href="javascript:"><span class="estab-name">' + suggestion[field] + '</span><span class="estab-status">Closed</span></a></div>';
            }

            if (suggestion.hasOwnProperty('groupType') && suggestion.groupType) {
                tmpl = '<div><a href="javascript:"><span class="group-name">' + suggestion[field] + '</span><span class="group-type-name">' + suggestion.groupType + '</span></a></div>';
            }

            return tmpl;
        };

        $(targetInputElementName).typeahead({
            hint: false,
            highlight: true,
            highlightAliases: [
                ["st. ", "st ", "saint "]
            ],
            minLength: minChars,
            classNames: {
                menu: 'tt-menu',
                highlight: 'suggestion-highlight'
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
                $(targetInputElementName).typeahead('val', '');

                if ($(this).nextAll('.tt-menu').find('.tt-suggestion').length > 0) {
                    $(this).nextAll('.tt-menu').find('.tt-cursor').click();

                }
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
                $(targetResolvedInputElementName).val('');
            }
        });

        $(targetInputElementName).on('typeahead:open', function () {
            suggestionsOpen = true;
        });

        $(targetInputElementName).on('typeahead:close', function () {
            window.setTimeout(function () {
                suggestionsOpen = false;
            }, 0);
        });

        $('#LocalAuthorityToAdd').on('typeahead:render', function (e) {
            $(this).nextAll('.tt-menu').find('.tt-suggestion').slice(0,1).addClass('tt-cursor');
        });

        $('#LocalAuthorityToAdd').on('typeahead:open', function (e) {
            $(this).nextAll('.tt-menu').find('.tt-suggestion').slice(0,1).addClass('tt-cursor');
        });

        $('#LocalAuthorityToAdd').on('keydown', function (e) {
            var $input = $(this);
            if (e.which === 13) {
                e.preventDefault();
                e.stopPropagation();
                if ($.trim($(this).val()).length === 0) {
                    return self.showWarning($('#searchby-la-ref'), 'Please enter a local authority to start a search');

                }
                if (!$(this).nextAll('.tt-menu').hasClass('tt-empty')) {
                    $(this).nextAll('.tt-menu').find('.tt-cursor').click();
                } else {
                    return self.showWarning($('#searchby-la-ref'),
                        'We don’t recognise this local authority. Amend it or clear it to continue searching.');
                }
            }
            window.setTimeout(function () {
                if ($input.nextAll('.tt-menu').find('.tt-suggestion').length === 0 && $input.val().length > 3) {
                    return self.showWarning($('#searchby-la-ref'),
                        'We don’t recognise this local authority. Amend it or clear it to continue searching.');
                } else {
                    $('#searchby-la-ref').find('.warning-message').remove();
                }
            }, 0);

            
        });

        $(window).on('noLocationMatch', function(e) {
            self.showWarning($('#searchby-location-ref'),
                'We couldn’t find any locations matching your search criteria');
        });
    }

};

if ($('#content').find('.has-typeahead').length > 0) {
    DfE.Views.schoolSearch.init();
}

