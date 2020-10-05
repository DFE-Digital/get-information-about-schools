import giasDismissMessage from "../GiasModules/GiasDismissMessage";
import giasAriaLive from '../GiasGlobal/giasAriaLive';
import GiasToolTip from "../GiasGlobal/GiasToolTips";
import GiasOkCancel from "../GiasModules/GiasModals/GiasOkCancel";
import GiasAccordionExtensions from '../GiasGlobal/GiasAccordionExtensions';
const checkGiasDataStatus = require("../GiasStandalone/GiasDataStatus");
const dfeCookieManager = require('../GiasModules/DfeCookieManager');

window.$ = $;

window.DfECookieManager = dfeCookieManager;
window.checkGiasDataStatus = checkGiasDataStatus;


import { initAll } from 'govuk-frontend';


const message = document.getElementById('global-cookie-message');
const acceptedCookies = (message && dfeCookieManager.cookie('seen_cookie_message') !== null);

if (!acceptedCookies) {
  message.style.display = 'block';

  document.getElementById('button-accept-cookies').addEventListener('click', function (e) {
    e.preventDefault();
    message.style.display = 'none';
    dfeCookieManager.cookie('seen_cookie_message', 'yes', {days: 28});
  });
}

window.displayNewsDialog = function(myetag, etag) {
  $("<p/>").okCancel({
    immediate: true,
    ok: function () {
      location.href = "/news";
    },
    title: 'The News page has been updated',
    content: 'A new article has been added to the News Page that could help your Get information about schools experience. Click \'Go to news page\' to view the News Page or find the News link in the black header.',
    triggerEvent: 'click',
    okLabel: "Go to News page"
  });
  DfECookieManager.setCookie(myetag, etag, { days: 365 });
};

initAll();

giasAriaLive();
$('#main-content').find('.js-tooltip').giasToolTip();

$('.govuk-accordion').giasAccordionExtensions();
giasDismissMessage();
