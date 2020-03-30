const defaults = {
};

class GiasToolTip {
  constructor(el, opts) {
    this.el = el;
    this.opts = opts;

    if (!document.getElementById('edubase-tooltip')) {
      const tooltip = '<div id="edubase-tooltip" class="edubase-tooltip hidden" aria-hidden="true">' +
        '<span class="pointer-shadow"></span><span class="pointer"></span>' +
        '<div id="tooltip-content" class="tooltip-content" tabindex="0"></div>' +
        '<a href="#" class="tooltip-close" title="close tool tip" role="button"><span class="govuk-visually-hidden">Close</span></a></div>';

      $('#app').after(tooltip);
    }
    this.$toolTip = $('#edubase-tooltip');
    this.init()
  }

  init() {
    const el = this.el;
    const opts = this.opts;
    const self = this;

    $(el).on('click', (e)=> {
      e.preventDefault();
      this.openToolTip();
      $(el).addClass('tooltip-opener');
    });

    this.closeFn = (e)=>{
      e.preventDefault();
      self.closeToolTip();
      $('.tooltip-opener').focus();
      $(el).removeClass('tooltip-opener');
    };

    this.$toolTip.find('.tooltip-close').on('click', (e)=> {
      e.preventDefault();
      this.$toolTip.addClass('hidden').attr('aria-hidden', true);
      $(document).off('click', '#app', this.closeFn);
      $('.tooltip-opener').focus();
      $(el).removeClass('tooltip-opener');
    });
  }

  openToolTip() {
    const self = this;
    const contentTarget = $(this.el).attr('href');
    const tooltipContent = $(contentTarget).html();
    const $toolContent = $('#tooltip-content');
    $toolContent.html(tooltipContent).focus();

    this.positionToolTip();
    this.$toolTip.removeClass('hidden').attr('aria-hidden', false);

    $toolContent.focus();

    window.setTimeout(function(){
      $(document).on('click', '#app', self.closeFn);
    }, 20);

  }

  closeToolTip() {
    this.$toolTip.addClass('hidden').attr('aria-hidden', true);
    $(document).off('click', '#app', this.closeFn);
  }

  positionToolTip() {
    const el = this.el;
    const opts = this.opts;
    const self = this;
    const winWidth = $(window).width();
    const offset = $(el).offset();
    const toolTipWidth = (winWidth < 480) ? 310: 480;
    let leftPos;


    if (offset.left + toolTipWidth > winWidth) {
      leftPos = winWidth - toolTipWidth - 10;

      self.$toolTip.css({ top: offset.top + 40 + 'px', left: leftPos + 'px' });

      self.$toolTip.find('.pointer, .pointer-shadow')
        .css({
          left: -1 * (parseInt(self.$toolTip.css('left'), 10) - offset.left + 7) + 'px'
        });

    } else {
      self.$toolTip.css({ top: offset.top + 40 + 'px', left: offset.left - 26 + 'px' });

      self.$toolTip.find('.pointer, .pointer-shadow').removeAttr('style');

    }
  }
}

$.fn.giasToolTip = function(opts) {
  return this.each(function() {
    if (!$.data(this, 'giasToolTip')) {
      $.data(this, 'giasToolTip', new GiasToolTip(this, opts));
    }
  });
};

export default GiasToolTip;
