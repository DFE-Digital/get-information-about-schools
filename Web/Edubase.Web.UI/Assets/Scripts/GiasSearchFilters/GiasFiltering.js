import QueryString from "../GiasHelpers/QueryString";
import supportsHistory from "../GiasHelpers/supportsHistory";
import GiasFilterValidation from "./GiasFilterValidation";
class GiasFiltering {
  constructor() {
    this.init()
  }

  init() {
    const $cos = $('#client-only-searchtype');

    this.filterIntent = null;
    this.searchParams = this.getParams();
    this.$form = $('#filter-form');
    this.seenOpenDateWarning = false;

    if ($cos) {
      this.searchType = $cos.val();
      if (this.searchType === 'Location' || this.searchType === 'ByLocalAuthority' || this.searchType === 'EstablishmentAll') {
        this.searchCategory = 'establishments';
      } else if (this.searchType === 'EstablishmentGroupsAll') {
        this.searchCategory = 'establishment groups';
      } else if (this.searchType === 'GovernorsAll') {
        this.searchCategory = 'governors';
      }
    }

    this.showPreviousAdditionalFilterSelections();
    this.bindEvents();
  }

  shouldShowGovWarning() {
    return $('#option-select-local-authority').find(':checkbox').filter(':checked').length > 0;
  }

  bindEvents() {
    const self = this;
    $(".js-save-set").on("click", (e) => {
      e.preventDefault();
      this.saveFilterSelection();
    });

    $('.radius-filter').find('.filter-button').on('click',
      function (e) {
        e.preventDefault();
        const $filter = $('.radius-filter');
        const errorSummary = $('#js-filter-error-summary');
        errorSummary.addClass('hidden');
        //$filter.find('.govuk-form-group').removeClass('govuk-form-group--error');
        $filter.find('.govuk-error-message').addClass('hidden');
        $filter.find('.govuk-input').removeClass('govuk-input--error');

        const isValid = GiasFilterValidation.validateRadiusFilter();
        console.log(isValid);
        if (!isValid) {
         // $filter.find('.govuk-form-group').addClass('govuk-form-group--error');
          $filter.find('.govuk-error-message').removeClass('hidden');
          $filter.find('.govuk-input').addClass('govuk-input--error');
          $filter.removeClass('hidden');
          errorSummary.find('.summary-radius-error').removeClass('hidden');
          errorSummary.removeClass('hidden').focus();
          window.document.title = "Error: Search results - GOV.UK";
          return;
        }

        self.setParams();
        self.getResults();
      });

    $('.age-filter').find('.filter-button').on('click',
      function (e) {
      console.log('validate age');
        e.preventDefault();
        const ageFilterId = $(this).closest('.age-filter').attr('id');
        let canSubmit = GiasFilterValidation.validateAgeFilter(ageFilterId);
        if (canSubmit) {
            self.setParams();
            self.getResults();
        }
      });

    $('.date-filters').find('.filter-button').on('click',
      function (e) {
        e.preventDefault();
        const dateFilterId = $(this).closest('.date-filters').attr('id');
        const canSubmit =  GiasFilterValidation.validateDateFilters(dateFilterId);
        if (canSubmit) {
          self.setParams();
          self.getResults();
        }
      });

    $(document).on('change', '.trigger-result-update', function() {
      self.setParams();
      const currentInput = this;
      const chxVal = $(this).val();
      const chxName = $(this).prop('name');
      const isChecked = this.checked;
      const filterCount = self.$form.find(':checkbox, select').filter(':checked, :selected').length;

      const similarInput = $('#filter-form').find('.trigger-result-update[name="' + chxName + '"]').filter(function (n, input) {
        if (input.value === chxVal) {
          return input;
        }
      }).not(currentInput);

      if (similarInput.length > 0) {
        similarInput.prop('checked', isChecked);
        window.setTimeout(function () {
          similarInput.parents('.nested-items').data().giasNestedFilters.setPartialState();
        }, 0);
      }

      if (self.filterIntent) {
        window.clearTimeout(self.filterIntent);
      }

     $('#results-container').addClass('pending-results-update');
      this.searchParams = self.deDupeParams(self.$form.find(':input').filter(function (n, ele) {
        return ele.value !== '';
      }).serialize());


      self.filterIntent = window.setTimeout(function () {
        self.getResults();
        // if (DfE.searchMap.currentView === 'map') {
        //   DfE.searchMap.getSearchData();
        // } else {
        //   DfE.searchMap.dataRefreshRequired = true;
        // }
        // if (searchType === 'ByLocalAuthority') {
        //   DfE.searchUtils.updateSearchedLas();
        // }
        // if (searchType === 'GovernorsAll') {
        //   DfE.searchUtils.updateGovernorRoles();
        //
        //   $('#gov-la-warning').addClass('hidden');
        //   if (self.shouldShowGovWarning()) {
        //     $('#gov-la-warning').removeClass('hidden');
        //   }
        // }

      }, 1500);

    });

    $('#clear-filters').on('click', (e)=> {
      e.preventDefault();
      window.clearTimeout(this.filterIntent);
      this.$form.find('input[type="text"]').val('');
      this.$form.find('.clear-selections').click();
      const selectedFilters = this.$form
        .find('.options-container .trigger-result-update')
        .filter(function (n, item) {
          return $(item).prop('checked');
        });
      selectedFilters.prop('checked', false);
      this.$form.find('.select-all').next('label').removeClass('partial-selection');
      this.$form.find('.select-all').prop('checked', false);
      this.$form.find('.filter-group-title').next('label').removeClass('partial-selection');
      this.$form.find('.filter-group-title').prop('checked', false);
      selectedFilters.slice(0, 1).trigger('change');
    });
  }

