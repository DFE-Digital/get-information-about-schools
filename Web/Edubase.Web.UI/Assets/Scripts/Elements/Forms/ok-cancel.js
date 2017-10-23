
(function($) {
    var defaults = {
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
        okLabel: 'OK'
    };


    var OkCancel = function(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);
        this.init();

    }


    function bindEscapeKey() {
        var self = this;
        $(document).on('keypress', function (e) {
            e = e || window.event;
            if (e.keyCode === 27) {
                self.closeModal();
            }
        });
    }

    function unbindEscapeKey() {
        $(document).off('keypress');
    }

    

    OkCancel.prototype = {
        pause: function (pausedState) {
            this.opts.onPause = pausedState || false;
        },
        unbind: function() {
            $(this.el).off(this.opts.triggerEvent);
            $(this.el).removeData('okCancel');
        },
        init: function () {
            var opts = this.opts;
            var $el = $(this.el);
            var self = this;
            var overlay = '<div id="'+opts.idPrefix+'modal-overlay" class="modal-overlay hidden"></div>' +
                '<div id="' + opts.idPrefix + 'modal-content" class="modal-content hidden" role="dialog"><a href="#" id="' + opts.idPrefix + 'exit-overlay" class="modal-exit">Close</a>' +
                '<div id="' + opts.idPrefix + 'modal-inner" class="modal-inner">' +
                '<h3 class="heading-large" id="' + opts.idPrefix + 'modal-title">' + opts.title + '</h3><p id="' + opts.idPrefix + 'modal-content-area">' + opts.content + '</p></div> ' +
                '<div class="button-row"><a href="#" class="button mobile-full-width allow-exit" id="' + opts.idPrefix + 'button-ok">' + (!opts.okLabel ? "OK" : opts.okLabel) + '</a>';
            
            if ($.isFunction(opts.cancel)) {
                overlay += '<a href="#" class="button button-grey mobile-full-width allow-exit" id="' + opts.idPrefix + 'button-cancel">Cancel</a>';
            } 
            overlay += '</div></div>';


            if ($('#' + opts.idPrefix + 'modal-overlay').length === 0) {
                $('#full-content').append(overlay);
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
        },
        closeModal: function () {
            var opts = this.opts;
            unbindEscapeKey();
            $('#full-content').off('click', '#' + opts.idPrefix + 'button-ok');
            $('#full-content').off('click', '#' + opts.idPrefix + 'button-cancel');
            
            $('#' + opts.idPrefix + 'modal-content , #' + opts.idPrefix + 'modal-overlay').addClass('hidden');

        },
        updateModalContent: function (title, content) {
            
            if (title) {
                $('#' + this.opts.idPrefix + 'modal-title').text(title);
            }
            if (content) {
                $('#' + this.opts.idPrefix + 'modal-content-area').html(content);
            }
        },
        showModal: function () {
            var opts = this.opts;
            var self = this;

            bindEscapeKey.call(self);
            $('#' + opts.idPrefix + 'modal-overlay, #' + opts.idPrefix + 'modal-content').removeClass('hidden');

            var modalChildren = $('#'+ opts.idPrefix +'modal-inner').children();
            var description = $('#' + opts.idPrefix + 'modal-inner').find(':header').slice(0, 1);

            modalChildren.attr('tabindex', 0);

            if (description.length > 0) {
                var descId = 'modal-desc';
                var labelId = 'modal-label';
                if (!description[0].hasAttribute('id')) {
                    description.attr('id', 'modal-label');

                } else {
                    descId = description.attr('id');
                }

                if (!description.next()[0].hasAttribute('id')) {
                    description.next().attr('id', 'model-desc');
                } else {
                    labelId = description.next().attr('id');
                }

                $('#' + opts.idPrefix + 'modal-content').attr({ 'aria-labelledby': labelId, 'aria-describedby': descId });
            }

            $('#' + opts.idPrefix + 'modal-label').focus();

            $('#full-content').on('click', '#' + opts.idPrefix + 'button-ok', function (e) {
                e.preventDefault();
                opts.ok.call(self);
                self.closeModal();
            });

            $('#full-content').on('click', '#' + opts.idPrefix + 'button-cancel', function (e) {
                e.preventDefault();
                opts.cancel.call(self);
                self.closeModal();
            });
        }

    };

    $.fn.okCancel = function(opts) {
        return this.each(function() {
            if (!$.data(this, 'okCancel')) {
                $.data(this, 'okCancel', new OkCancel(this, opts));
            }
        });
    }

}($));

