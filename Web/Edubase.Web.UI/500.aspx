<%@ Page Language="C#" %>
<% Response.StatusCode = 500; %>
<% var errorCode = HttpContext.Current.Items["edubase_error_code"]; %>

<!doctype html>
<html lang="en" class="govuk-template">
<head>
    <meta charset="utf-8">
    <title>Get Information about Schools - GOV.UK</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover">
    <meta name="theme-color" content="blue">

    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <!-- Google Tag Manager -->
    <script nonce="x8k8v9r9MFNM3BVGnKMrUv5gDSq4YEES6RLQirxVw+8=">
        (function (w, d, s, l, i) {
            w[l] = w[l] || []; w[l].push({
                'gtm.start':
                    new Date().getTime(), event: 'gtm.js'
            }); var f = d.getElementsByTagName(s)[0],
                j = d.createElement(s), dl = l != 'dataLayer' ? '&l=' + l : ''; j.async = true; j.src =
                    'https://www.googletagmanager.com/gtm.js?id=' + i + dl; f.parentNode.insertBefore(j, f);
        })(window, document, 'script', 'dataLayer', 'GTM-WSQH3HX'); //
    </script>
    <!-- End Google Tag Manager -->

    <meta name="google-site-verification" content="uRt7CAkfClOB8Jig6Pw2-DELUWuuyG2kYKip0UThxhg"/>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta property="og:image" content="/public/govuk-frontend/images/opengraph-image.png?0.12.0">
    <meta name="description">
    <meta name="keywords" content="Get Information about Schools"/>

    <script nonce="x8k8v9r9MFNM3BVGnKMrUv5gDSq4YEES6RLQirxVw+8=">
        (function () {
            if (navigator.userAgent.match(/IEMobile\/10\.0/)) {
                var d = document, c = "appendChild", a = d.createElement("style");
                a[c](d.createTextNode("@-ms-viewport{width:auto!important}"));
                d.getElementsByTagName("head")[0][c](a);
            }
        })();
    </script>

    <link rel="shortcut icon" sizes="16x16 32x32 48x48" href="/public/govuk-frontend/images/favicon.ico" type="image/x-icon">
    <link rel="mask-icon" href="/public/govuk-frontend/images/govuk-mask-icon.svg" color="blue">
    <link rel="apple-touch-icon" sizes="180x180" href="/public/govuk-frontend/images/govuk-apple-touch-icon-180x180.png">
    <link rel="apple-touch-icon" sizes="167x167" href="/public/govuk-frontend/images/govuk-apple-touch-icon-167x167.png">
    <link rel="apple-touch-icon" sizes="152x152" href="/public/govuk-frontend/images/govuk-apple-touch-icon-152x152.png">
    <link rel="apple-touch-icon" href="/public/govuk-frontend/images/govuk-apple-touch-icon.png">

    <link href="/public/assets/stylesheets/main.css?v=1.0.0.0" rel="stylesheet" type="text/css">
