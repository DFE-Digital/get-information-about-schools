const GiasCookieBanner = {
  preferenceEndPoint: '/CookieChoicesAjax',

  setPreference: function(accept) {
    $.ajax({
      url: GiasCookieBanner.preferenceEndPoint,
      data: { acceptAnalyticsCookies: accept },
      type: 'post',
      success : function(d) {
        GiasCookieBanner.updateBanner(accept)
      }
    });
  },

  updateBanner: function(accept) {
    const bannerHtml =
      `<div class="govuk-grid-column-two-thirds">
          <p>
              You have ${ accept === true ? "accepted" : "rejected"} additional cookies.
              You can <a href="/cookies#set-analytics-prefs">change your cookie settings</a> at any time.
          </p>
          <div class="govuk-button-group">
              <button class="gias-cookie-banner__hide-button govuk-button" id="gias-dismiss-cookie-banner">Hide this message</button>
          </div>
      </div>`;

    $('#cookie-banner').find('.govuk-grid-column-two-thirds').replaceWith(bannerHtml);
  },

  bindEvents: function() {
    $('#cookie-banner').find('.govuk-button').on('click', function(e){
      e.preventDefault();
      GiasCookieBanner.setPreference($(this).val() === 'true');
    });

    // dismiss with delgation as this content will be loaded in after the pref has been set
    $('#cookie-banner').on('click', '.gias-cookie-banner__hide-button', function(e) {
      e.preventDefault();
      $('#cookie-banner').addClass('hidden');
    })
  }


};

export default GiasCookieBanner;
