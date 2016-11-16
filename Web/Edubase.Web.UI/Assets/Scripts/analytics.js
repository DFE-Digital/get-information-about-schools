(function () {

    window.DfE.Util.Analytics = {
        isAvailable: function () {
            var retVal = window.ga && window.ga.hasOwnProperty('loaded') === true && window.ga.loaded === true;
            return retVal;
        },
        TrackClick: function (event) { // Tracks outbound links and element clicks when bound to "a,.js-track"
            if (DfE.Util.Analytics.isAvailable()) {
                var $element = $(this);
                var isHyperlinking = event.currentTarget && event.currentTarget.hostname;
                var isExternalLink = isHyperlinking && location.hostname != event.currentTarget.hostname;
                var isTargetBlank = $element.attr("target") == "_blank";
                var targetHref = isHyperlinking ? event.currentTarget.href : null;
                var navigateHitCallback = function () { document.location = targetHref; };
                var hasJsTrackCssClass = $(this).hasClass("js-track");

                var trackAction = isExternalLink || hasJsTrackCssClass;
                if (trackAction) {
                    var gaActionName = null, gaCategoryName = null, gaEventLabel = null;
                    if (isExternalLink && !hasJsTrackCssClass) {
                        gaActionName = "click";
                        gaCategoryName = "outbound";
                        gaEventLabel = targetHref;
                    }
                    else {
                        var trackData = $element.data("track"); // 'category|label|action'
                        if (trackData) {
                            var parts = trackData.split("|");
                            if (parts.length == 3) {
                                gaCategoryName = parts[0];
                                gaEventLabel = parts[1];
                                gaActionName = parts[2];

                                if (gaEventLabel == "[use-path]")
                                    gaEventLabel = event.currentTarget.pathname + event.currentTarget.search;
                            }
                        }
                    }

                    if (gaCategoryName && gaActionName) {
                        var gaHitCallback = null;
                        if (isHyperlinking && !isTargetBlank && !navigator.sendBeacon) {
                            gaHitCallback = navigateHitCallback;
                            event.preventDefault();
                        }
                        DfE.Util.Analytics.TrackEvent(gaCategoryName, gaEventLabel, gaActionName, gaHitCallback);
                    }
                }
            }
        },
        TrackEvent: function (category, label, action, callback) {
            if (DfE.Util.Analytics.isAvailable() && category && action) {
                category = category.trim();
                label = label ? label.trim() : null;
                action = action.trim();

                var isCallbackRequired = typeof (callback) === "function";
                var hasCallbackBeenCalled = false;
                var safeCallback = isCallbackRequired ? function () {
                    if (hasCallbackBeenCalled === true) return;
                    hasCallbackBeenCalled = true;
                    callback();
                } : null;

                try {
                    var payload = {
                        eventCategory: category,
                        eventAction: action,
                        eventLabel: label
                    };

                    if (isCallbackRequired) {
                        payload.transport = "beacon";
                        payload.hitCallback = safeCallback;
                        window.setTimeout(function () { // allow GA 1 second to call the callback, otherwise we do it ourselves
                            //console.log("Hit callback not invoked within 1 second; invoking failsafe");
                            safeCallback();
                        }, 1000);
                    }

                    ga('send', 'event', payload);
                    //console.log("Tracked event: cat: " + category + ", action: " + action + ", label: " + label);
                }
                catch (error) {
                    //console.log("TrackEvent error: " + error);
                }
            } else {
                //console.log("Unable to track event: cat: " + category + ", action: " + action);
            }
        },
        TrackPageView: function (path) {
            if (DfE.Util.Analytics.isAvailable() && path) {
                try {
                    ga('send', 'pageview', path);
                    //console.log("Tracked pageview: " + path);
                } catch (error) {
                    //console.log("TrackPageView error: " + error);
                }
            }
        }
    };

}())