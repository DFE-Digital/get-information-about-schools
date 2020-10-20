class GiasSelectAll {
  constructor(el, opts) {
    this.el  = el;
    this.opts = opts;

    this.init();
  }

  init() {
    const $el = $(this.el);
    const $childOptions = $el.parents('fieldset').find('.trigger-result-update');
    const self = this;

    this.childOptionCount = $childOptions.length;

    $el.on('change', function() {
      if (this.checked) {
        if ($el.parents('.govuk-option-select').hasClass('nested-filter-options')) {
          $el.parents('fieldset').find('.filter-group-title').prop('checked', true);
        }
        $childOptions.prop('checked', true);

      } else {
        if ($el.parents('.govuk-option-select').hasClass('nested-filter-options')) {
          $el.parents('fieldset').find('.filter-group-title').prop('checked', false);
        }
        $childOptions.prop('checked', false);
      }
      $childOptions.slice(0, 1).trigger('change');
    });

    $childOptions.on('change', function() {
      const count = $childOptions.filter(function(n, input) {
        return input.checked;
      }).length;

      if (count === self.childOptionCount || count === 0) {
        $el.next('.js-filter-label').removeClass('partial-selection');
      } else {
        $el.next('.js-filter-label').addClass('partial-selection');
      }
    });

  }
}

$.fn.giasSelectAll = function (opts) {
  return this.each(function () {
    if (!$.data(this, 'giasSelectAll')) {
      $.data(this, 'giasSelectAll', new GiasSelectAll(this, opts));
    }
  });
}

export default GiasSelectAll;
