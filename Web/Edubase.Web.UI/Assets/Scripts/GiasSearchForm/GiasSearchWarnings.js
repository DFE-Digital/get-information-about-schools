class GiasSearchWarnings {
  constructor() {
    this.init()
  }

  init() {
    const showWarning = function($panel, message){
      $('.warning-message').addClass('govuk-visually-hidden');
      $panel.find('.warning-message').first().removeClass('govuk-visually-hidden');
      $panel.find('.message-text').first().html(message);
    };

    // Name or reference number search
    $('#name-search-submit').on('click', function (e) {
      const suggestionCount = $('#TextSearchModel_Text').next('ul').find('li').length;
      const numericValue = !isNaN ($('#TextSearchModel_Text').val().replace('/', ''));

      if ($.trim($('#TextSearchModel_Text').val()) === '') {
        e.preventDefault();
        showWarning($('#searchby-name-ref'),
          'Please enter an establishment name, URN, LAESTAB or UKPRN to start a search');
      }
    });

    // Location search
    $('#location-search-submit').on('click', function (e) {
      if ($.trim($('#LocationSearchModel_Text').val()) === '') {
        e.preventDefault();
        showWarning($('#searchby-location-ref'),
          'Please enter a postcode, town or city to start a search');
      }
    });

    // Local Authority search
    $('#la-search-submit').on('click', function (e) {
      const suggestionCount = $('#LocalAuthorityToAdd').next('ul').find('li').length;
      const addedLaCount = $('.remove-suggest-la').length;

      if (addedLaCount === 0) {
        e.preventDefault();

        if ($.trim($('#LocalAuthorityToAdd').val()) === '') {
          return showWarning($('#searchby-la-ref'),
            'Please enter a local authority to start a search');
        } else if (suggestionCount === 0) {
          return showWarning($('#searchby-la-ref'),
            'We couldn\'t find any local authorities matching your search criteria');
        }
      } else {

        if (suggestionCount === 0 && $.trim($('#LocalAuthorityToAdd').val()).length > 0) {
          e.preventDefault();
          return showWarning($('#searchby-la-ref'),
            'We don’t recognise this local authority. Amend it or clear it to continue searching.');
        }
      }
    });

    // Group search
    $('#group-search-submit').on('click', function (e) {
      var suggestionCount = $('#GroupSearchModel_Text').next('ul').find('li').length;
      var numericValue = !isNaN($('#GroupSearchModel_Text').val().replace(/\D/g, ""));

      if ($.trim($('#GroupSearchModel_Text').val()) === '') {
        e.preventDefault();
        return showWarning($('#group-search-container'),
          'Please enter an establishment group to start a search');

      } else if (suggestionCount === 0 && !numericValue) {
        e.preventDefault();
        return showWarning($('#group-search-container'),
          'We couldn’t find any establishment groups matching your search criteria');
      }
    });

    // Governor search
    $('#governor-search-submit').on('click', function (e) {
      const fName = $.trim($('#forename').val());
      const sName = $.trim($('#surname').val());
      const roles = $('#governor-roles').find(':checkbox').filter(':checked');
      const gId = $.trim($('#GovernorSearchModel_Gid').val());

      if (fName === '' && sName === '' && roles.length === 0) {
        e.preventDefault();
        return showWarning($('#searchtype-gov-namerole-ref'),
          'Please enter a governor to start a search');
      }
    });

    // Governor reference search
    $('#governor-search-submit-1').on('click', function (e) {
      const gId = $.trim($('#GovernorSearchModel_Gid').val());

      if (gId === '') {
        e.preventDefault();
        return showWarning($('#searchtype-gov-refno-ref'),
          'Please enter a governor ID to start a search');
      }
    });
  }
}


export default GiasSearchWarnings;
