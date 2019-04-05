DfE.Views.editLocation = {
  init: function (options) {
    debugger;
    'use strict';

    var defaults = {
      highlightFirstSuggestion: false
    };

    this.opts = $.extend({}, defaults, options);

    var self = this,
      jScriptVersion;
    self.addedLaCount = 0;
    /*@cc_on
        jScriptVersion = @_jscript_version
    @*/

    $('#content').find('.search-toggle-panel').radioToggle({
      untoggle: true
    });

    if (typeof (jScriptVersion) === 'undefined' || jScriptVersion >= 9) {
      $(function () {
        setTimeout(function () {
          self.bindAutosuggest('#LSOAToAdd',
            { data: window.lsoas, name: "name", value: "id" });
        }, 500);
      });
    }
  },
  
  bindAutosuggest: function (targetInputElementName, suggestionSource) {

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


    $('#LSOAToAdd').on('focus', function () {
      $('#LSOAToAddTarget').addClass('focused');
    }).on('blur', function () {
      $('#LSOAToAddTarget').removeClass('focused');
    });

    if (typeof (suggestionSource) === "function") { // remote source
      minChars = 3;
      source = function (query, syncResultsFn, asyncResultsFn) {
        return suggestionSource.call(self, query, asyncResultsFn);
      };
    } else if (typeof (suggestionSource) === "object") { // local data source

      if (!suggestionSource.data) { console.log("suggestionSource.data is null"); return; }
      if (!suggestionSource.name) { console.log("suggestionSource.name is null"); return; }
      if (!suggestionSource.value) { console.log("suggestionSource.value is null"); return; }

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
      return '<div><a href="javascript:">' + suggestion[field] + '</a></div>';
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

      if (targetInputElementName === '#LSOAToAdd') {
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

    function highLightFirstSuggestion() {
      $(this).nextAll('.tt-menu').find('.tt-suggestion').slice(0, 1).addClass('tt-cursor');
    }

    if (self.opts.highlightFirstSuggestion) {
      $('#TextSearchModel_Text, #GroupSearchModel_Text').on('typeahead:render', function (e) {
        highLightFirstSuggestion.call(this);
      });
    }

    $('#LSOAToAdd').on('typeahead:render', function (e) {
      highLightFirstSuggestion.call(this);
    });

    $('#LSOAToAdd, #TextSearchModel_Text').on('typeahead:open', function (e) {
      highLightFirstSuggestion.call(this);
    });

    $('#LSOAToAdd').on('keydown', function (e) {
      var $input = $(this);
      if (e.which === 13) {
        e.preventDefault();
        e.stopPropagation();
        if ($.trim($(this).val()).length === 0) {
          return self.showWarning($('#searchby-lsoa-ref'), 'Please enter a LSOA to start a search');

        }
        if (!$(this).nextAll('.tt-menu').hasClass('tt-empty')) {
          $(this).nextAll('.tt-menu').find('.tt-cursor').click();
        } else {
          return self.showWarning($('#searchby-lsoa-ref'),
            'We don’t recognise this LSOA. Amend it or clear it to continue searching.');
        }
      }
    });

    $(window).on('noLocationMatch', function (e) {
      var text = $('#LocationSearchModel_Text').val();
      var match = text.match(/\b[A-Z]{1,2}[0-9][A-Z0-9]? [0-9][ABD-HJLNP-UW-Z]{2}\b/i);
      if (!match) { // only show message when not a postcode
        self.showWarning($('#searchby-location-ref'),
          'We couldn’t find any locations matching your search criteria');
      }
    });
  }

};


