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
    title: 'The news page has been updated',
    content: 'A new news item has been added to the news page.  We recommend you view this so you are aware of the recent development release to Get Information about Schools (GIAS). When applicable other non-development information is recorded here. Viewing the news items could improve your user experience of the service. Select the button,  \'Go to news page\' to view the news page or select the news tab on the main GIAS header on each page.',
    triggerEvent: 'click',
    okLabel: "Go to news page",
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
