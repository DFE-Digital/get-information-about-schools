class GiasRadioToggles {
  constructor(el, opts) {
    this.el = el;
    this.opts = $.extend({},
      {
        panelDataKey: 'togglePanel',
        untoggle: false,
        toggleCallBack: null
      }, opts);

    this.init();
  }

  init() {
    const opts = this.opts;
    const $el = $(this.el);
    const $radios = $el.find(':radio, :checkbox').filter('[data-toggle-panel]');
    let $panels = [];
    const self = this;

    $radios.each(function (n) {
      if ($(this).data() && $(this).data().hasOwnProperty(opts.panelDataKey)) {
        $panels.push($(this).data()[opts.panelDataKey]);
      } else {
        $radios.splice(n, 1);
      }
    });

    $panels = $($panels.join(','));

    $panels.addClass('hidden');

    if ($radios.filter(':checked').length > 0) {
      $($radios.filter(':checked').data()[opts.panelDataKey]).removeClass('hidden');
    }

    if ($.isFunction(opts.toggleCallBack)) {
      $radios.each(function () {
        opts.toggleCallBack.call(self, this);
      });
    }

    $radios.on('change', function () {
      const radioButton = this;
      const isRadio = this.getAttribute('type') === 'radio';
      const panelClass = $(this).data()[opts.panelDataKey];

      $(window).trigger({
        type: 'radioChange',
        selectedRadio: radioButton
      });

      if (isRadio) {
       $panels.addClass('hidden')
          .attr('aria-hidden', 'true');

        $(panelClass)
          .removeClass('hidden')
          .attr('aria-hidden', 'false');
      }
      else {
        $(panelClass).toggleClass('hidden');

        if ($(panelClass).is(':visible')) {
          $(panelClass).attr('aria-hidden', 'false');
        } else {
          $(panelClass).attr('aria-hidden', 'true');
        }
      }

      if ($.isFunction(opts.toggleCallBack)) {
        opts.toggleCallBack.call(self, radioButton);
      }
    });

    if (opts.untoggle) {
      $(window).on('radioChange', function (e) {
        $radios.prop('checked', false);
        $panels.addClass('hidden');
        $(e.selectedRadio).prop('checked', true);
      });
    }
  }
}
$.fn.giasRadioToggles = function (opts) {
  return this.each(function () {
    if (!$.data(this, 'giasRadioToggles')) {
      $.data(this, 'giasRadioToggles', new GiasRadioToggles(this, opts));
    }
  });
};

export default GiasRadioToggles;
