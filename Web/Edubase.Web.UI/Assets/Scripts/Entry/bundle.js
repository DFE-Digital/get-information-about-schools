import giasDismissMessage from '../GiasModules/GiasDismissMessage';
import giasAriaLive from '../GiasGlobal/giasAriaLive';
import GiasModal from '../GiasModules/GiasModals/GiasModal';
import GiasOkCancel from "../GiasModules/GiasModals/GiasOkCancel";
import giasAccordionExtensions from '../GiasGlobal/GiasAccordionExtensions';
import GiasReOrderValidationSummary from '../GiasGlobal/GiasReOrderValidationSummary';
import CheckGiasDataStatus from '../GiasStandalone/GiasDataStatus';

import DfECookieManager from '../GiasModules/DfeCookieManager';
window.$ = $;

window.DfECookieManager = new DfECookieManager();
window.checkGiasDataStatus = CheckGiasDataStatus;


import { initAll } from 'govuk-frontend';

const $main = $('#main-content');

const message = document.getElementById('global-cookie-message');
const acceptedCookies = (message && window.DfECookieManager.cookie('seen_cookie_message') !== null);

if (!acceptedCookies) {
  message.style.display = 'block';

  document.getElementById('button-accept-cookies').addEventListener('click', function (e) {
    e.preventDefault();
    message.style.display = 'none';
    window.DfECookieManager.cookie('seen_cookie_message', 'yes', {days: 28});
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
    okLabel: "Go to News page",
    headingSize: 'l',
  });
  window.DfECookieManager.setCookie(myetag, etag, { days: 365 });
};

initAll();

giasAriaLive();

$main.find('.modal-link').each(function(n, el) {
  new GiasModal(el);
});
giasAccordionExtensions();
giasDismissMessage();

if ($main.find('.govuk-error-summary').length > 0) {
  GiasReOrderValidationSummary();
}
