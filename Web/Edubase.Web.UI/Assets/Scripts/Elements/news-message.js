DfE.Views.newsMessage = {
    closeButton: document.getElementById('news-dismiss'),
    delay: 1,
    newsBanner: document.getElementById('global-news-message'),

    init: function () {
        var self = this;
        this.closeButton.addEventListener('click',
            function(e) {
                e.preventDefault();
                GOVUK.setCookie('seen-news-banner', true, { days: self.delay });
                $(self.newsBanner).addClass('hidden');
            });

        if (GOVUK.getCookie('seen-news-banner') !== 'true') {
            $(this.newsBanner).removeClass('hidden');

        }
    }
    
};
DfE.Views.newsMessage.init();