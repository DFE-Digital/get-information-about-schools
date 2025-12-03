import GiasModal from '../GiasModules/GiasModals/GiasModal';
class CheckGiasDataStatus {
  constructor() {
    this.options = {
      statusUrl: '/DataQuality/Status',
      cookieExpiresIn: 0.08
    };

    if (window.DfECookieManager.cookie('dataStatusViewed') !== 'true') {
      this.checkPromptRequired();
    }
  }

  clearCookie() {
    window.DfECookieManager.setCookie('dataStatusViewed', '', { days: -1 });
    return true;
  }


  checkPromptRequired() {
    const self = this;
    $.ajax({
      url: '/Api/should-show-data-status',
      dataType: 'json',
      success: function(data) {
        if (data.userRequiresDataPrompt) {
          self.init();
        }
      },
      error: function() {
        console.log('Error retrieving users data prompt status');
      }
    });

  }

  init() {
    const opts = this.options;
    const modal = new GiasModal(document.body, {
      remoteContent: true,
      remoteUrl: opts.statusUrl,
      immediate: true,
      additionalClasses: 'data-status-summary',
      headingSize: 'l',
      contentSelector: '#data-status-summary',
      closeButtonClass: '.data-status-exit'
    });

    window.DfECookieManager.setCookie('dataStatusViewed', 'true', { days: opts.cookieExpiresIn });
  }
}

export default CheckGiasDataStatus;
