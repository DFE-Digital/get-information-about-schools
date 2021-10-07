const Awesomplete = require('awesomplete');
import GiasRadioToggles from '../GiasSearchForm/GiasRadioToggles';

$('#main-content').find('.search-toggle-panel').giasRadioToggles({
  untoggle: true
});

const MINCHARS = 2;
let intervalId;

function openSuggestionsOnFocus(autocomplete) {
  if (autocomplete.ul.childNodes.length > 0 && autocomplete.ul.hasAttribute('hidden') && autocomplete.input.value.length > MINCHARS) {
    autocomplete.evaluate();
    autocomplete.open();
  }
}

function getNameSuggestions(searchString, autocomplete, isMatSearch) {
  const url =  isMatSearch ? `/search/suggestgroup?text=${searchString}`: `/search/suggest?text=${searchString}`;

  intervalId = setTimeout(function() {
    const openOnly = document.getElementById('include-open-establishments-name');
    $.ajax({
      url: url,
      dataType: 'json',
      success: function(response) {
        autocomplete.list = response.filter((school)=> {
          if (openOnly && openOnly.checked) {
            return !school.closed;
          }
          return true;
        }).map((school) => {
          let obj = {};
          obj.text = school.text;
          obj.urn = school.urn;
          return obj;
        });
      }
    });
  }, 200);
}
const schoolNameInput = document.getElementById('TextSearchModel_Text');
if (schoolNameInput) {
  schoolNameInput.classList.remove('govuk-!-width-one-half');
  const schoolNameAutoSuggest = new Awesomplete(schoolNameInput, {
    replace: function (suggestion) {
      this.input.value = suggestion;
    },
    data: function (item, input) {
      return {
        label: item.text,
        value: item.urn
      }
    },
    filter: function (item, input) {
      return true;
    },
    sort: false,
    autoFirst: true
  });

  schoolNameInput.addEventListener('keyup', function (e) {
    const code = (e.keyCode || e.which);
    let shouldGetResults = true;
    clearInterval(intervalId);
    if (code === 37 || code === 38 || code === 39 || code === 40 || code === 27 || code === 13) {
      shouldGetResults = false;
    }

    if (this.value.length > 2 && shouldGetResults) {
      getNameSuggestions(this.value, schoolNameAutoSuggest, false);
    }
  });

  schoolNameInput.addEventListener('awesomplete-select', function (ev) {
    document.getElementById('estab-autosuggest-value').value = ev.text.value;
  });

  schoolNameInput.addEventListener('focus', function () {
    openSuggestionsOnFocus(schoolNameAutoSuggest);
  });

  schoolNameInput.addEventListener('paste', function (event) {
    const pastedValue = (event.clipboardData || window.clipboardData).getData('text');
    if (pastedValue.length > 2) {
      getNameSuggestions(pastedValue, schoolNameAutoSuggest, false);
    }
  });
}
const matNameInput = document.getElementById('GroupSearchModel_Text');
if (matNameInput) {
  matNameInput.classList.remove('govuk-!-width-one-half');
  const matNameAutoSuggest = new Awesomplete(matNameInput, {
    replace: function (suggestion) {
      this.input.value = suggestion;
    },
    data: function (item, input) {
      return {
        label: item.text,
        value: item.urn
      }
    },
    filter: function (item, input) {
      return true;
    },
    sort: false,
    autoFirst: true
  });

  matNameInput.addEventListener('keyup', function (e) {
    const code = (e.keyCode || e.which);
    let shouldGetResults = true;
    clearInterval(intervalId);
    if (code === 37 || code === 38 || code === 39 || code === 40 || code === 27 || code === 13) {
      shouldGetResults = false;
    }

    if (this.value.length > 2 && shouldGetResults) {
      getNameSuggestions(this.value, matNameAutoSuggest, true);
    }
  });

  matNameInput.addEventListener('awesomplete-select', function (ev) {
    document.getElementById('GroupSearchModel_AutoSuggestValue').value = ev.text.value;
  });

  matNameInput.addEventListener('focus', function () {
    openSuggestionsOnFocus(matNameAutoSuggest);
  });

  matNameInput.addEventListener('paste', function (event) {
    const pastedValue = (event.clipboardData || window.clipboardData).getData('text');
    if (pastedValue.length > 2) {
      getNameSuggestions(pastedValue, matNameAutoSuggest, true);
    }
  });
}
