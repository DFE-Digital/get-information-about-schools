DfE.Views.editLocation = {
  init: function (options) {
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
          self.bindAutosuggest('#LSOAName',
            '#LSOAIdHidden',
            { data: window.lsoas, name: "name", value: "id" });
          self.bindAutosuggest('#MSOAName',
            '#MSOAIdHidden',
            { data: window.msoas, name: "name", value: "id" });
        }, 500);
      });
    }
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
      return '<div><a class="js-allow-exit" href="javascript:">' + suggestion[field] + '</a></div>';
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
    });

    $(targetInputElementName).bind("typeahead:autocomplete", function (src, suggestion) {
      $(targetResolvedInputElementName).val(suggestion[value]);
      currentSuggestionName = suggestion[name];
    });

    $(targetInputElementName).bind("input propertychange", function (event) {
      // When the user changes the value in the search having already selected an item, ensure the selection resets
      var currentValue = $(event.target).val();
      if (currentValue === "" || currentValue !== currentSuggestionName) {
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
    
    $('#LSOAName').on('typeahead:render', function (e) {
      highLightFirstSuggestion.call(this);
    });

    $('#LSOAName, #TextSearchModel_Text').on('typeahead:open', function (e) {
      highLightFirstSuggestion.call(this);
    });

    $('#LSOAName').on('keydown', function (e) {
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

    $('#MSOAName').on('typeahead:render', function (e) {
      highLightFirstSuggestion.call(this);
    });

    $('#MSOAName, #TextSearchModel_Text').on('typeahead:open', function (e) {
      highLightFirstSuggestion.call(this);
    });

    $('#MSOAName').on('keydown', function (e) {
      var $input = $(this);
      if (e.which === 13) {
        e.preventDefault();
        e.stopPropagation();
        if ($.trim($(this).val()).length === 0) {
          return self.showWarning($('#searchby-MSOA-ref'), 'Please enter a MSOA to start a search');

        }
        if (!$(this).nextAll('.tt-menu').hasClass('tt-empty')) {
          $(this).nextAll('.tt-menu').find('.tt-cursor').click();
        } else {
          return self.showWarning($('#searchby-msoa-ref'),
            'We don’t recognise this MSOA. Amend it or clear it to continue searching.');
        }
      }
    });
  }

};


