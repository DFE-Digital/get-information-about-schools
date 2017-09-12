DfE.Views.preventMassiveDownload = {
    $trigger : $('#content').find('.download-link'),
    okClick: function() {
        this.closeModal();
    },
    attachOkCancel: function () {
        var self = this;

        self.$trigger.okCancel({
            ok: self.okClick,
            cancel: null,
            title: 'Too many records',
            content: 'Please filter your search to fewer than 5000 governors. If you require a list of all governors you can download a full list from the <a href="/Downloads">downloads</a> page.',
            triggerEvent: 'click'
        });
    },
    init: function () {
        var self = this;
        var $trigger = self.$trigger;

        if ($trigger.hasClass('prevent-download')) {
            self.attachOkCancel();
        }

        $(window).on('ajaxResultLoad', function (e) {
            if (e.count <= 5000 && $trigger.data().hasOwnProperty('okCancel')) {
                $trigger.data().okCancel.unbind();
            }
            else if (e.count > 5000 && !$trigger.data().hasOwnProperty('okCancel')) {
              self.attachOkCancel();
            }
        });        
    }
}
if (document.getElementById('governors-search-results')) {
    DfE.Views.preventMassiveDownload.init();
}
