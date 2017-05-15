
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
        immediate: false
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
        init: function () {
            var opts = this.opts;
            var $el = $(this.el);
            var self = this;
            var overlay = '<div id="modal-overlay" class="modal-overlay hidden"></div>' +
                '<div id="modal-content" class="modal-content hidden" role="dialog"><a href="#" id="exit-overlay" class="modal-exit">Close</a><div id="modal-inner">' +
                '<h3 class="heading-large" id="modal-title">'+ opts.title + '</h3><p id="modal-content-area">'+ opts.content + '</p></div> ' +
                '<div class="button-row"><a href="#" class="button mobile-full-width" id="button-ok">OK</a>';
            
            if ($.isFunction(opts.cancel)) {
                overlay += '<a href="#" class="button button-grey mobile-full-width" id="button-cancel">Cancel</a>';
            } 
            overlay += '</div></div>';


            if ($('#modal-overlay').length === 0) {
                $('#full-content').append(overlay);
            }
            
            function displayModal(e) {
                e.preventDefault();
                if (!opts.onPause) {
                    self.showModal();
                }
            }
            
            $el.on(opts.triggerEvent, displayModal);


            $('#exit-overlay , #modal-overlay').on('click', function (e) {
                e.preventDefault();
                self.closeModal();
            });

            if (opts.immediate) {
                self.showModal();
                $el.off(opts.triggerEvent, displayModal);
            }
        },
        closeModal: function () {
            unbindEscapeKey();
            $('#full-content').off('click', '#button-ok');
            $('#full-content').off('click', '#button-cancel');
            
            $('#modal-content , #modal-overlay').addClass('hidden');

        },
        updateModalContent: function (title, content) {
            if (title) {
               $('#modal-title').text(title); 
            }
            if (content) {
                $('#modal-content-area').text(content);
            }
        },
        showModal: function () {
            var opts = this.opts;
            var self = this;

            bindEscapeKey.call(self);
            $('#modal-overlay, #modal-content').removeClass('hidden');

            var modalChildren = $('#modal-inner').children();
            var description = $('#modal-inner').find(':header').slice(0, 1);

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

                $('#modal-content').attr({ 'aria-labelledby': labelId, 'aria-describedby': descId });
            }

            $('#modal-label').focus();

            $('#full-content').on('click', '#button-ok', function (e) {
                e.preventDefault();
                opts.ok.call(self);
                self.closeModal();
            });

            $('#full-content').on('click', '#button-cancel', function (e) {
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

