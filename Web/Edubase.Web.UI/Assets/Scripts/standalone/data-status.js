DfE.Views.dataStatus = {
    options: {
        statusUrl: '/DataQuality/Status',
        editStatusUrl: '/DataQuality/Edit',
        viewStatusUrl: '/DataQuality',
        params: {},
        cookieExpiresIn: 0.08
    },
    exitOverlay: function (e) {
        e.preventDefault();

        document.body.removeChild(document.getElementById('data-status-summary'));
        document.body.removeChild(document.getElementById('modal-overlay'));

    },
    clearCookie: function() {
        GOVUK.setCookie('dataStatusViewed', '', { days: -1 });
        return true;
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
    checkPromptRequired: function () {
        var self = this;
        $.ajax({
            url: '/Api/should-show-data-status',
            dataType: 'json',
            success: function(data) {
                if (data.UserRequiresDataPrompt) {
                    self.init();
                }
            },
            error: function() {
                console.log('Error retrieving users data prompt status');
            }
        });
        
    },
    init: function() {
        var self = this;
        var mask = document.createElement('div');

        self.attachEvents();

        mask.id = 'modal-overlay';
        mask.classList.add('modal-overlay');
        document.body.appendChild(mask);

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
    DfE.Views.dataStatus.checkPromptRequired();
}
$('#logout-link').on('click', function (e) {
    e.preventDefault();
    var href = $(this).attr('href');
    DfE.Views.dataStatus.clearCookie();
    window.location = href;
});
