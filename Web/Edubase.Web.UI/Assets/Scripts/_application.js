(function () {
    'use strict';

    if (typeof window.GOVUK === 'undefined') { window.GOVUK = {}; }
    if (typeof window.GOVUK.support === 'undefined') { window.GOVUK.support = {}; }

    window.GOVUK.support.history = function () {
        return window.history && window.history.pushState && window.history.replaceState;
    };

    // Declares the main application library
    if (typeof window.DfE === 'undefined') {
        window.DfE = {
            Views: {},
            Elements: {},
            Util: { Analytics: {} },
            mapConfig: {}
        };
    }

    window.DfE.mapConfig.apiKey = azureMapsApiKey;
    window.DfE.Util.randomNumber = function () { return Math.floor((Math.random() * 10000000) + 1); };

    window.DfE.Util.QueryString = {
        get: function (name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)", "i"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }
    };

    $(function () {
        $(document).on("click", "a,.js-track", window.DfE.Util.Analytics.TrackClick);
    });

}());
