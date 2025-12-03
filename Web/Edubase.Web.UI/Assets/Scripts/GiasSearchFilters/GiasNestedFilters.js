class GiasNestedFilters {
  constructor(el, opts) {
    const defaults = {
      parentControl: '.filter-group-title',
      expander: '.js-child-option-toggle',
      childItems: '.filter-group'
    };

    this.el = el;
    this.opts = $.extend({}, defaults, opts);
    this.init();
  }

  init() {
    const $el = $(this.el);
    const opts = this.opts;

    const parent = $el.find(opts.parentControl);
    const trigger = $el.find(opts.expander);
    const optionsPanel = $el.find('.filter-group');
    const childControls = optionsPanel.find('input');
    const $clearLinks = $('#filter-form').find('.filter-clear')
    const actionText = trigger.find('.filter-action-state');
    const checkedChildren = childControls.filter(function (n, elem) {
      return elem.checked;
    });

    const textFilters = $('#EditSearchCollapse').find('.age-filter .govuk-input, .date-filters .govuk-input');

    textFilters.on('blur', function () {
      const valueEntered = $(this).parents('.js-auto-height-inner').find('.govuk-input').filter(function(n, input){
        return input.value.length > 0
      }).length > 0;

      if (valueEntered) {
        $(this).parents('.govuk-option-select').find('.clear-selections').addClass('active-clear');
      } else {
        $(this).parents('.govuk-option-select').find('.clear-selections').removeClass('active-clear');
      }
    });

    textFilters.each(function (n, filter) {
      const filterHasValue = $(filter).parents('.js-auto-height-inner').find('.govuk-input').filter(function(n, input){
        return input.value.length > 0
      }).length > 0;

      if (filterHasValue) {
        $(this).parents('.govuk-option-select').find('.clear-selections').addClass('active-clear');
      }
    });

    $clearLinks.on('click', function (e) {
        e.preventDefault();
        if (!$(this).hasClass('clear-disabled')) {
          const $govUkSelect = $(this).parents('.govuk-option-select');
          if ($govUkSelect.hasClass('date-filters') || $govUkSelect.hasClass('age-filter')) {
            $govUkSelect.find('input[type="text"]').val('');
            $(this).removeClass('active-clear');
          }
        }
      });

    if (checkedChildren.length > 0) {
      parent.prop('checked', true);

      if (checkedChildren.length < childControls.length) {
        parent.next('label').addClass('partial-selection');
      }
    }

    optionsPanel.addClass('hidden');
    actionText.text('Expand ');

    trigger.on('click', function (e) {
      e.preventDefault();
      $(this).toggleClass('open-children');
      optionsPanel.toggleClass('hidden');
      $(this).attr('aria-expanded', $(this).hasClass('open-children'));

      actionText.text($(this).hasClass('open-children') ? 'Hide ' : 'Expand ');
    });


    parent.on('change', function (e) {
      const optSelect = $(this).parents('.govuk-option-select');
      optSelect.find('.clear-selections').removeClass('active-clear');

      $(this).next('label').removeClass('partial-selection');
      childControls.prop('checked', this.checked);

      childControls.change();

      var buf = [];
      optSelect.find('.trigger-result-update').filter(function (n, elem) {
        return elem.checked;
      }).each(function (n, elem) {
        if ($.inArray(elem.value, buf) === -1) {
          buf.push(elem.value);
        }
      });

      const count = buf.length;

      let checkedString = '';
      if (count > 0) {
        checkedString = count + ' selected';
        optSelect.find('.clear-selections').addClass('active-clear');
      }

      window.setTimeout(function () {
        optSelect.find('.js-selected-counter-text').text(checkedString);
      }, 0);
    });

    childControls.on('change', function (e) {
      const checkedCount = childControls.filter(function (n, ctrl) {
        return ctrl.checked;
      }).length;

      if (checkedCount < childControls.length && checkedCount > 0) {
        parent.next('label').addClass('partial-selection');
        parent.prop('checked', true);
      } else {
        parent.next('label').removeClass('partial-selection');
        if (checkedCount === 0) {
          parent.prop('checked', false);

        }
      }
    });
  }

  //Set visual state of nested filters without triggering a result update
  setPartialState() {
    const $el = $(this.el);
    const opts = this.opts;
    const childControls = $el.find('.filter-group input');
    const parent = $el.find(opts.parentControl);
    const checkedChildren = childControls.filter(function (n, elem) {
      return elem.checked;
    });

    parent.next('label').removeClass('partial-selection');

    if (checkedChildren.length === 0) {
      parent.prop('checked', false);
    } else if (checkedChildren.length < childControls.length) {
      parent.prop('checked', true);
      parent.next('label').addClass('partial-selection');
    } else if(checkedChildren.length === childControls.length) {
      parent.prop('checked', true);
    }
  }
}

$.fn.giasNestedFilters = function(opts) {
  return this.each(function() {
    if (!$.data(this, 'giasNestedFilters')) {
      $.data(this, 'giasNestedFilters', new GiasNestedFilters(this, opts));
    }
  });
}

export default GiasNestedFilters;