</head>
<body class="govuk-template__body schools-search-page search-page no-js">
<div id="app">
    <script type="text/javascript" nonce="@Html.ScriptNonce()">
        document.body.className = document.body.className.replace(/\bno-js\b/, 'js-enabled');
        if (!('ontouchstart' in document.documentElement)) { document.body.className += ' no-touch'; }
        if (navigator.userAgent.indexOf('Mac OS X') > -1) { document.body.className += ' is-mac';}
    </script>

    <a href="#main-content" class="govuk-skip-link">Skip to main content</a>

    <div class="app-cookie-banner js-cookie-banner hidden" id="global-cookie-message">
        <p class="govuk-width-container">
            GOV.UK uses cookies which are essential for the site to work. We also use non-essential cookies to help us improve government digital services. Any data collected is anonymised. By continuing to use this site, you agree to our use of cookies. <a class="govuk-link" href="/cookies">Find out more about cookies</a><br/>
            <button class="govuk-button" id="button-accept-cookies">Accept cookies</button>
        </p>
    </div>

    <header role="banner" id="global-header" class="govuk-header" data-module="govuk-header">
        <div class="govuk-header__container govuk-width-container">
            <div class="govuk-header__logo">
                <a href="/" class="govuk-header__link govuk-header__link--homepage">
                    <span class="govuk-header__logotype">
                        <svg aria-hidden="true" focusable="false" class="govuk-header__logotype-crown" xmlns="http://www.w3.org/2000/svg" viewbox="0 0 132 97" height="30" width="36">
                            <path fill="currentColor" fill-rule="evenodd" d="M25 30.2c3.5 1.5 7.7-.2 9.1-3.7 1.5-3.6-.2-7.8-3.9-9.2-3.6-1.4-7.6.3-9.1 3.9-1.4 3.5.3 7.5 3.9 9zM9 39.5c3.6 1.5 7.8-.2 9.2-3.7 1.5-3.6-.2-7.8-3.9-9.1-3.6-1.5-7.6.2-9.1 3.8-1.4 3.5.3 7.5 3.8 9zM4.4 57.2c3.5 1.5 7.7-.2 9.1-3.8 1.5-3.6-.2-7.7-3.9-9.1-3.5-1.5-7.6.3-9.1 3.8-1.4 3.5.3 7.6 3.9 9.1zm38.3-21.4c3.5 1.5 7.7-.2 9.1-3.8 1.5-3.6-.2-7.7-3.9-9.1-3.6-1.5-7.6.3-9.1 3.8-1.3 3.6.4 7.7 3.9 9.1zm64.4-5.6c-3.6 1.5-7.8-.2-9.1-3.7-1.5-3.6.2-7.8 3.8-9.2 3.6-1.4 7.7.3 9.2 3.9 1.3 3.5-.4 7.5-3.9 9zm15.9 9.3c-3.6 1.5-7.7-.2-9.1-3.7-1.5-3.6.2-7.8 3.7-9.1 3.6-1.5 7.7.2 9.2 3.8 1.5 3.5-.3 7.5-3.8 9zm4.7 17.7c-3.6 1.5-7.8-.2-9.2-3.8-1.5-3.6.2-7.7 3.9-9.1 3.6-1.5 7.7.3 9.2 3.8 1.3 3.5-.4 7.6-3.9 9.1zM89.3 35.8c-3.6 1.5-7.8-.2-9.2-3.8-1.4-3.6.2-7.7 3.9-9.1 3.6-1.5 7.7.3 9.2 3.8 1.4 3.6-.3 7.7-3.9 9.1zM69.7 17.7l8.9 4.7V9.3l-8.9 2.8c-.2-.3-.5-.6-.9-.9L72.4 0H59.6l3.5 11.2c-.3.3-.6.5-.9.9l-8.8-2.8v13.1l8.8-4.7c.3.3.6.7.9.9l-5 15.4v.1c-.2.8-.4 1.6-.4 2.4 0 4.1 3.1 7.5 7 8.1h.2c.3 0 .7.1 1 .1.4 0 .7 0 1-.1h.2c4-.6 7.1-4.1 7.1-8.1 0-.8-.1-1.7-.4-2.4V34l-5.1-15.4c.4-.2.7-.6 1-.9zM66 92.8c16.9 0 32.8 1.1 47.1 3.2 4-16.9 8.9-26.7 14-33.5l-9.6-3.4c1 4.9 1.1 7.2 0 10.2-1.5-1.4-3-4.3-4.2-8.7L108.6 76c2.8-2 5-3.2 7.5-3.3-4.4 9.4-10 11.9-13.6 11.2-4.3-.8-6.3-4.6-5.6-7.9 1-4.7 5.7-5.9 8-.5 4.3-8.7-3-11.4-7.6-8.8 7.1-7.2 7.9-13.5 2.1-21.1-8 6.1-8.1 12.3-4.5 20.8-4.7-5.4-12.1-2.5-9.5 6.2 3.4-5.2 7.9-2 7.2 3.1-.6 4.3-6.4 7.8-13.5 7.2-10.3-.9-10.9-8-11.2-13.8 2.5-.5 7.1 1.8 11 7.3L80.2 60c-4.1 4.4-8 5.3-12.3 5.4 1.4-4.4 8-11.6 8-11.6H55.5s6.4 7.2 7.9 11.6c-4.2-.1-8-1-12.3-5.4l1.4 16.4c3.9-5.5 8.5-7.7 10.9-7.3-.3 5.8-.9 12.8-11.1 13.8-7.2.6-12.9-2.9-13.5-7.2-.7-5 3.8-8.3 7.1-3.1 2.7-8.7-4.6-11.6-9.4-6.2 3.7-8.5 3.6-14.7-4.6-20.8-5.8 7.6-5 13.9 2.2 21.1-4.7-2.6-11.9.1-7.7 8.8 2.3-5.5 7.1-4.2 8.1.5.7 3.3-1.3 7.1-5.7 7.9-3.5.7-9-1.8-13.5-11.2 2.5.1 4.7 1.3 7.5 3.3l-4.7-15.4c-1.2 4.4-2.7 7.2-4.3 8.7-1.1-3-.9-5.3 0-10.2l-9.5 3.4c5 6.9 9.9 16.7 14 33.5 14.8-2.1 30.8-3.2 47.7-3.2z"></path>
                            <image src="/public/govuk-frontend/images/govuk-logotype-crown.png" xlink:href="" class="govuk-header__logotype-crown-fallback-image" width="36" height="32"></image>
                        </svg>
                        <span class="govuk-header__logotype-text">
                            GOV.UK
                        </span>
                    </span>
                </a>
            </div>

            <div class="govuk-header__content">
                <a class="govuk-header__link govuk-header__link--service-name" href="/" id="proposition-name">Get Information about Schools</a>
                <button type="button" class="govuk-header__menu-button govuk-js-header-toggle" aria-controls="navigation" aria-label="Show or hide Top Level Navigation">Menu</button>
                <nav>
                    <ul id="navigation" class="govuk-header__navigation " aria-label="Top Level Navigation">
                        <li class="govuk-header__navigation-item govuk-header__navigation-item--active">
                            <a class="govuk-header__link" href="/">Search</a>
                        </li>
                        <li class="govuk-header__navigation-item ">
                            <a class="govuk-header__link" href="/Downloads">Downloads</a>
                        </li>
                        <li class="govuk-header__navigation-item ">
                            <a class="govuk-header__link" href="/help">Help</a>
                        </li>
                        <li class="govuk-header__navigation-item ">
                            <a href="/News" class="govuk-header__link">News</a>
                        </li>
                        <li class="govuk-header__navigation-item">
                            <a Class="login-link govuk-header__link" href="/Account/Login?returnUrl=%2F" rel="nofollow">Sign in</a>
                        </li>
                    </ul>
                </nav>
            </div>
        </div>
    </header>

    <!--end global-header-bar-->

    <div class="govuk-width-container govuk-width-container">
        <div class="govuk-phase-banner">
            <p class="govuk-phase-banner__content">
                <strong class="govuk-tag govuk-phase-banner__content__tag">
                    beta
                </strong>
                <span class="govuk-phase-banner__text">
                    How could we improve this service? Your 
                    <a href="https://forms.office.com/Pages/ResponsePage.aspx?id=yXfS-grGoU2187O4s0qC-ZYZAR7wa5FMmFGoqFTjsw5UREJBVlAxSTBKWFdOMEcwTTU1M0gyVVJKRCQlQCN0PWcu">
                        feedback
                    </a>
                    will help.
                </span>
            </p>
        </div>
        <div class="govuk-grid-row">
            <div class="govuk-grid-column-full">
                <div class="govuk-breadcrumbs">
                    <ol class="govuk-breadcrumbs__list">
                        <li class="govuk-breadcrumbs__list-item"><a href="/" class="govuk-breadcrumbs__link">Search</a></li>
                    </ol>
                </div>
            </div>
        </div>



    <main id="main-content" class="page error-page" role="main" tabindex="-1">


        <div class="govuk-grid-row">
            <div class="govuk-grid-column-two-thirds">
                <h1 class="govuk-heading-xl">Sorry, there is a problem with the service</h1>
            </div>
            <div class="govuk-grid-column-two-thirds">
                <p>Try again later.</p>
                <% if (errorCode != null)
                   { %>
                    <p style="color:#ddd;">Error code: <%= errorCode %></p>
                <% } %>
            </div>
        </div>


    </main>
    </div>
    <footer class="govuk-footer" id="footer" role="contentinfo">
        <div class="govuk-width-container">
            <div class="govuk-footer__meta">
                <div class="govuk-footer__meta-item govuk-footer__meta-item--grow">
                    <h2 class="govuk-visually-hidden">Support links</h2>
                    <ul class="govuk-footer__inline-list">
                        <li class="govuk-footer__inline-list-item"><a class="govuk-footer__link" href="/Contact">Contact</a></li>
                        <li class="govuk-footer__inline-list-item"><a class="govuk-footer__link" href="/glossary">Glossary</a></li>
                        <li class="govuk-footer__inline-list-item"><a class="govuk-footer__link" href="/Guidance">Guidance</a></li>
                        <li class="govuk-footer__inline-list-item"><a class="govuk-footer__link" href="/Privacy">Privacy</a></li>
                        <li class="govuk-footer__inline-list-item"><a class="govuk-footer__link" href="/cookies">Cookies</a></li>
                        <li class="govuk-footer__inline-list-item"><a class="govuk-footer__link" href="/accessibility">Accessibility statement</a></li>

                    </ul>

                    <svg aria-hidden="true" focusable="false" class="govuk-footer__licence-logo" xmlns="http://www.w3.org/2000/svg" viewbox="0 0 483.2 195.7" height="17" width="41">
                        <path fill="currentColor" d="M421.5 142.8V.1l-50.7 32.3v161.1h112.4v-50.7zm-122.3-9.6A47.12 47.12 0 0 1 221 97.8c0-26 21.1-47.1 47.1-47.1 16.7 0 31.4 8.7 39.7 21.8l42.7-27.2A97.63 97.63 0 0 0 268.1 0c-36.5 0-68.3 20.1-85.1 49.7A98 98 0 0 0 97.8 0C43.9 0 0 43.9 0 97.8s43.9 97.8 97.8 97.8c36.5 0 68.3-20.1 85.1-49.7a97.76 97.76 0 0 0 149.6 25.4l19.4 22.2h3v-87.8h-80l24.3 27.5zM97.8 145c-26 0-47.1-21.1-47.1-47.1s21.1-47.1 47.1-47.1 47.2 21 47.2 47S123.8 145 97.8 145"/>
                    </svg>
                    <span class="govuk-footer__licence-description">
                        All content is available under the
                        <a class="govuk-footer__link" href="https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/" rel="license">Open Government Licence v3.0</a>, except where otherwise stated<br />
                        v 1.0.0.0
                    </span>
                </div>
                <div class="govuk-footer__meta-item">
                    <a class="govuk-footer__link govuk-footer__copyright-logo" href="https://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/uk-government-licensing-framework/crown-copyright/">&copy; Crown copyright</a>
                </div>
            </div>
        </div>
    </footer>
    <!--end footer-->

    <div id="global-app-error" class="app-error hidden"></div>

    <script src="/public/assets/scripts/build/bundle.js?v=1.0.0.0"></script>

</div>
</body>

</html>
