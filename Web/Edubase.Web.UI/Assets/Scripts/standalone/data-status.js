﻿DfE.Views.dataStatus = {
    options: {
        statusUrl: '/Prototype/DataStatus',
        editStatusUrl: '/Prototype/EditDataStatus',
        viewStatusUrl: '/Prototype/ViewDataStatus',
        params: {},
        cookieExpiresIn: 1
    },
    exitOverlay: function (e) {
        e.preventDefault();

        document.body.removeChild(document.getElementById('data-status-summary'));
        document.body.removeChild(document.getElementById('modal-overlay'));

    },
    clearCookie: function() {
        // for testing/ dev only
        GOVUK.setCookie('dataStatusViewed', '', { days: -1 });
    },
    attachEvents: function() {
        var self = this;

        $(document).on('click', '#data-status-close, data-status-exit', self.exitOverlay);

        $(document).on('click', '#data-status-info', function() {
            window.location = self.options.viewStatusUrl;
        });

        $(document).on('click', '#data-status-update', function () {
            window.location = self.options.editStatusUrl;
        });
        
    },
    init: function() {
        var self = this;
        var mask = document.createElement('div');

        self.attachEvents();

        mask.id = 'modal-overlay';
        mask.classList.add('modal-overlay');
        document.body.appendChild(mask);

        if (DfE.Util.QueryString.get('warning') === 'true') {
            self.options.params.warning = true;
        }
        
        $.ajax({
            url: self.options.statusUrl,
            data: self.options.params,
            dataType: 'html',
            success: function (data) {
                var content = $(data).filter('#data-status-summary');
                $(document.body).append(content);
                GOVUK.setCookie('dataStatusViewed', 'true', { days: self.options.cookieExpiresIn });
            },
            error: function () {
                console.error('Error retrieving data status panel');
            }
        });

    }

};

if (GOVUK.cookie('dataStatusViewed') !== 'true') {
    DfE.Views.dataStatus.init();
}
