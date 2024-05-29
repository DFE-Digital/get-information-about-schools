import QueryString from "../GiasHelpers/QueryString";
import supportsHistory from "../GiasHelpers/supportsHistory";
import GiasFilterValidation from "./GiasFilterValidation";
import {cons} from "@most/prelude";
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
    this.$filterSetradios = $('#save-filter-options').find('input')
    this.savedSelections = this.$form.find(':input').serializeArray();
    this.pauseAutoSelection = false;

    if ($cos) {
      this.searchType = $cos.val();
      if (this.searchType === 'Location' || this.searchType === 'ByLocalAuthority' || this.searchType === 'EstablishmentAll') {
        this.searchCategory = 'establishments';
      } else if (this.searchType === 'EstablishmentGroupsAll') {
        this.searchCategory = 'establishment groups';

      } else if (this.searchType === 'GovernorsAll') {
        this.searchCategory = 'governors';
        this.updateGovernorsDescription();
      }
      if (this.searchType === 'ByLocalAuthority') {
        this.updateLaDescription();
      }
    }

    this.showPreviousAdditionalFilterSelections();
    this.bindEvents();
  }

  shouldShowGovWarning() {
    return $('#option-select-local-authority').find(':checkbox').filter(':checked').length > 0;
  }

  updateLaDescription() {
    let laCount = 0;
    let selectedLas = [];
    $('#option-select-local-authority').find('.trigger-result-update').filter(':checked').each(function () {
      const label = $(this).parent().clone();
      label.find('span, input').remove();
      const text = label.text();
      selectedLas.push('&lsquo;<span class="govuk-!-font-weight-bold">' + $.trim(text) + '</span>&rsquo;');
      laCount++;
    });
    selectedLas = selectedLas.join(', ');
    const lastComma = selectedLas.lastIndexOf(',');
    if (laCount > 1) {
      selectedLas = selectedLas.substring(0, lastComma) +
        ' and ' +
        selectedLas.substring(lastComma + 1, selectedLas.length);
    }

    $('#la-list').html(selectedLas);
  }

  updateGovernorsDescription() {
    let selectedGovRoles = [];
    const selectedRoles = $('#option-select-role-type').find('.trigger-result-update').filter(':checked');
    if (selectedRoles.length > 0) {
      selectedRoles.each(function () {
        const label = $(this).parent().clone();
        label.find('span, input').remove();
        const text = label.text();
        selectedGovRoles.push('&lsquo;<span class="govuk-!-font-weight-bold">' + $.trim(text) + '</span>&rsquo;');
      });
      selectedGovRoles = selectedGovRoles.join(', ');
      const lastComma = selectedGovRoles.lastIndexOf(',');
      selectedGovRoles = selectedRoles.length > 1 ? selectedGovRoles.substring(0, lastComma) +
        ' and ' +
        selectedGovRoles.substring(lastComma + 1, selectedGovRoles.length) : selectedGovRoles;

      $('.conjunction-text').removeClass('hidden');
    } else {
      selectedGovRoles = '';
      $('.conjunction-text').addClass('hidden');
    }

    $('.governor-roles').eq(0).html(selectedGovRoles);
    $('.governor-roles').slice(1).addClass('hidden');

    $('#gov-la-warning').addClass('hidden');
    if (this.shouldShowGovWarning()) {
      $('#gov-la-warning').removeClass('hidden');
    }
  }
  bindEvents() {
    const self = this;
    $("#gias-filterset--save-button").on("click", (e) => {
      e.preventDefault();
      this.saveFilterSelection();
    });

    $('#gias-filterset--delete-button').on('click', (e)=>{
      e.preventDefault();
      const $savedTokenInput = $('#SavedFilterToken');
      const messagePanel = $('#gias-filterset--delete-container').find('.gias-filter-save--alert')
      const message =  $savedTokenInput.val() === '' ? 'You have no saved filter set': 'Filter set deleted';
      messagePanel.html(message);
      messagePanel.removeClass('hidden');
      window.setTimeout(function (){
        messagePanel.addClass('hidden');
        $('#gias-filterset--delete-container').addClass('hidden');
      }, 5000);

      $savedTokenInput.val('');
      this.savedSelections = this.$form.find(':input').serializeArray();
      $('#filter-set-saved').prop('disabled', 'disabled');
      if (document.getElementById('filter-set-all')) {
        $('#filter-set-all').prop('checked', true).trigger('change');
      } else {
        $('#filter-set-custom').prop('checked', true);
      }

      const token = $('input[name="__RequestVerificationToken"').val();

      $.ajax({
        url: '/api/save-search-token',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        method: 'post',
        data: JSON.stringify({
          token: null
        }),
        headers: {
          'RequestVerificationToken': token
        },
        success: function (data) {
        },
        error: function (jqXHR, textStatus, errorThrown) {
          console.log(errorThrown);
        }
      });
    });

    $('.radius-filter').find('.filter-button').on('click',
      function (e) {
        e.preventDefault();
        const $filter = $('.radius-filter');
        const errorSummary = $('#js-filter-error-summary');
        errorSummary.addClass('hidden');
        $filter.find('.govuk-error-message').addClass('hidden');
        $filter.find('.govuk-input').removeClass('govuk-input--error');

        const isValid = GiasFilterValidation.validateRadiusFilter();
        if (!isValid) {
          $filter.find('.govuk-error-message').removeClass('hidden');
          $filter.find('.govuk-input').addClass('govuk-input--error');
          $filter.removeClass('hidden');
          errorSummary.find('.summary-radius-error').removeClass('hidden');
          errorSummary.removeClass('hidden').focus();
          window.document.title = "Error: Search results - GOV.UK";
          window.setTimeout(function(){
            self.enableFilters();
          },0);
          return;
        }

        self.setParams();
        self.getResults();
      });

    function resetGlobalError() {
      const errorSummary = $('#js-filter-error-summary');
      errorSummary.addClass('hidden');
      errorSummary.find('li').addClass('hidden');
    }

    $('.age-filter').find('.filter-button').on('click',
      function (e) {
        e.preventDefault();
        resetGlobalError();
        const $filter = $(this).closest('.age-filter');
        const ageFilterId = $filter.attr('id');
        let canSubmit = GiasFilterValidation.validateAgeFilter(ageFilterId);
        $filter.find('.govuk-input').removeClass('govuk-input--error');
        $filter.find('.govuk-error-message').addClass('hidden');

        if (canSubmit) {
            self.setParams();
            self.getResults();
        } else {
          $filter.find('.govuk-input').addClass('govuk-input--error');
          $filter.find('.govuk-error-message').removeClass('hidden');
          window.setTimeout(function(){
            self.enableFilters();
          },0);
        }
      });

    $('.date-filters').find('.filter-button').on('click',
      function (e) {
        e.preventDefault();
        const $filter = $(this).closest('.date-filters');
        const dateFilterId = $filter.attr('id');
        const canSubmit =  GiasFilterValidation.validateDateFilters(dateFilterId);

        if (canSubmit) {
          self.setParams();
          self.getResults();
        } else {
          window.setTimeout(function(){
            self.enableFilters();
          },0);
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

      if (!self.pauseAutoSelection) {
        $('#filter-set-custom').prop('checked', true).trigger('change');
      }

      self.filterIntent = window.setTimeout(function () {
        self.getResults();
        if (self.searchType === 'ByLocalAuthority') {
          self.updateLaDescription();
        }
        if (self.searchType === 'GovernorsAll') {
          self.updateGovernorsDescription();
        }

      }, 1500);

    });

    $('#clear-filters, #clear-filters-additional').on('click', (e)=> {
      e.preventDefault();
      window.clearTimeout(this.filterIntent);
      this.$form.find('input[type="text"]').val('');
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
      this.$form.find('.govuk-option-select').each(function(n, container){
        $(container).find('.trigger-result-update').slice(0, 1).trigger('change');
      });
    });

    const $openSelector = $("#b_1");
    const $openClosedSelector = $("#b_4");

    $($openSelector).change(function () {
      if (this.checked) {
        $openClosedSelector.prop('checked', true);
      }
    });

    const $groupOpenSelector = $("#group-status_1");
    const $groupOpenClosedSelector = $("#group-status_4");

    $($groupOpenSelector).change(function () {
      if (this.checked) {
        $groupOpenClosedSelector.prop('checked', true);
      }
    });

    self.$filterSetradios.on('change',()=>{
      self.handleFilterSetChange()
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


  getResults(_token) {
    const $resultsContainer = $('#results-container');
    const self = this;
    const $downloadLink = $('.search-results-download-link');
    const downloadBaseUrl = $downloadLink.attr('href').split('?')[0];
    const $resultsNotification = $('#results-notification');
    let token = _token;

    function requestResults(token){
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
          $('#button-loader').remove();
          $('#gias-mobile-filter-submit').append("<span class='mobile-count'> ("+ count+")</span>");
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
          window.gMap.refreshMap();
        }
      });
    }
    $resultsContainer.html('<div class="gias-wait-mask gias-wait-mask--inline"><div class="lds-spinner"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div><span class="govuk-visually-hidden">Please wait</span></div>');

    this.disableFilters();
    $('#gias-mobile-filter-submit').find('.mobile-count').remove();
    $('#gias-mobile-filter-submit').append("<span class='gias-button-loader' id='button-loader'>&nbsp;</span>");

    $('.date-filter-warning').addClass('hidden');

    if (token) {
      requestResults(token);

    } else {

      function getAntiForgeryToken() {
        return $('meta[name="csrf-token"]').attr('content');
      }

      const csrfToken = getAntiForgeryToken();

      $.ajax({
        type: "POST",
        url: '/api/tokenize',

        data: self.searchParams,
        headers: {
          'RequestVerificationToken': csrfToken
        },
        success: function (data, status, xhr) {
          token = data.token;
          if (supportsHistory()) {
            history.pushState({}, null, window.location.href.split('?')[0] + '?tok=' + token);
          }

          requestResults(token);
        },
        error: function (xhr) {
          self.enableFilters();
        }
      });
    }
  }

  saveFilterSelection() {
    const filterCount = this.$form.find(':checkbox, select').filter(':checked, :selected').length;
    let token = null;
    this.savedSelections = this.$form.find(':input').serializeArray();
    const messagePanel = $('#gias-filterset--save-container').find('.gias-filter-save--alert');

    if (filterCount === 0) {
      messagePanel.html('Choose filters');
      messagePanel.removeClass('hidden');
      window.setTimeout(function (){
        messagePanel.addClass('hidden');
      }, 4000);

      return true;
    }

    if (filterCount > 0) {
      token = QueryString('tok')
    }
    document.getElementById('SavedFilterToken').value = token;
    document.getElementById('filter-set-saved').removeAttribute('disabled');

    messagePanel.html('Filter set saved');
    messagePanel.removeClass('hidden');
    window.setTimeout(function (){
      messagePanel.addClass('hidden');
    }, 5000);

    function getCookie(name) {
      const value = '; ${document.cookie}';
      const parts = value.split('; ${name}=');
      if (parts.length === 2) return parts.pop().split(';').shift();
    }

    const verifyToken = getCookie('__RequestVerificationToken');

    $.ajax({
      url: "/api/save-search-token",
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      method: 'post',
      data: JSON.stringify({
        token: token
      }),
      headers: {
        'RequestVerificationToken': verifyToken
      },
      success: function (data) {
      },
      error: function (jqXHR, textStatus, errorThrown) {
        console.log(errorThrown);
      }
    });
  }

  restoreFilterSelections() {
    const self = this;
    const len = self.savedSelections.length;
    $.each(self.savedSelections, function(n, selection) {
      const _input = self.$form.find('[name="'+ selection.name+'"][value="'+selection.value+'"]');

      if (_input.attr('type') === 'checkbox') {
        _input.prop('checked', true);
      } else {
        _input.value = selection.value;
      }

      $('.nested-items').each(function() {
        $(this).data().giasNestedFilters.setPartialState();
      });
    });
  }

  handleFilterSetChange() {
    this.pauseAutoSelection = true;
    const radioValue = this.$filterSetradios.filter(':checked').val();
    const savePanel = $('#gias-filterset--save-container');
    const deletePanel = $('#gias-filterset--delete-container');
    const $clearBtn = $('#clear-filters');
    const self = this;

    savePanel.addClass('hidden');
    deletePanel.addClass('hidden');
    switch (radioValue) {
      case 'all':
        $clearBtn.click();
        if (this.searchType === 'ByLocalAuthority') {
          $('#d_all').prop('checked', true).trigger('change');
        }
        break;

      case 'open' :
        $clearBtn.click();
        if (this.searchType === 'ByLocalAuthority') {
          $('#d_all').prop('checked', true).trigger('change');
        }

        $("#b_1").prop('checked', true).trigger('change');

        break;

      case 'custom':
        savePanel.removeClass('hidden');
        break;

      case 'saved':
        const token = document.getElementById('SavedFilterToken').value;
        const windowToken = QueryString('tok');
        function replaceQueryParam(param, newval, search) {
          const regex = new RegExp("([?;&])" + param + "[^&;]*[;&]?");
          const query = search.replace(regex, "$1").replace(/&$/, '');

          return (query.length > 2 ? query + "&" : "?") + (newval ? param + "=" + newval : '');
        }

        if (token !== windowToken) {
          if (windowToken !== null){
            window.location.search = replaceQueryParam('tok', token, window.location.search);
          } else {
            window.location.search = `tok=${token}`;
          }

        } else {
          this.restoreFilterSelections();
          deletePanel.removeClass('hidden');
        }

        break;
    }
    window.setTimeout(function(){
      self.pauseAutoSelection = false;
    },1000);

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
