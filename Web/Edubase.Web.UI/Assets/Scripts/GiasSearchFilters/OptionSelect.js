class OptionSelect {
  constructor(el, opts) {
    this.el = el;
    this.$options = $(this.el).find(".govuk-checkboxes__input").not('.filter-group-title');

    this.opts = $.extend({},  opts);

    this.setUp();
  }

  bindUpdateSelectedCount() {
    this.$options.on('change', ()=> {
      const count = this.$options.filter(':checked').length;
      const countTarget = $(this.el).find('.js-selected-counter-text');
      const countStr = count === 0 ? '' : count + ' selected';

      countTarget.html(countStr);
    });
  }

  setUp() {
    this.isOpen = !$(this.el).data().closedOnLoad;
    const containerHead = $(this.el).find('.js-container-head');
    const $button = $(`<button class="js-container-head" aria-expanded="${!this.isOpen}" aria-controls="${this.el.id}">${containerHead.html()}</button>`);

    containerHead.replaceWith($button);
    this.init();
  }

  openFilters() {
    $(this.el).find('.js-container-head').attr('aria-expanded', true);
    $(this.el).removeClass('js-closed');
    this.isOpen = true;
  }

  closeFilters() {
    $(this.el).find('.js-container-head').attr('aria-expanded', true);
    $(this.el).addClass('js-closed');
    this.isOpen = false;
  }

  toggleFilterDisplay(e) {
    e.preventDefault();

    this.isOpen ? this.closeFilters() : this.openFilters();
  }

  init() {
    const count = this.$options.filter(':checked').length;
    if (count > 0){
      $(this.el).find('.js-selected-counter-text').html(count + ' selected');
    }

    if (!this.isOpen) {
      this.closeFilters()
    }
    this.bindUpdateSelectedCount();
    $(this.el).find('.js-container-head').on('click', this.toggleFilterDisplay.bind(this));
  }
}

$.fn.optionSelect = function (opts) {
  return this.each(function () {
    if (!$.data(this, 'optionSelect')) {
      $.data(this, 'optionSelect', new OptionSelect(this, opts));
    }
  });
}

export default OptionSelect;
