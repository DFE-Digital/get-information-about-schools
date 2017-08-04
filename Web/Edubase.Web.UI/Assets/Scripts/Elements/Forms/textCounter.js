(function ($) {
    var defaults = {
        maxLength: 1000,
        countTemplate: '<div class="textarea-counter form-hint">Character limit: <span class="char-count"></span>/{0}</div>'
    };

    function TextCounter(el, opts) {
        this.el = el;
        this.metaData = $(el).data();
        this.opts = $.extend({}, defaults, this.metaData, opts);

        this.init();
    }

    function throttle(func, rate) {
        var counter = 0;
        return function () {
            var context = this;
            var args = arguments;
            counter++;
            if (counter === rate) {
                func.apply(context, args);
                counter = 0;
            }
        };
    }

    TextCounter.prototype = {
        setCount: function () {
            var elemText = this.el.value,
                cleanText = elemText.replace(/([^\r]{0,1})\n/g, "$1\r\n").substring(0, this.opts.maxLength);

            this.el.value = cleanText;
            $(this.el).siblings('.textarea-counter').find('.char-count').text(cleanText.length);
        },
        init: function () {
            var $el = $(this.el);
            var opts = this.opts;
            var self = this;

            this.tmpl = opts.countTemplate.replace('{0}', opts.maxLength);

            $(this.tmpl).insertBefore($el);

            $el.on("keyup blur paste", function () {
                throttle(self.setCount(), 100);
            });

            this.setCount();
        }
    };

    $.fn.textCount = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'textCount')) {
                $.data(this, 'textCount', new TextCounter(this, opts));
            }
        });
    }
}($));