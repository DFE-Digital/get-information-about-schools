const Awesomplete = require('awesomplete');

const autocompleteSetup = (function(){
  const MINCHARS = 2;
  let intervalId;

  function getNameSuggestions(searchString, autocomplete, isMatSearch) {
    let url =  isMatSearch ? `/search/suggestgroup?text=${searchString}`: `/search/suggest?text=${searchString}`;

    intervalId = setTimeout(function() {
    const openOnly = document.getElementById('include-open-establishments-name');
      $.ajax({
        url: `/search/suggest?text=${searchString}`,
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

  function getLocationSuggestions(searchString, autocomplete) {
    intervalId = setTimeout(function() {
      $.ajax({
        url: `/search/suggestplace?text=${searchString}`,
        dataType: 'json',
        success: function(response) {
          autocomplete.list = response.map((location) => {
            let obj = {};
            obj.text = location.name.replace('Saint', 'St ');
            obj.location = `${location.coords.latitude}, ${location.coords.longitude}`;

            return obj;
          });
        }
      });
    }, 200);
  }

  function openSuggestionsOnFocus(autocomplete) {
    if (autocomplete.ul.childNodes.length > 0 && autocomplete.ul.hasAttribute('hidden') && autocomplete.input.value.length > MINCHARS) {
      autocomplete.evaluate();
      autocomplete.open();
    }
  }

  function setUp() {
    /* ###################
        School name
     ################### */
    const schoolNameInput = document.getElementById('TextSearchModel_Text');

    const schoolNameAutoSuggest = new Awesomplete(schoolNameInput, {
      replace: function(suggestion) {
        this.input.value = suggestion;
      },
      data: function(item, input) {
        return {
          label: item.text,
          value: item.urn
        }
      },
      filter: function(item, input) {
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
        getNameSuggestions(this.value, schoolNameAutoSuggest,false);
      }
    });

    schoolNameInput.addEventListener('awesomplete-select', function (ev) {
      document.getElementById('TextSearchModel_AutoSuggestValue').value = ev.text.value;
    });

    schoolNameInput.addEventListener('focus', function () {
      openSuggestionsOnFocus(schoolNameAutoSuggest);
    });

    /* ###################
        MAT name
     ################### */
    const matNameInput = document.getElementById('GroupSearchModel_Text');

    const matNameAutoSuggest = new Awesomplete(matNameInput, {
      replace: function(suggestion) {
        this.input.value = suggestion;
      },
      data: function(item, input) {
        return {
          label: item.text,
          value: item.urn
        }
      },
      filter: function(item, input) {
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
      document.getElementById('TextSearchModel_AutoSuggestValue').value = ev.text.value;
    });

    matNameInput.addEventListener('focus', function () {
      openSuggestionsOnFocus(schoolNameAutoSuggest);
    });

    /* ###################
        School location
     ################### */

    const schoolLocationInput = document.getElementById('LocationSearchModel_Text');

    const schoolLocationAutoSuggest = new Awesomplete(schoolLocationInput, {
      replace: function (suggestion) {
        this.input.value = suggestion;
      },
      data: function (item, input) {
        return {label: item.text, value: item.location}
      },
      filter: function () {
        return true;
      },
      sort: false,
      autoFirst: true
    });

    schoolLocationInput.addEventListener('keyup', function (e) {
      let shouldGetResults = true;
      const code = (e.keyCode || e.which);
      clearInterval(intervalId);
      // ensure that if the key stroke should navigate the suggestions, it still works and doesn't refresh the data
      if (code === 37 || code === 38 || code === 39 || code === 40 || code === 27 || code === 13) {
        shouldGetResults = false;
      }

      if (this.value.length >= 2 && shouldGetResults) {
        getLocationSuggestions(this.value, schoolLocationAutoSuggest);
      }
    });

    schoolLocationInput.addEventListener('awesomplete-select', function (ev) {
      document.getElementById('LocationSearchModel_AutoSuggestValue').value = ev.text.value;
    });

    schoolLocationInput.addEventListener('focus', function () {
      openSuggestionsOnFocus(schoolLocationAutoSuggest);
    });

    /* ###################
        Local authority
     ################### */
    const laInput = document.getElementById('LocalAuthorityToAdd');
    let selectedLocalAuthorities = [];
    const laAutoSuggest = new Awesomplete(laInput, {
      list: window.localAuthorities,
      data: function(item, text) {
        return {
          label: item.name,
          value: item.id
        }
      },
      sort: false,
      autoFirst: true
    });

    $('#la-id-target').on('click', '.remove-suggest-la', function (e) {
      e.preventDefault();
      $('#' + $(this).text().toLowerCase().replace(/\s/g ,'-')).remove();
      $(this).remove();
    });

    $('#LocalAuthorityToAdd').on('focus', function () {
      $('#la-id-target').addClass('focused');

    }).on('blur', function () {
      $('#la-id-target').removeClass('focused');

    });

    laInput.addEventListener('focus', function (){
      openSuggestionsOnFocus(laAutoSuggest);
    });

    laInput.addEventListener('awesomplete-selectcomplete', function (e) {
      let rmButton = `<a href="#" id="button-${e.text.value}" class="remove-suggest-la" data-remove="${e.text.value}" title="Remove ${e.text.label} local authority">${e.text.label}</a>`;
      let hiddenField = `<input type="hidden" name="d" value="${e.text.value}" id="${e.text.label.toLowerCase().replace(/\s/g ,'-')}" />`;

      const buttonTarget = $('.floating-text-field-wrap');
      const previouslySelected = $.inArray(e.text.value, selectedLocalAuthorities) > -1;

      $('#searchby-la-ref').find('.warning-message').addClass('govuk-visually-hidden');

      if (!previouslySelected) {
        selectedLocalAuthorities.push(e.text.value);

        //self.addedLaCount++;
        $(rmButton).insertBefore(buttonTarget);
        $('#la-id-target').append(hiddenField);
      } else {
        const button = $('#button-' + e.text.value).detach();

        button.insertBefore(buttonTarget);
      }

      laInput.value = '';
    });
  }

  return { setUp };
}());

module.exports = autocompleteSetup;
