var GOVUK = GOVUK || {};

GOVUK.performance = GOVUK.performance || {};

GOVUK.performance.stageprompt = function() {
    var setup, setupForGoogleAnalytics, splitAction;
    splitAction = function(action) {
        var parts = action.split(":");
        if (parts.length <= 3) return parts;
        return [ parts.shift(), parts.shift(), parts.join(":") ];
    };
    setup = function(analyticsCallback) {
        var journeyStage = $("[data-journey]").attr("data-journey"), journeyHelpers = $("[data-journey-click]");
        if (journeyStage) {
            analyticsCallback.apply(null, splitAction(journeyStage));
        }
        journeyHelpers.on("click", function(event) {
            analyticsCallback.apply(null, splitAction($(this).data("journey-click")));
        });
    };
    setupForGoogleAnalytics = function() {
        setup(GOVUK.performance.sendGoogleAnalyticsEvent);
    };
    return {
        setup: setup,
        setupForGoogleAnalytics: setupForGoogleAnalytics
    };
}();

GOVUK.performance.sendGoogleAnalyticsEvent = function(category, event, label) {
    _gaq.push([ "_trackEvent", category, event, label, undefined, true ]);
};

(function() {
    "use strict";
    window.GOVUK = window.GOVUK || {};
    var $ = window.$;
    function MultivariateTest(options) {
        this.$el = $(options.el);
        this._loadOption(options, "name");
        this._loadOption(options, "customDimensionIndex", null);
        this._loadOption(options, "cohorts");
        this._loadOption(options, "runImmediately", true);
        this._loadOption(options, "defaultWeight", 1);
        this._loadOption(options, "contentExperimentId", null);
        if (this.runImmediately) {
            this.run();
        }
    }
    MultivariateTest.prototype._loadOption = function(options, key, defaultValue) {
        if (options[key] !== undefined) {
            this[key] = options[key];
        }
        if (this[key] === undefined) {
            if (defaultValue === undefined) {
                throw new Error(key + " option is required for a multivariate test");
            } else {
                this[key] = defaultValue;
            }
        }
    };
    MultivariateTest.prototype.run = function() {
        var cohort = this.getCohort();
        if (cohort) {
            this.setUpContentExperiment(cohort);
            this.setCustomVar(cohort);
            this.executeCohort(cohort);
            this.createDummyEvent(cohort);
        }
    };
    MultivariateTest.prototype.executeCohort = function(cohort) {
        var cohortObj = this.cohorts[cohort];
        if (cohortObj.callback) {
            if (typeof cohortObj.callback === "string") {
                this[cohortObj.callback]();
            } else {
                cohortObj.callback();
            }
        }
        if (cohortObj.html) {
            this.$el.html(cohortObj.html);
            this.$el.show();
        }
    };
    MultivariateTest.prototype.getCohort = function() {
        var cohort = GOVUK.cookie(this.cookieName());
        if (!cohort || !this.cohorts[cohort]) {
            cohort = this.chooseRandomCohort();
            GOVUK.cookie(this.cookieName(), cohort, {
                days: 30
            });
        }
        return cohort;
    };
    MultivariateTest.prototype.setCustomVar = function(cohort) {
        if (this.customDimensionIndex) {
            GOVUK.analytics.setDimension(this.customDimensionIndex, this.cookieName(), cohort, 2);
        }
    };
    MultivariateTest.prototype.setUpContentExperiment = function(cohort) {
        var contentExperimentId = this.contentExperimentId;
        var cohortVariantId = this.cohorts[cohort]["variantId"];
        if (typeof contentExperimentId !== "undefined" && typeof cohortVariantId !== "undefined" && typeof window.ga === "function") {
            window.ga("set", "expId", contentExperimentId);
            window.ga("set", "expVar", cohortVariantId);
        }
    };
    MultivariateTest.prototype.createDummyEvent = function(cohort) {
        GOVUK.analytics.trackEvent(this.cookieName(), "run", {
            nonInteraction: true
        });
    };
    MultivariateTest.prototype.weightedCohortNames = function() {
        var names = [], defaultWeight = this.defaultWeight;
        $.each(this.cohorts, function(key, cohortSettings) {
            var numberForCohort, i;
            if (typeof cohortSettings.weight === "undefined") {
                numberForCohort = defaultWeight;
            } else {
                numberForCohort = cohortSettings.weight;
            }
            for (i = 0; i < numberForCohort; i++) {
                names.push(key);
            }
        });
        return names;
    };
    MultivariateTest.prototype.chooseRandomCohort = function() {
        var names = this.weightedCohortNames();
        return names[Math.floor(Math.random() * names.length)];
    };
    MultivariateTest.prototype.cookieName = function() {
        return "multivariatetest_cohort_" + this.name;
    };
    window.GOVUK.MultivariateTest = MultivariateTest;
})();

