import { createApp } from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
const clientErrorSummary = createApp({
  components: {
    errorSummary,
  },
  data() {
    return {
      errors: []
    };
  },
  methods: {
    updateErrors: function(errObj){
      this.errors = [];
      if (errObj) {
        this.errors.push(errObj);
      }
    }
  }
});

class GiasSearchErrors {
  constructor() {
    this.init();
  }

  init() {
    const $main  = $('#main-content');
    const showInlineError = function($panel, message) {
      $('.govuk-error-message').addClass('hidden');

      $panel.find('.govuk-error-message').eq(0).removeClass('hidden').html(message);
      $panel.addClass('govuk-form-group--error');

    };

    const hideErrorSummary = function() {
      $main.find('.server-error-summary').addClass('hidden');
      $main.find('.panel').removeClass('govuk-form-group--error');
      clientErrorSummary.updateErrors();
    }

    // Name or reference number search
    $('#name-search-submit').on('click', function (e) {
      hideErrorSummary();
      const suggestionCount = $('#TextSearchModel_Text').next('ul').find('li').length;
      const numericValue = !isNaN ($('#TextSearchModel_Text').val().replace('/', ''));

      if ($.trim($('#TextSearchModel_Text').val()) === '') {
        e.preventDefault();
        showInlineError($('#searchby-name-ref'),
          'Please enter an establishment name, URN, LAESTAB or UKPRN to start a search');

        clientErrorSummary.updateErrors({
          href: '#TextSearchModel_Text',
          message: 'Please enter an establishment name, URN, LAESTAB or UKPRN to start a search'
        });
      }

      if (!numericValue && suggestionCount === 0) {
        e.preventDefault();
        showInlineError($('#searchby-name-ref'),
          'We could not find any establishments matching your search criteria');
        clientErrorSummary.updateErrors({
          href: '#TextSearchModel_Text',
          message: 'We could not find any establishments matching your search criteria'
        });
      }
    });

    // Location search
    $('#location-search-submit').on('click', function (e) {
      hideErrorSummary();
      if ($.trim($('#LocationSearchModel_Text').val()) === '') {
        e.preventDefault();
        showInlineError($('#searchby-location-ref'),
          'Please enter a postcode, town or city to start a search');

        clientErrorSummary.updateErrors({
          href: '#LocationSearchModel_Text',
          message: 'Please enter a postcode, town or city to start a search'
        });
      }
    });

    // Local Authority search
    $('#la-search-submit').on('click', function (e) {
      hideErrorSummary();
      const suggestionCount = $('#LocalAuthorityToAdd').next('ul').find('li').length;
      const addedLaCount = $('.remove-suggest-la').length;

      if (addedLaCount === 0 && $.trim($('#LocalAuthorityToAdd').val()) === '') {
        e.preventDefault();

        if ($.trim($('#LocalAuthorityToAdd').val()) === '') {
          clientErrorSummary.updateErrors({
            href: '#LocalAuthorityToAdd',
            message: 'Please enter a local authority to start a search'
          });

          return showInlineError($('#searchby-la-ref'),
            'Please enter a local authority to start a search');

        } else if (suggestionCount === 0) {

          clientErrorSummary.updateErrors({
            href: '#LocalAuthorityToAdd',
            message: 'We could not find any local authorities matching your search criteria'
          });

          return showInlineError($('#searchby-la-ref'),
            'We could not find any local authorities matching your search criteria');
        }
      }
      // else {
      //
      //   if (suggestionCount === 0 && $.trim($('#LocalAuthorityToAdd').val()).length > 0) {
      //     e.preventDefault();
      //
      //     clientErrorSummary.updateErrors({
      //       href: '#LocalAuthorityToAdd',
      //       message: 'We don\'t recognise this local authority. Amend it or clear it to continue searching.'
      //     });
      //
      //     return showInlineError($('#searchby-la-ref'),
      //       'We don\'t recognise this local authority. Amend it or clear it to continue searching.');
      //   }
      // }
    });

    // Group search
    $('#group-search-submit').on('click', function (e) {
      hideErrorSummary();
      const suggestionCount = $('#GroupSearchModel_Text').next('ul').find('li').length;
      const numericValue = !isNaN($('#GroupSearchModel_Text').val().replace(/\D/g, ""));

      if ($.trim($('#GroupSearchModel_Text').val()) === '') {
        e.preventDefault();
        clientErrorSummary.updateErrors({
          href: '#GroupSearchModel_Text',
          message: 'Please enter an establishment group to start a search.'
        });

        return showInlineError($('#group-search-container'),
          'Please enter an establishment group to start a search');

      } else if (suggestionCount === 0 && !numericValue) {
        e.preventDefault();
        clientErrorSummary.updateErrors({
          href: '#GroupSearchModel_Text',
          message: 'We couldn’t find any establishment groups matching your search criteria'
        });
        return showInlineError($('#group-search-container'),
          'We couldn’t find any establishment groups matching your search criteria');
      }
    });

    // Governor search
    $('#governor-search-submit').on('click', function (e) {
      hideErrorSummary();
      const fName = $.trim($('#GovernorSearchModel_Forename').val());
      const sName = $.trim($('#surname').val());
      const roles = $('#governor-roles').find(':checkbox').filter(':checked');
      const gId = $.trim($('#GovernorSearchModel_Gid').val());

      if (fName === '' && sName === '' && roles.length === 0) {
        e.preventDefault();
        clientErrorSummary.updateErrors({
          href: '#GovernorSearchModel_Forename',
          message: 'Please enter a governor to start a search'
        });
        return showInlineError($('#searchtype-gov-namerole-ref'),
          'Please enter a governor to start a search');
      }
    });

    // Governor reference search
    $('#governor-search-submit-1').on('click', function (e) {
      hideErrorSummary();

      const gId = $.trim($('#GovernorSearchModel_Gid').val());
      if (gId === '' || isNaN(gId)) {
        e.preventDefault();
        clientErrorSummary.updateErrors({
          href: '#GovernorSearchModel_Gid',
          message: 'Please enter a governor ID to start a search'
        });
        return showInlineError($('#searchtype-gov-refno-ref'),
          'Please enter a governor ID to start a search');
      }
    });

  }
}

clientErrorSummary.mount('#js-error-summary');

export default GiasSearchErrors;
