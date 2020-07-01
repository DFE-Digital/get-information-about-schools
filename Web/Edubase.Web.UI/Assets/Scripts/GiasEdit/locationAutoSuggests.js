const Awesomplete = require('awesomplete');

const locationAutoSuggest = (function() {
  const suggestionConfigs = [
    {
      textFieldId: 'AdministrativeDistrictName',
      suggestions: window.districts,
      hiddenFieldId: 'AdministrativeDistrictIdHidden',
      maxItems: 10
    },
    {
      textFieldId: 'AdministrativeWardName',
      suggestions: window.wards,
      hiddenFieldId: 'AdministrativeDistrictIdHidden',
      maxItems: 30
    },
    {
      textFieldId: 'ParliamentaryConstituencyName',
      suggestions: window.constituencies,
      hiddenFieldId: 'ParliamentaryConstituencyIdHidden',
      maxItems: 10
    },
    {
      textFieldId: 'CASWardName',
      suggestions: window.caswards,
      hiddenFieldId: 'CASWardIdHidden',
      maxItems: 30
    },
    {
      textFieldId: 'GSSLAName',
      suggestions: window.gsslas,
      hiddenFieldId: 'GSSLAIdHidden',
      maxItems: 10
    },
    {
      textFieldId: 'LSOAName',
      suggestions: window.lsoas,
      hiddenFieldId: 'LSOAIdHidden',
      maxItems: 10
    },
    {
      textFieldId: 'LSOAName',
      suggestions: window.lsoas,
      hiddenFieldId: 'LSOAIdHidden',
      maxItems: 10
    },
    {
      textFieldId: 'MSOAName',
      suggestions: window.msoas,
      hiddenFieldId: 'MSOAIdHidden',
      maxItems: 10
    },
  ];

function setUp() {
  for (const config of suggestionConfigs) {
    if (typeof config.suggestions !== 'undefined') {
      const _input = document.getElementById(config.textFieldId);
      const _hiddenInput = document.getElementById(config.hiddenFieldId);

      new Awesomplete(_input, {
        list: config.suggestions.map((k) => {
          return k.name;
        }),
        replace: function (suggestion) {
          this.input.value = suggestion;
          _hiddenInput.value = config.suggestions.filter((s) => {
            return s.name.toLowerCase() === suggestion.toLowerCase();
          })[0].id;
        },

        autoFirst: true,
        maxItems: config.maxItems,
        minChars: 1
      });
    }
  }
}

return { setUp };
}());

module.exports = locationAutoSuggest;