(function() {
    "use strict";
    window.GOVUK = window.GOVUK || {};
    var PrimaryList = function(el, selector) {
        this.$el = $(el);
        this.$extraLinks = this.$el.find("li:not(" + selector + ")");
        if (this.$extraLinks.length > 1) {
            this.addToggleLink();
            this.hideExtraLinks();
        }
    };
    PrimaryList.prototype = {
        toggleText: function() {
            if (this.$extraLinks.length > 1) {
                return "+" + this.$extraLinks.length + " others";
            } else {
                return "+" + this.$extraLinks.length + " other";
            }
        },
        addToggleLink: function() {
            this.$toggleLink = $('<a href="#">' + this.toggleText() + "</a>");
            this.$toggleLink.click($.proxy(this.toggleLinks, this));
            this.$toggleLink.insertAfter(this.$el);
        },
        toggleLinks: function(e) {
            e.preventDefault();
            this.$toggleLink.remove();
            this.showExtraLinks();
        },
        hideExtraLinks: function() {
            this.$extraLinks.addClass("visuallyhidden");
            $(window).trigger("govuk.pageSizeChanged");
        },
        showExtraLinks: function() {
            this.$extraLinks.removeClass("visuallyhidden");
            $(window).trigger("govuk.pageSizeChanged");
        }
    };
    GOVUK.PrimaryList = PrimaryList;
    GOVUK.primaryLinks = {
        init: function(selector) {
            $(selector).parent().each(function(i, el) {
                new GOVUK.PrimaryList(el, selector);
            });
        }
    };
})();

(function() {
    "use strict";
    var root = this, $ = root.jQuery;
    if (typeof GOVUK === "undefined") {
        root.GOVUK = {};
    }
    var SelectionButtons = function(elmsOrSelector, opts) {
        var $elms;
        this.selectedClass = "selected";
        this.focusedClass = "focused";
        if (opts !== undefined) {
            $.each(opts, function(optionName, optionObj) {
                this[optionName] = optionObj;
            }.bind(this));
        }
        if (typeof elmsOrSelector === "string") {
            $elms = $(elmsOrSelector);
            this.selector = elmsOrSelector;
            this.setInitialState($(this.selector));
        } else {
            this.$elms = elmsOrSelector;
            this.setInitialState(this.$elms);
        }
        this.addEvents();
    };
    SelectionButtons.prototype.addEvents = function() {
        if (typeof this.$elms !== "undefined") {
            this.addElementLevelEvents();
        } else {
            this.addDocumentLevelEvents();
        }
    };
    SelectionButtons.prototype.setInitialState = function($elms) {
        $elms.each(function(idx, elm) {
            var $elm = $(elm);
            if ($elm.is(":checked")) {
                this.markSelected($elm);
            }
        }.bind(this));
    };
    SelectionButtons.prototype.markFocused = function($elm, state) {
        if (state === "focused") {
            $elm.parent("label").addClass(this.focusedClass);
        } else {
            $elm.parent("label").removeClass(this.focusedClass);
        }
    };
    SelectionButtons.prototype.markSelected = function($elm) {
        var radioName;
        if ($elm.attr("type") === "radio") {
            radioName = $elm.attr("name");
            $($elm[0].form).find('input[name="' + radioName + '"]').parent("label").removeClass(this.selectedClass);
            $elm.parent("label").addClass(this.selectedClass);
        } else {
            if ($elm.is(":checked")) {
                $elm.parent("label").addClass(this.selectedClass);
            } else {
                $elm.parent("label").removeClass(this.selectedClass);
            }
        }
    };
    SelectionButtons.prototype.addElementLevelEvents = function() {
        this.clickHandler = this.getClickHandler();
        this.focusHandler = this.getFocusHandler({
            level: "element"
        });
        this.$elms.on("click", this.clickHandler).on("focus blur", this.focusHandler);
    };
    SelectionButtons.prototype.addDocumentLevelEvents = function() {
        this.clickHandler = this.getClickHandler();
        this.focusHandler = this.getFocusHandler({
            level: "document"
        });
        $(document).on("click", this.selector, this.clickHandler).on("focus blur", this.selector, this.focusHandler);
    };
    SelectionButtons.prototype.getClickHandler = function() {
        return function(e) {
            this.markSelected($(e.target));
        }.bind(this);
    };
    SelectionButtons.prototype.getFocusHandler = function(opts) {
        var focusEvent = opts.level === "document" ? "focusin" : "focus";
        return function(e) {
            var state = e.type === focusEvent ? "focused" : "blurred";
            this.markFocused($(e.target), state);
        }.bind(this);
    };
    SelectionButtons.prototype.destroy = function() {
        if (typeof this.selector !== "undefined") {
            $(document).off("click", this.selector, this.clickHandler).off("focus blur", this.selector, this.focusHandler);
        } else {
            this.$elms.off("click", this.clickHandler).off("focus blur", this.focusHandler);
        }
    };
    root.GOVUK.SelectionButtons = SelectionButtons;
}).call(this);