  deDupeParams(qs) {
    const paramArray = qs.split('&');
    return paramArray.sort().filter(function (item, pos, ary) {
      return !pos || item !== ary[pos - 1];
    }).join('&');
  }

  enableFilters() {
    const $form = $('#filter-form');
    const $filters = $form.find(':checkbox, select');

    $filters.prop('disabled', false);
    $form.find('.filter-clone').prop('disabled', false);
    $form.find('.active-clear').removeClass('clear-disabled');
    $form.find('input[type="text"]').prop('disabled', false);
    $('#filter-additional-controls a').removeClass('hidden');
  }

  disableFilters() {
    const $form = $('#filter-form');
    const $filters = $form.find(':checkbox, select');

    $filters.prop('disabled', true);
    $form.find('.filter-clone').prop('disabled', true);
    $form.find('.active-clear').addClass('clear-disabled');
    $form.find('input[type="text"]').prop('disabled', true);
    $('#results-notification').html('Please wait, loading search results');
  }

  getParams() {
    return this.deDupeParams($('#filter-form').find(':input').filter(function (n, ele) {
      return ele.value !== '';
    }).serialize());
  }

  setParams() {
    this.searchParams = this.deDupeParams($('#filter-form').find(':input').filter(function (n, ele) {
      return ele.value !== '';
    }).serialize());
  }


  getResults() {
    const $resultsContainer = $('#results-container');
    const self = this;
    const $downloadLink = $('.search-results-download-link');
    const downloadBaseUrl = $downloadLink.attr('href').split('?')[0];
    const $resultsNotification = $('#results-notification');
    let token;
    $resultsContainer.html('<div class="progress-indicator"><span class="govuk-visually-hidden">Please wait</span></div>');

    this.disableFilters();

    $('.date-filter-warning').addClass('hidden');

    $.ajax({
      type: "POST",
      url: '/api/tokenize',

      data: self.searchParams,
      success: function (data, status, xhr) {
        token = data.token;
        if (supportsHistory()) {
          history.pushState({}, null, window.location.href.split('?')[0] + '?tok=' + token);
        }

        $.ajax({
          url: 'Search/results-js',
          data: "tok=" + token,
          dataType: 'html',
          success: function (results, status, xhr) {
            let count;
            if (xhr.getResponseHeader("x-count")) {
              count = xhr.getResponseHeader("x-count");
            }
            $resultsContainer.html(results);
            if (count > 0) {
              $resultsNotification.html('Search results loaded. ' + count + ' ' + self.searchCategory + ' found.');
            }
            $downloadLink.attr('href', downloadBaseUrl + '?tok=' + token);
            $downloadLink.removeClass('hidden');
            $resultsContainer.removeClass('pending-results-update');

            if (Number(xhr.getResponseHeader("x-count")) === 0) {
              $downloadLink.addClass('hidden');
              $resultsNotification.html('Search results loaded. No ' + self.searchCategory + ' found.');
            }

            $(window).trigger({
              type: 'ajaxResultLoad',
              count: count
            });



            if (xhr.getResponseHeader("x-show-date-filter-warning") === "true") {
              $('.date-filter-warning').removeClass('hidden');
              if (!self.seenOpenDateWarning) {
                window.scrollTo(0, 0);
                self.seenOpenDateWarning = true;
              }
            }

            $('.js-save-set').removeClass('hidden');
            self.enableFilters();
          }
        });
      },
      error: function (xhr) {
        self.enableFilters();
      }
    });
  }

  saveFilterSelection() {
    const filterCount = this.$form.find(':checkbox, select').filter(':checked, :selected').length;
    let token = null;

    if (filterCount > 0)
      token = QueryString('tok')

    $.ajax({
      url: "/api/save-search-token",
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      method: 'post',
      data: JSON.stringify({
        token: token
      }),
      success: function (data) {
      },
      error: function (jqXHR, textStatus, errorThrown) {
        console.log(errorThrown);
      }
    });
  }

  showPreviousAdditionalFilterSelections() {
    // a hidden input which stores the selected filter ids (aliases) so that
    // we know what additional filters need to be displayed.
    // this also puts a check on the selected inputs
    if (document.getElementById('selected-search-filters')) {
      const aliases = $('#selected-search-filters').val().split('');
      $.each(aliases, function (n, alias) {
        $('.additional-search-critera[data-bind-alias="' + alias + '"]').removeClass('hidden');
        $('#filter-type-target').find('[data-alias="' + alias + '"]').prop('checked', true);
      });
    }
  }
}


export default GiasFiltering;
