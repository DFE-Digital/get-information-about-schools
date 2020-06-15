const defaults = {
  ok: function() {
    alert('OK!');
  },
  cancel: function() {
    this.closeModal();
  },
  title: 'Are you sure...',
  content: 'That you want to perform this action?',
  triggerEvent: 'change',
  onPause: false,
  immediate: false,
  idPrefix: '',
  okLabel: 'OK',
};

class GiasOkCancel {
  constructor(el, opts) {
    this.el = el;
    this.opts = $.extend({}, defaults, opts);

    this.init();
  }

  pause(pausedState) {
    this.opts.onPause = pausedState || false;
  }

  unbind() {
    $(this.el).off(this.opts.triggerEvent);
    $(this.el).removeData('okCancel');
  }

  bindEscapeKey() {
    var self = this;
    $(document).on('keyup', function (e) {
      e = e || window.event;
      if (e.keyCode === 27) {
        self.closeModal();
      }
    });
  }

  unbindEscapeKey() {
    $(document).off('keyup');
  }

  init() {
    const opts = this.opts;
    const $el = $(this.el);
    const self = this;
    let overlay = '<div id="'+opts.idPrefix+'modal-overlay" class="modal-overlay hidden"></div>' +
      '<div id="' + opts.idPrefix + 'modal-content" class="modal-content hidden" role="dialog"><a href="#" id="' + opts.idPrefix + 'exit-overlay" class="modal-exit">Close</a>' +
      '<div id="' + opts.idPrefix + 'modal-inner" class="modal-inner">' +
      '<h3 class="govuk-heading-l" id="' + opts.idPrefix + 'modal-title">' + opts.title + '</h3><p id="' + opts.idPrefix + 'modal-content-area" class="govuk-body">' + opts.content + '</p></div> ' +
      '<div class="button-row"><a href="#" class="govuk-button js-allow-exit" id="' + opts.idPrefix + 'button-ok">' + (!opts.okLabel ? "OK" : opts.okLabel) + '</a>';

    if ($.isFunction(opts.cancel)) {
      overlay += '<a href="#" class="govuk-button govuk-button--secondary js-allow-exit" id="' + opts.idPrefix + 'button-cancel">Cancel</a>';
    }
    overlay += '</div></div>';


    if ($('#' + opts.idPrefix + 'modal-overlay').length === 0) {
      $('#app').append(overlay);
    }

    function displayModal(e) {
      e.preventDefault();
      if (!opts.onPause) {
        self.showModal();
      }
    }

    $el.on(opts.triggerEvent, displayModal);


    $('#' + opts.idPrefix + 'exit-overlay , #' + opts.idPrefix + 'modal-overlay').on('click', function (e) {
      e.preventDefault();
      self.closeModal();
    });

    if (opts.immediate) {
      self.showModal();
      $el.off(opts.triggerEvent, displayModal);
    }
  }
  closeModal() {
    const opts = this.opts;
    this.unbindEscapeKey();
    $('#app').off('click', '#' + opts.idPrefix + 'button-ok');
    $('#app').off('click', '#' + opts.idPrefix + 'button-cancel');

    $('#' + opts.idPrefix + 'modal-content , #' + opts.idPrefix + 'modal-overlay').addClass('hidden');

  }
  updateModalContent(title, content) {
    if (title) {
      $('#' + this.opts.idPrefix + 'modal-title').text(title);
    }
    if (content) {
      $('#' + this.opts.idPrefix + 'modal-content-area').html(content);
    }
  }

  showModal() {
    const opts = this.opts;
    const self = this;

    this.bindEscapeKey();

    $('#' + opts.idPrefix + 'modal-overlay, #' + opts.idPrefix + 'modal-content').removeClass('hidden');

    const modalChildren = $('#'+ opts.idPrefix +'modal-inner').children();
    const description = $('#' + opts.idPrefix + 'modal-inner').find(':header').slice(0, 1);

    modalChildren.attr('tabindex', 0);

    if (description.length > 0) {
      var descId = 'modal-desc';
      var labelId = 'modal-label';
      if (!description[0].hasAttribute('id')) {
        description.attr('id', 'modal-label');

      } else {
        labelId = description.attr('id');
      }

      if (!description.next()[0].hasAttribute('id')) {
        description.next().attr('id', 'model-desc');
      } else {
        descId = description.next().attr('id');
      }

      $('#' + opts.idPrefix + 'modal-content').attr({ 'aria-labelledby': labelId, 'aria-describedby': descId });
    }

    // Focus on main CTA when modal available
    if ($('#' + opts.idPrefix + 'modal-content').length) {
      $('#' + opts.idPrefix + 'button-ok').focus();
    }

    $('#app').on('click', '#' + opts.idPrefix + 'button-ok', function (e) {
      e.preventDefault();
      opts.ok.call(self);
      self.closeModal();
    });

    $('#app').on('click', '#' + opts.idPrefix + 'button-cancel', function (e) {
      e.preventDefault();
      opts.cancel.call(self);
      self.closeModal();
    });
  }
}
$.fn.okCancel = function(opts) {
  return this.each(function() {
    if (!$.data(this, 'okCancel')) {
      $.data(this, 'okCancel', new GiasOkCancel(this, opts));
    }
  });
};

export default GiasOkCancel;
