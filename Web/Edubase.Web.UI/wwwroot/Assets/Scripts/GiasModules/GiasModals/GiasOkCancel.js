import GiasModal from './GiasModal';

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

class GiasOkCancel extends GiasModal {
  constructor(el, opts) {
    opts = $.extend({}, defaults, opts);
    super(el, opts);
  }

  pause(pausedState) {
    this.opts.onPause = pausedState || false;
  }

  unbind() {
    $(this.el).off(this.opts.triggerEvent);
    $(this.el).removeData('okCancel');
  }

  updateModalContent(title, content) {
    if (title) {
      this.opts.title = title;
    }
    if (content) {
      this.opts.content = content;
    }
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
