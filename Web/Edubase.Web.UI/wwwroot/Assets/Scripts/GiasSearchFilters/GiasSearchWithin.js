import throttle from "../GiasHelpers/throttle";
var suggestionTemplate =
  "<div class='filter-suggestion govuk-checkboxes__item'><input type='checkbox' class='js-filter-input filter-clone govuk-checkboxes__input' id='clone-{0}' {2}/>" +
  "<label for='clone-{0}' class='govuk-checkboxes__label govuk-label js-filter-label'>{1}</label></div>";

const defaults = {
  suggestionTemplate: suggestionTemplate
};
class GiasSearchWithin {
  constructor(el, opts) {
    this.el = el;
    this.opts = $.extend({}, defaults, opts);

    this.init()
  }

  init() {
    const $el = $(this.el);
    const opts = this.opts;
    const searchField = $el.find('.filter-search');
    const clearButton = $el.find('.field-clear');
    const originalFilters = $el.find('.js-filter-input');

    $el.find('.options-container').css({ minHeight: '200px' });

    const self = this;
    // helpers
    const renderSuggestions = function (options, userText) {
      let html = '';
      for (let i = 0, len = options.length; i < len; i++) {
        const opt = options[i];

        const optionText = opt.text.replace(new RegExp(userText, 'gi'), function(match) {
          return '<span>' + match + '</span>';
        });
        let suggestion = opts.suggestionTemplate.replace(/\{0}/g, opt.id).replace('{1}', optionText);
        if (opt.checked) {
          suggestion = suggestion.replace('{2}', 'checked="checked"');
        } else {
          suggestion = suggestion.replace('{2}', '');
        }

        html += suggestion;
      }
      $el.find('fieldset').addClass('hidden');
      $el.find('.suggestion-target').html('<div class="govuk-checkboxes gias-filter-checkboxes">'+html+'</div>');
      clearButton.removeClass('hidden');
    };

    const removeSuggestions = function () {
      $el.find('.suggestion-target').html('');
      $el.find('fieldset').removeClass('hidden');
      clearButton.addClass('hidden');
      searchField.val('');
    }

    this.regenerateItems();


    //attach events
    searchField.on('keyup', function () {
      if (this.value.trim().length > 1) {
        const searchVal = new RegExp(this.value, 'i');
        const options = self.dataList.filter(function (suggestion) {
          if (searchVal.test(suggestion.text)) {
            return suggestion;
          }
        });

        throttle(renderSuggestions(options, this.value), 200);
      } else if (this.value.trim().length === 0) {
        removeSuggestions();
      }
    });

    clearButton.on('click', function (e) {
      e.preventDefault();
      removeSuggestions();
    });

    //trigger click on the real filter
    $el.on('change', '.filter-clone', function (e) {
      var listId = this.id.replace('clone-', '');
      var realInput = $('#' + listId);
      var radioChecked = realInput.is(':checked');

      realInput.click();

      self.dataList.filter(function (item) {
        if (item.id === listId) {
          item.checked = !radioChecked;
        }
      });
    });

    // maintain state on the datalist when the original filters are updated
    originalFilters.on('change', function () {
      var radioChecked = $(this).is(':checked');
      var radioId = this.id;
      self.regenerateItems();
      self.dataList.filter(function (item) {
        if (item.id === radioId) {
          item.checked = radioChecked;
        }
      });
    });

  }

  regenerateItems() {
    const filterData = [];
    $(this.el).find('.govuk-checkboxes__item').each(function (n, elem) {
      var temp = {};
      temp.id = $(elem).find('.js-filter-input').prop('id');
      temp.text = $.trim($(elem).find('.js-filter-label').text());
      temp.checked = $(elem).find('.trigger-result-update').is(':checked');

      filterData.push(temp);

    });

    this.dataList = filterData;
  }
}

$.fn.giasSearchWithin = function (opts) {
  return this.each(function () {
    if (!$.data(this, 'giasSearchWithin')) {
      $.data(this, 'giasSearchWithin', new GiasSearchWithin(this, opts));
    }
  });
}

export default GiasSearchWithin;
