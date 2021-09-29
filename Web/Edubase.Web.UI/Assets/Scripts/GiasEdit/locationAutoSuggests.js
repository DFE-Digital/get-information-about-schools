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
      hiddenFieldId: 'AdministrativeWardIdHidden',
      maxItems: 30
    },
    {
      textFieldId: 'ParliamentaryConstituencyName',
      suggestions: window.constituencies,
      hiddenFieldId: 'ParliamentaryConstituencyIdHidden',
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

      _input.dataset.storedValue = _input.value;
      _input.removeAttribute('disabled');

      const awe = new Awesomplete(_input, {
        list: config.suggestions.map((k) => {
          return k.name;
        }),
        replace: function (suggestion) {
          this.input.value = suggestion;
          _hiddenInput.value = config.suggestions.filter((s) => {
            return s.name.toLowerCase() === suggestion.toLowerCase();
          })[0].id;

          _input.dataset.storedValue = suggestion;
        },

        autoFirst: true,
        maxItems: config.maxItems,
        minChars: 1
      });

      _input.addEventListener('change',function(){
        if (_input.value !== _input.dataset.storedValue) {
          _hiddenInput.value = '';
        }
      });

      _input.addEventListener('focus', function() {
        if (awe.ul.childNodes.length > 0 && awe.ul.hasAttribute('hidden')) {
          awe.evaluate();
          awe.open();
        }
      });
    }
  }
}

return { setUp };
}());

export default locationAutoSuggest;

