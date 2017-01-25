(function ($) {
    'use strict';
    
    function ClearField(el) {
        this.el = el;

        this.buttonTemplate = '<button class="field-clear hidden" title="clear field" id="{0}"></button>';
        this.init();
    }

    ClearField.prototype = {
        init: function() {
            var $field = $(this.el),
                buttonId =  $field.prop('id') + '-clear-button',
                buttonString = this.buttonTemplate.replace('{0}', buttonId);

            $field.wrap('<div class="clear-wrapper"></div>');

            var $button = $(buttonString).insertAfter($field);

            if ($field.val().length > 0) {
                $button.removeClass('hidden');
            }

            $(document).on('click', '.field-clear', function (e) {
                e.preventDefault();
                $field.val('');
                $button.addClass('hidden');
            });

            $field.on('change, keyup', function() {
                if (this.value.length > 0 && $button.hasClass('hidden')) {
                    $button.removeClass('hidden');
                }
            });
        }
    }

    $.fn.clearFields = function() {
        return this.each(function() {
            if (!$.data(this, 'clearField')) {
                $.data(this, 'clearField', new ClearField(this));
            }
        });
    }

}($));

if ($(window).width() < 640) {
    $('#content').find('.auto-clear').clearFields();
}

