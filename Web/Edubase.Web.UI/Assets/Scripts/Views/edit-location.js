DfE.Views.editLocation = {
  init: function(options) {
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
      $(function() {
        setTimeout(function() {
          self.bindAutosuggest('#AdministrativeDistrictName', '#searchby-district-ref', 'District', '#AdministrativeDistrictIdHidden', { data: window.districts, name: "name", value: "id" });
          self.bindAutosuggest('#AdministrativeWardName', '#searchby-ward-ref', 'Ward', '#AdministrativeWardIdHidden', { data: window.wards, name: "name", value: "id" });
          self.bindAutosuggest('#ParliamentaryConstituencyName', '#searchby-constituency-ref', 'Parliamentary constituency', '#ParliamentaryConstituencyIdHidden', { data: window.constituencies, name: "name", value: "id" });
          self.bindAutosuggest('#CASWardName', '#searchby-casward-ref', 'Census ward', '#CASWardIdHidden', { data: window.caswards, name: "name", value: "id" });
          self.bindAutosuggest('#GSSLAName', '#searchby-gssla-ref', 'LA Code', '#GSSLAIdHidden', { data: window.gsslas, name: "name", value: "id" });
          self.bindAutosuggest('#LSOAName', '#searchby-lsoa-ref', 'LSOA', '#LSOAIdHidden', { data: window.lsoas, name: "name", value: "id" });
          self.bindAutosuggest('#MSOAName', '#searchby-msoa-ref', 'MSOA', '#MSOAIdHidden', { data: window.msoas, name: "name", value: "id" });
          },
          500);
      });
    }
  },

  bindAutosuggest: function (targetInputElementName, warningInputElement, targetDisplayName, targetResolvedInputElementName, suggestionSource) {

    // Setting a .focused class on the pseudo wrapper
    $(targetInputElementName).on('focus', function () {
        $(warningInputElement + ' .autosuggest').addClass('focused');
    }).on('blur', function () {
        $(warningInputElement + ' .autosuggest').removeClass('focused');
    });

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
        datumTokenizer: function (d) { return Bloodhound.tokenizers.nonword(d[field]); },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: suggestionSource.data,
        sorter: function (a, b) {
            var inputString = $(targetInputElementName).val().toLowerCase();
            var aText = a[field].toLowerCase();
            var bText = b[field].toLowerCase();
            var regex = /(.*?)\s\[(.*?)\]/gi;

            //move exact non-bracket matches to top
            if (inputString === aText.replace(regex, "$1")) { return -1; }
            if (inputString === bText.replace(regex, "$1")) { return 1; }

            //move exact inner-bracket matches to top
            if (inputString === aText.replace(regex, "$2")) { return -1; }
            if (inputString === bText.replace(regex, "$2")) { return 1; }
            
            if ((inputString !== aText) && (inputString !== bText)) {
              if (aText < bText) {
                return -1;
              }
              else if (aText > bText) {
                return 1;
              }
              else return 0;
            }
            return 0;
        }
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

    $(targetInputElementName).on('typeahead:render',
      function (e) {
        highLightFirstSuggestion.call(this);
      });

    $(targetInputElementName, '#TextSearchModel_Text').on('typeahead:open',
      function (e) {
        highLightFirstSuggestion.call(this);
      });

    $(targetInputElementName).on('keydown',
      function (e) {
        var $input = $(this);
        if (e.which === 13) {
          e.preventDefault();
          e.stopPropagation();
          if ($.trim($(this).val()).length === 0) {
            return self.showWarning($(warningInputElement), 'Please enter a ' + targetDisplayName + ' to start a search');
          }
          if (!$(this).nextAll('.tt-menu').hasClass('tt-empty')) {
            $(this).nextAll('.tt-menu').find('.tt-cursor').click();
          } else {
            return self.showWarning($(warningInputElement),
              'We don’t recognise this ' + targetDisplayName + '. Amend it or clear it to continue searching.');
          }
        }
      });
  }
};
