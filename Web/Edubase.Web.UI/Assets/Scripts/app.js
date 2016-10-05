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
            Util: { Analytics: {} }
        };
    }
}());