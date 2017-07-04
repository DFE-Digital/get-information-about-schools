// toggle panels from radios or checkboxes
// radios only allow one panel expanded at a time
// checkboxes can have mulitple open
// not currently tested with mixed radios/ checkboxes + don't feed after midnight

; (function ($) {
    'use strict';
    var defaults = {
        panelDataKey: 'togglePanel',
        untoggle: false,
        toggleCallBack: null
}

    function RadioToggle(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    RadioToggle.prototype = {
        init: function () {
            var $el = $(this.el),
                opts = this.opts,
                $radios = $el.find(':radio, :checkbox').filter('[data-toggle-panel]'),
                $panels = [],
                self = this;

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
                $radios.each(function() {
                    opts.toggleCallBack.call(self, this);
                });
            }


            $radios.on('change', function () {
                var radioButton = this;
                var isRadio = this.getAttribute('type') === 'radio';
                var panelClass = $(this).data()[opts.panelDataKey];

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
    };


    $.fn.radioToggle = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'radioToggle')) {
                $.data(this, 'radioToggle', new RadioToggle(this, opts));
            }
        });
    }


}($));

$('#content').find('.toggle-panels').radioToggle();