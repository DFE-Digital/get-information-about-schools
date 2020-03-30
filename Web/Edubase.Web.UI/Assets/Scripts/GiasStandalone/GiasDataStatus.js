class CheckGiasDataStatus {
  constructor() {
    this.options = {
      statusUrl: '/DataQuality/Status',
      editStatusUrl: '/DataQuality/Edit',
      viewStatusUrl: '/DataQuality',
      params: {},
      cookieExpiresIn: 0.08
    };

    if (window.DfECookieManager.cookie('dataStatusViewed') !== 'true') {
      this.checkPromptRequired();
    }
  }

  exitOverlay(e) {
    e.preventDefault();

    document.body.removeChild(document.getElementById('data-status-summary'));
    document.body.removeChild(document.getElementById('modal-overlay'));

  }
  clearCookie() {
    window.DfECookieManager.setCookie('dataStatusViewed', '', { days: -1 });
    return true;
  }

  attachEvents() {
    const self = this;

    $(document).on('click', '#data-status-close, .data-status-exit', self.exitOverlay);

    $(document).on('click', '#data-status-info', function() {
      window.location = self.options.viewStatusUrl;
    });

    $(document).on('click', '#data-status-update', function () {
      window.location = self.options.editStatusUrl;
    });

    $('#logout-link').on('click', function (e) {
      e.preventDefault();
      const href = $(this).attr('href');
      self.clearCookie();
      window.location = href;
    });

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
    const self = this;
    let mask = document.createElement('div');

    self.attachEvents();

    mask.id = 'modal-overlay';
    mask.classList.add('modal-overlay');
    document.body.appendChild(mask);

    $.ajax({
      url: self.options.statusUrl,
      data: self.options.params,
      dataType: 'html',
      success: function (data) {
        const content = $(data).filter('#data-status-summary');
        $(document.body).append(content);
        window.DfECookieManager.setCookie('dataStatusViewed', 'true', { days: self.options.cookieExpiresIn });
        window.setTimeout(function() {
          if ($('#data-status-summary').length > 0) {
            $('#data-status-summary').focus();
          }
        }, 100);
      },
      error: function () {
        console.error('Error retrieving data status panel');
      }
    });

  }
}

module.exports = CheckGiasDataStatus;
