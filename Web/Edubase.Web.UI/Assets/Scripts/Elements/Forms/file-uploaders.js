(function($) {
    'use strict';

    var defaults = {}

    function Uploader(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }

    Uploader.prototype = {
        init: function() {
            console.log("init");
            var $el = $(this.el),
                opts = this.opts;


            var $fileInput = $el.find('input[type="file"]'),
                $displayBox = $el.find('.file-name'),
                $button = $el.find('.file-selector');


            $fileInput.on('change', function () {
                var fileName = $(this).val().split('\\').slice(-1);
                $displayBox.val(fileName);
            });


            $button.on('click', function (e) {
                e.preventDefault();
                $fileInput.click();
            });
        }
    }


    $.fn.uploadControl = function(opts) {
        return this.each(function() {
            if (!$.data(this, 'uploadControl')) {
                $.data(this, 'uploadControl', new Uploader(this, opts));
            }
        });
    }
}($));


$('#content').find('.upload-control-wrapper').uploadControl();