(function() {
    "use strict";
    var root = this, $ = root.jQuery;
    if (typeof root.GOVUK === "undefined") {
        root.GOVUK = {};
    }
    var sticky = {
        _hasScrolled: false,
        _scrollTimeout: false,
        init: function() {
            var $els = $(".js-stick-at-top-when-scrolling");
            if ($els.length > 0) {
                sticky.$els = $els;
                if (sticky._scrollTimeout === false) {
                    $(root).scroll(sticky.onScroll);
                    sticky._scrollTimeout = root.setInterval(sticky.checkScroll, 50);
                }
                $(root).resize(sticky.onResize);
            }
            if (root.GOVUK.stopScrollingAtFooter) {
                $els.each(function(i, el) {
                    var $img = $(el).find("img");
                    if ($img.length > 0) {
                        var image = new Image();
                        image.onload = function() {
                            root.GOVUK.stopScrollingAtFooter.addEl($(el), $(el).outerHeight());
                        };
                        image.src = $img.attr("src");
                    } else {
                        root.GOVUK.stopScrollingAtFooter.addEl($(el), $(el).outerHeight());
                    }
                });
            }
        },
        onScroll: function() {
            sticky._hasScrolled = true;
        },
        checkScroll: function() {
            if (sticky._hasScrolled === true) {
                sticky._hasScrolled = false;
                var windowVerticalPosition = $(root).scrollTop();
                sticky.$els.each(function(i, el) {
                    var $el = $(el), scrolledFrom = $el.data("scrolled-from");
                    if (scrolledFrom && windowVerticalPosition < scrolledFrom) {
                        sticky.release($el);
                    } else if ($(root).width() > 768 && windowVerticalPosition >= $el.offset().top) {
                        sticky.stick($el);
                    }
                });
            }
        },
        stick: function($el) {
            if (!$el.hasClass("content-fixed")) {
                $el.data("scrolled-from", $el.offset().top);
                var height = Math.max($el.height(), 1);
                $el.before('<div class="shim" style="width: ' + $el.width() + "px; height: " + height + 'px">&nbsp;</div>');
                $el.css("width", $el.width() + "px").addClass("content-fixed");
            }
        },
        release: function($el) {
            if ($el.hasClass("content-fixed")) {
                $el.data("scrolled-from", false);
                $el.removeClass("content-fixed").css("width", "");
                $el.siblings(".shim").remove();
            }
        }
    };
    root.GOVUK.stickAtTopWhenScrolling = sticky;
}).call(this);

