const Awesomplete = require('awesomplete');

const autocompleteSetup = (function (){
  const MINCHARS = 2;
  let intervalId;
  let nameSuggestions;

  function debounce(func, wait) {
    let timeout;
    return function executed(...args) {
      const context = this;
      const later = () => {
        clearTimeout(intervalId);
        func.apply(context, args);
      };
      clearTimeout(intervalId);
      intervalId = setTimeout(later, wait);
    };
  }

  function getNameSuggestions(searchString, autocomplete, isMatSearch) {
    const url =  isMatSearch ? `/search/suggestgroup?text=${searchString}`: `/search/suggest?text=${searchString}`;

    const openOnly = document.getElementById('include-open-establishments-name');
      $.ajax({
        url: url,
        dataType: 'json',
        success: function(response) {
          const suggestions = response.map((school) => {
            return {
              text: school.text,
              urn: school.urn,
              closed: school.closed
            }
          });

          nameSuggestions = suggestions;

          autocomplete.list = suggestions.filter((school) => {
            return openOnly && openOnly.checked ? !school.closed : true;
              return !school.closed;
          });
        }
      });
  }

  const debouncedGetNameSuggestions = debounce(getNameSuggestions, 500);

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
    const schoolNameOpenChx = document.getElementById('include-open-establishments-name');

    if (schoolNameInput) {
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
        if (code === 37 || code === 38 || code === 39 || code === 40 || code === 27 || code === 13) {
          shouldGetResults = false;
        }
        if (this.value.length > MINCHARS && shouldGetResults) {
          debouncedGetNameSuggestions(this.value, schoolNameAutoSuggest, false);
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
        if (pastedValue.length > MINCHARS) {
          debouncedGetNameSuggestions(pastedValue, schoolNameAutoSuggest, false);
        }
      });

      schoolNameOpenChx.addEventListener('change', function (event) {
        if (schoolNameInput.value.length > MINCHARS) {
          schoolNameAutoSuggest.ul.innerHTML = '';
          if (!schoolNameOpenChx.checked) {
            schoolNameAutoSuggest.list = nameSuggestions;
          } else {
            const openSchools = nameSuggestions.filter(school => {
              return !school.closed
            });

            schoolNameAutoSuggest.list = openSchools;
          }

          schoolNameAutoSuggest.evaluate();
        }
      });
    }
    /* ###################
        MAT name
     ################### */
    const matNameInput = document.getElementById('GroupSearchModel_Text');

    if (matNameInput) {
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

        if (this.value.length > MINCHARS && shouldGetResults) {
          debouncedGetNameSuggestions(this.value, matNameAutoSuggest, true);
        }
      });

      matNameInput.addEventListener('awesomplete-select', function (ev) {
        document.getElementById('TextSearchModel_AutoSuggestValue').value = ev.text.value;
      });

      matNameInput.addEventListener('focus', function () {
        openSuggestionsOnFocus(matNameAutoSuggest);
      });

      matNameInput.addEventListener('paste', function (event) {
        const pastedValue = (event.clipboardData || window.clipboardData).getData('text');
        if (pastedValue.length > MINCHARS) {
          debouncedGetNameSuggestions(pastedValue, matNameAutoSuggest, true);
        }
      });
    }

    /* ###################
        School location
     ################### */

    const schoolLocationInput = document.getElementById('LocationSearchModel_Text');

    if (schoolLocationInput) {
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

        if (this.value.length >= MINCHARS && shouldGetResults) {
          getLocationSuggestions(this.value, schoolLocationAutoSuggest);
        }
      });

      schoolLocationInput.addEventListener('awesomplete-select', function (ev) {
        document.getElementById('LocationSearchModel_AutoSuggestValue').value = ev.text.value;
      });

      schoolLocationInput.addEventListener('focus', function () {
        openSuggestionsOnFocus(schoolLocationAutoSuggest);
      });

      schoolLocationInput.addEventListener('paste', function(event) {
        const pastedValue = (event.clipboardData || window.clipboardData).getData('text');
        if (pastedValue.length > MINCHARS) {
          getLocationSuggestions(pastedValue, schoolLocationAutoSuggest);
        }
      });
    }
    /* ###################
        Local authority
     ################### */
    const laInput = document.getElementById('LocalAuthorityToAdd');
    if (laInput) {
      let selectedLocalAuthorities = [];
      const laAutoSuggest = new Awesomplete(laInput, {
        list: window.localAuthorities,
        data: function (item, text) {
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
        $('#' + $(this).text().toLowerCase().replace(/\s/g, '-')).remove();
        $(this).remove();
        $("input[value='" + $(this).val() +"'][name='d']").remove();
      });

      $('#LocalAuthorityToAdd').on('focus', function () {
        $('#la-id-target').addClass('focused');

      }).on('blur', function () {
        $('#la-id-target').removeClass('focused');

      });

      laInput.addEventListener('focus', function () {
        openSuggestionsOnFocus(laAutoSuggest);
      });

      laInput.addEventListener('awesomplete-selectcomplete', function (e) {
        let rmButton = `<button id="button-${e.text.value}" class="remove-suggest-la" data-remove="${e.text.value}" title="Remove ${e.text.label} local authority">${e.text.label}</a>`;
        let hiddenField = `<input type="hidden" name="d" value="${e.text.value}" id="${e.text.label.toLowerCase().replace(/\s/g, '-')}" />`;

        const buttonTarget = $('.floating-text-field-wrap');
        const previouslySelected = $.inArray(e.text.value, selectedLocalAuthorities) > -1;

        if (!previouslySelected) {
          selectedLocalAuthorities.push(e.text.value);

          $(rmButton).insertBefore(buttonTarget);
          $('#la-id-target').append(hiddenField);
        } else {
          const button = $('#button-' + e.text.value).detach();

          button.insertBefore(buttonTarget);
        }

        laInput.value = '';
      });
    }
  }

  return { setUp };
}());

export default autocompleteSetup;
