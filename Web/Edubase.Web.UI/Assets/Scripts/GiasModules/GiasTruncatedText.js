class GiasTruncatedText {

  constructor(el, opts) {
    const defaults = {
      maxCharacters: 40,
      minScreenWidth: 640
    };
    this.el = el;
    this.opts = $.extend({}, defaults, opts);

    this.init();
  }

  init() {
    const $el = $(this.el);
    const opts = this.opts;
    const self = this;
    const tooltip = '<div id="edubase-tooltip" class="edubase-tooltip truncated-tooltip hidden" aria-hidden="true">' +
      '<span class="pointer-shadow"></span><span class="pointer"></span>' +
      '<div id="tooltip-content" class="tooltip-content" tabindex="-1"></div></div>';

    function showTooltip(text) {
      if (!$('#edubase-tooltip').hasClass('hidden')) {
        $(window).trigger('tooltipShift');
      }
      $('#tooltip-content').text(text);
      $('#edubase-tooltip')
        .removeClass('hidden')
        .css({
          display: 'block',
          left: '-1000em'
        });

      const width = $('#edubase-tooltip').outerWidth();


      $('#edubase-tooltip').css({
        display: 'block',
        top: $el.offset().top + $el.height() + 20 + 'px',
        left: $el.offset().left + $el.width() -width + 'px'
      });
    }

    function hideTooltip() {
      $('#edubase-tooltip').addClass('hidden');
    }

    const originalText = $.trim($el.text());

    this.clicks = 0;

    if ($(window).width() < opts.minScreenWidth) {
      return;
    }

    if (!document.getElementById('edubase-tooltip')) {
      $(document.body).append(tooltip);
    }

    if (originalText.length <= opts.maxCharacters) {
      return;
    } else {
      $el.originalText = originalText;
      $el.addClass('truncated-content');
      $el.text(originalText.substring(0, opts.maxCharacters) + '...');
    }

    $(window).on('tooltipShift', function () {
      self.clicks = 0;
    });

    $el.on('click', function () {
      if (self.clicks % 2 === 0) {
        showTooltip($el.originalText);
        self.clicks++;
      } else {
        hideTooltip();
        self.clicks = 0;
      }
    });

    $('.tooltip-close').on('click', function () {
      hideTooltip();
    });
  }

  resetClickCount() {
    this.clicks = 0;
  }
}

$.fn.giasTruncate = function (options) {
  return this.each(function () {
    if (!$.data(this, 'giasTruncate')) {
      $.data(this, 'truncate', new GiasTruncatedText(this, options));
    }
  });
}

export default GiasTruncatedText;