(function() {
    "use strict";
    var root = this, $ = root.jQuery;
    if (typeof root.GOVUK === "undefined") {
        root.GOVUK = {};
    }
    var stopScrollingAtFooter = {
        _pollingId: null,
        _isPolling: false,
        _hasScrollEvt: false,
        _els: [],
        addEl: function($fixedEl, height) {
            var fixedOffset;
            if (!$fixedEl.length) {
                return;
            }
            fixedOffset = parseInt($fixedEl.css("top"), 10);
            fixedOffset = isNaN(fixedOffset) ? 0 : fixedOffset;
            stopScrollingAtFooter.updateFooterTop();
            $(root).on("govuk.pageSizeChanged", stopScrollingAtFooter.updateFooterTop);
            var $siblingEl = $("<div></div>");
            $siblingEl.insertBefore($fixedEl);
            var fixedTop = $siblingEl.offset().top - $siblingEl.position().top;
            $siblingEl.remove();
            var el = {
                $fixedEl: $fixedEl,
                height: height + fixedOffset,
                fixedTop: height + fixedTop,
                state: "fixed"
            };
            stopScrollingAtFooter._els.push(el);
            stopScrollingAtFooter.initTimeout();
        },
        updateFooterTop: function() {
            var footer = $(".js-footer:eq(0)");
            if (footer.length === 0) {
                return 0;
            }
            stopScrollingAtFooter.footerTop = footer.offset().top - 10;
        },
        initTimeout: function() {
            if (stopScrollingAtFooter._hasScrollEvt === false) {
                $(window).scroll(stopScrollingAtFooter.onScroll);
                stopScrollingAtFooter._hasScrollEvt = true;
            }
        },
        onScroll: function() {
            if (stopScrollingAtFooter._isPolling === false) {
                stopScrollingAtFooter.startPolling();
            }
        },
        startPolling: function() {
            if (window.requestAnimationFrame) {
                return function() {
                    var callback = function() {
                        stopScrollingAtFooter.checkScroll();
                        if (stopScrollingAtFooter._isPolling === true) {
                            stopScrollingAtFooter.startPolling();
                        }
                    };
                    stopScrollingAtFooter._pollingId = window.requestAnimationFrame(callback);
                    stopScrollingAtFooter._isPolling = true;
                };
            } else {
                return function() {
                    stopScrollingAtFooter._pollingId = window.setInterval(stopScrollingAtFooter.checkScroll, 16);
                    stopScrollingAtFooter._isPolling = true;
                };
            }
        }(),
        stopPolling: function() {
            if (window.requestAnimationFrame) {
                return function() {
                    window.cancelAnimationFrame(stopScrollingAtFooter._pollingId);
                    stopScrollingAtFooter._isPolling = false;
                };
            } else {
                return function() {
                    window.clearInterval(stopScrollingAtFooter._pollingId);
                    stopScrollingAtFooter._isPolling = false;
                };
            }
        }(),
        checkScroll: function() {
            var cachedScrollTop = $(window).scrollTop();
            if (cachedScrollTop < stopScrollingAtFooter.cachedScrollTop + 2 && cachedScrollTop > stopScrollingAtFooter.cachedScrollTop - 2) {
                stopScrollingAtFooter.stopPolling();
                return;
            } else {
                stopScrollingAtFooter.cachedScrollTop = cachedScrollTop;
            }
            $.each(stopScrollingAtFooter._els, function(i, el) {
                var bottomOfEl = cachedScrollTop + el.height;
                if (bottomOfEl > stopScrollingAtFooter.footerTop) {
                    stopScrollingAtFooter.stick(el);
                } else {
                    stopScrollingAtFooter.unstick(el);
                }
            });
        },
        stick: function(el) {
            if (el.state === "fixed" && el.$fixedEl.css("position") === "fixed") {
                el.$fixedEl.css({
                    position: "absolute",
                    top: stopScrollingAtFooter.footerTop - el.fixedTop
                });
                el.state = "absolute";
            }
        },
        unstick: function(el) {
            if (el.state === "absolute") {
                el.$fixedEl.css({
                    position: "",
                    top: ""
                });
                el.state = "fixed";
            }
        }
    };
    root.GOVUK.stopScrollingAtFooter = stopScrollingAtFooter;
    $(root).load(function() {
        $(root).trigger("govuk.pageSizeChanged");
    });
}).call(this);

