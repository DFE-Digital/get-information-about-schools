const _assign = require('lodash.assign');
const _throttle = require('lodash.throttle');

const defaults = {
  maxLength: 1000,
  countTemplate: `<div class='textarea-counter govuk-hint'>Character limit: <span class='char-count'></span>/{0}</div>`
};

class GiasTextCounter {
  constructor(el, opts) {
    this.el = el;
    this.metaData = this.el.dataset;
    this.opts = _assign({}, defaults, this.metaData, opts)

    this.init();
  }

  init() {
    const self = this;

    this.el.insertAdjacentHTML('beforebegin', this.opts.countTemplate.replace('{0}', this.opts.maxLength));
    this.el.addEventListener('keyup', _throttle(this.setCount.bind(this), 100));
    this.el.addEventListener('blur', this.setCount.bind(this));
    this.el.addEventListener('paste',this.setCount.bind(this));


    $(this.el).on('drop', function() {
      window.setTimeout(function() {
        self.setCount();
      }, 0);
    });

    this.setCount();
  }

  setCount() {
    const cleanText =  this.el.value.replace(/([^\r]{0,1})\n/g, "$1\r\n").substring(0, this.opts.maxLength);
    this.el.value = cleanText;

    this.el.previousElementSibling.querySelector('.char-count').innerHTML = cleanText.length.toString();
  }
}

export default GiasTextCounter;
