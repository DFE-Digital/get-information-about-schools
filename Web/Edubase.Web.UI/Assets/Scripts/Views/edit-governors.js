DfE.Views.governors = {
    radioToggleCallback: function (radioButton) {
        if ($(radioButton).is(':checked')) {
            if (radioButton.getAttribute('type') === 'radio') {
                $('.shared-governor').removeClass('panel-indent');
            }

            $($(radioButton).data()[this.opts.panelDataKey]).parent('.shared-governor').addClass('panel-indent');

        } else {
            $($(radioButton).data()[this.opts.panelDataKey]).parent('.shared-governor').removeClass('panel-indent');
        }

    },
    init: function() {
        $('#content').find('.js-governor-toggles')
            .radioToggle({
                toggleCallBack: DfE.Views.governors.radioToggleCallback
            });
    }
};

DfE.Views.governors.init();