(function() {
    "use strict";
    window.GOVUK = window.GOVUK || {};
    var Analytics = function(config) {
        this.trackers = [];
        if (typeof config.universalId != "undefined") {
            this.trackers.push(new GOVUK.GoogleAnalyticsUniversalTracker(config.universalId, config.cookieDomain));
        }
    };
    Analytics.prototype.sendToTrackers = function(method, args) {
        for (var i = 0, l = this.trackers.length; i < l; i++) {
            var tracker = this.trackers[i], fn = tracker[method];
            if (typeof fn === "function") {
                fn.apply(tracker, args);
            }
        }
    };
    Analytics.load = function() {
        GOVUK.GoogleAnalyticsUniversalTracker.load();
    };
    Analytics.prototype.trackPageview = function(path, title, options) {
        this.sendToTrackers("trackPageview", arguments);
    };
    Analytics.prototype.trackEvent = function(category, action, options) {
        this.sendToTrackers("trackEvent", arguments);
    };
    Analytics.prototype.trackShare = function(network) {
        this.sendToTrackers("trackSocial", [ network, "share", location.pathname ]);
    };
    Analytics.prototype.setDimension = function(index, value) {
        this.sendToTrackers("setDimension", arguments);
    };
    Analytics.prototype.addLinkedTrackerDomain = function(trackerId, name, domain) {
        this.sendToTrackers("addLinkedTrackerDomain", arguments);
    };
    GOVUK.Analytics = Analytics;
})();

(function() {
    "use strict";
    GOVUK.analyticsPlugins = GOVUK.analyticsPlugins || {};
    GOVUK.analyticsPlugins.downloadLinkTracker = function(options) {
        var options = options || {}, downloadLinkSelector = options.selector;
        if (downloadLinkSelector) {
            $("body").on("click", downloadLinkSelector, trackDownload);
        }
        function trackDownload(evt) {
            var $link = getLinkFromEvent(evt), href = $link.attr("href"), evtOptions = {
                transport: "beacon"
            }, linkText = $.trim($link.text());
            if (linkText) {
                evtOptions.label = linkText;
            }
            GOVUK.analytics.trackEvent("Download Link Clicked", href, evtOptions);
        }
        function getLinkFromEvent(evt) {
            var $target = $(evt.target);
            if (!$target.is("a")) {
                $target = $target.parents("a");
            }
            return $target;
        }
    };
})();

(function() {
    "use strict";
    GOVUK.analyticsPlugins = GOVUK.analyticsPlugins || {};
    GOVUK.analyticsPlugins.error = function() {
        var trackJavaScriptError = function(e) {
            var errorSource = e.filename + ": " + e.lineno;
            GOVUK.analytics.trackEvent("JavaScript Error", e.message, {
                label: errorSource,
                value: 1,
                nonInteraction: true
            });
        };
        if (window.addEventListener) {
            window.addEventListener("error", trackJavaScriptError, false);
        } else if (window.attachEvent) {
            window.attachEvent("onerror", trackJavaScriptError);
        } else {
            window.onerror = trackJavaScriptError;
        }
    };
})();

(function() {
    "use strict";
    GOVUK.analyticsPlugins = GOVUK.analyticsPlugins || {};
    GOVUK.analyticsPlugins.externalLinkTracker = function() {
        var currentHost = GOVUK.analyticsPlugins.externalLinkTracker.getHostname(), externalLinkSelector = 'a[href^="http"]:not(a[href*="' + currentHost + '"])';
        $("body").on("click", externalLinkSelector, trackClickEvent);
        function trackClickEvent(evt) {
            var $link = getLinkFromEvent(evt), options = {
                transport: "beacon"
            }, href = $link.attr("href"), linkText = $.trim($link.text());
            if (linkText) {
                options.label = linkText;
            }
            GOVUK.analytics.trackEvent("External Link Clicked", href, options);
        }
        function getLinkFromEvent(evt) {
            var $target = $(evt.target);
            if (!$target.is("a")) {
                $target = $target.parents("a");
            }
            return $target;
        }
    };
    GOVUK.analyticsPlugins.externalLinkTracker.getHostname = function() {
        return window.location.hostname;
    };
})();

(function() {
    "use strict";
    window.GOVUK = window.GOVUK || {};
    var GoogleAnalyticsUniversalTracker = function(id, cookieDomain) {
        configureProfile(id, cookieDomain);
        anonymizeIp();
        function configureProfile(id, cookieDomain) {
            sendToGa("create", id, {
                cookieDomain: cookieDomain
            });
        }
        function anonymizeIp() {
            sendToGa("set", "anonymizeIp", true);
        }
    };
    GoogleAnalyticsUniversalTracker.load = function() {
        (function(i, s, o, g, r, a, m) {
            i["GoogleAnalyticsObject"] = r;
            i[r] = i[r] || function() {
                (i[r].q = i[r].q || []).push(arguments);
            }, i[r].l = 1 * new Date();
            a = s.createElement(o), m = s.getElementsByTagName(o)[0];
            a.async = 1;
            a.src = g;
            m.parentNode.insertBefore(a, m);
        })(window, document, "script", "//www.google-analytics.com/analytics.js", "ga");
    };
    GoogleAnalyticsUniversalTracker.prototype.trackPageview = function(path, title, options) {
        var options = options || {};
        if (typeof path === "string") {
            var pageviewObject = {
                page: path
            };
            if (typeof title === "string") {
                pageviewObject.title = title;
            }
            if (options.transport) {
                pageviewObject.transport = options.transport;
            }
            sendToGa("send", "pageview", pageviewObject);
        } else {
            sendToGa("send", "pageview");
        }
    };
    GoogleAnalyticsUniversalTracker.prototype.trackEvent = function(category, action, options) {
        var value, options = options || {}, evt = {
            hitType: "event",
            eventCategory: category,
            eventAction: action
        };
        if (typeof options.label === "string") {
            evt.eventLabel = options.label;
        }
        if (typeof options.page === "string") {
            evt.page = options.page;
        }
        if (options.value || options.value === 0) {
            value = parseInt(options.value, 10);
            if (typeof value === "number" && !isNaN(value)) {
                evt.eventValue = value;
            }
        }
        if (options.nonInteraction) {
            evt.nonInteraction = 1;
        }
        if (options.transport) {
            evt.transport = options.transport;
        }
        sendToGa("send", evt);
    };
    GoogleAnalyticsUniversalTracker.prototype.trackSocial = function(network, action, target) {
        sendToGa("send", {
            hitType: "social",
            socialNetwork: network,
            socialAction: action,
            socialTarget: target
        });
    };
    GoogleAnalyticsUniversalTracker.prototype.addLinkedTrackerDomain = function(trackerId, name, domain) {
        sendToGa("create", trackerId, "auto", {
            name: name
        });
        sendToGa("require", "linker");
        sendToGa(name + ".require", "linker");
        sendToGa("linker:autoLink", [ domain ]);
        sendToGa(name + ".linker:autoLink", [ domain ]);
        sendToGa(name + ".set", "anonymizeIp", true);
        sendToGa(name + ".send", "pageview");
    };
    GoogleAnalyticsUniversalTracker.prototype.setDimension = function(index, value) {
        sendToGa("set", "dimension" + index, String(value));
    };
    function sendToGa() {
        if (typeof window.ga === "function") {
            ga.apply(window, arguments);
        }
    }
    GOVUK.GoogleAnalyticsUniversalTracker = GoogleAnalyticsUniversalTracker;
})();

(function() {
    "use strict";
    GOVUK.analyticsPlugins = GOVUK.analyticsPlugins || {};
    GOVUK.analyticsPlugins.printIntent = function() {
        var printAttempt = function() {
            GOVUK.analytics.trackEvent("Print Intent", document.location.pathname);
            GOVUK.analytics.trackPageview("/print" + document.location.pathname);
        };
        if (window.matchMedia) {
            var mediaQueryList = window.matchMedia("print"), mqlListenerCount = 0;
            mediaQueryList.addListener(function(mql) {
                if (!mql.matches && mqlListenerCount === 0) {
                    printAttempt();
                    mqlListenerCount++;
                    window.setTimeout(function() {
                        mqlListenerCount = 0;
                    }, 3e3);
                }
            });
        }
        if (window.onafterprint) {
            window.onafterprint = printAttempt;
        }
    };
})();

(function() {
    "use strict";
    if (typeof window.GOVUK === "undefined") {
        window.GOVUK = {};
    }
    if (typeof window.GOVUK.support === "undefined") {
        window.GOVUK.support = {};
    }
    window.GOVUK.support.history = function() {
        return window.history && window.history.pushState && window.history.replaceState;
    };
    if (typeof window.DfE === "undefined") {
        window.DfE = {
            Views: {},
            Elements: {},
            Util: {
                Analytics: {}
            }
        };
    }
})();