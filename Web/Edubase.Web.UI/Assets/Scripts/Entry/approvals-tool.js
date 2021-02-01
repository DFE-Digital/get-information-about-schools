import Vue from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
import GiasWaitSpinner from '../GiasVueComponents/GiasWaitSpinner';

import GiasTextCounter from '../GiasModules/GiasTextCounter';

const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

new Vue({
  el: '#change-approvals',
  components: {
    errorSummary,
    GiasWaitSpinner
  },
  data: {
    changes: [],
    apiUrl: '/api/approvals/change-requests',
    confirmUrl: '/api/approvals/change-request',
    reason: '',
    currentPage: 0,
    slicePage: 0,
    pages: [],
    initialCount: 0,
    currentCount: 0,
    startIndex: 0,
    pageSize: 10,
    invalidReason: false,
    reasonLength: false,
    pendingRejection: false,
    itemsConfirmedRemoved: false,
    itemsConfirmedRejected: false,
    approvalMessage: '',
    rejectionMessage: '',
    isProcessing: true,
    apiError: '',
    apiBork: {},
    sortAscending: true,
    sortType: 'effectiveDateUtc',
    cannedRejections: window.GIASRejections || [],
    showRejections: false,
    noReasonSelectedError: false,
    reasonIds: [],
    noItemsSelected: true,
    tableColumns: {
      establishmentUrn: 'URN',
      establishmentLAESTAB: 'DfE Number',
      establishmentName: 'Establishment',
      fieldName: 'Updated field',
      oldValue: 'Old value',
      newValue: 'New value',
      effectiveDateUtc: 'Effective date',
      requestedDateUtc: 'Date requested',
      originatorFullName: 'Suggested by',
    },

    errors: [],
  },
  created: function () {
    this.getChangesData();

  },
  mounted: function (){
    this.counter = new GiasTextCounter(document.getElementById('reason'), {maxLength: 1024});
  },
  computed: {
    pageCount: function () {
      return this.currentPage * this.pageSize;
    },
    paginationDescription: function () {
      const starting = this.currentPage * this.pageSize + 1;
      let ending = this.currentPage * this.pageSize + this.pageSize;

      ending = ending > this.currentCount ? this.currentCount : ending;

      return starting + " - " + ending;
    },
    totalPages: function () {
      return Math.ceil(this.currentCount / this.pageSize);
    },
    visiblePagination: function () {
      const maxPages = 5;
      let paginationStart = 1;
      let paginationFinish = this.totalPages;

      if (this.totalPages > maxPages) {
        if (this.currentPage < 4) {
          paginationFinish = maxPages;
        }
        else if (this.currentPage >= 4 && !(this.currentPage + 3 > this.totalPages)) {
          paginationFinish = this.currentPage + 2;
          paginationStart = paginationFinish - (maxPages - 1);
        }
        else if (this.currentPage + 3 > this.totalPages) {
          paginationFinish = this.totalPages;
          paginationStart = this.totalPages - (maxPages - 1);
        }
      }

      let paginationItems = [];
      for (let i = paginationStart; i <= paginationFinish; i++) {
        paginationItems.push(i);
      }

      return paginationItems;
    },
    isZeroItemsSelected: function () {
      return $('#changes-table').find('.govuk-checkboxes__input').filter(':checked').length === 0;
    }
  },
  methods: {
    formatValue(dateString) {
      const RE = /^\d{1,2}\/\d{1,2}\/\d{2,4}$/;
      if (!RE.test(dateString)) {
        return dateString;
      }

      const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
      const dateSplit = dateString.split('/');
      const monthInt = parseInt(dateSplit[1],10) -1;

      return `${dateSplit[0]} ${monthNames[monthInt]} ${dateSplit[2]}`;
    },
    showRejectionsModal: function () {
      this.clearErrors();
      this.showRejections = true;
      window.setTimeout(function () {
        $('#modal-label').focus();
      }, 0);
    },
    selectItem: function () {
      this.noItemsSelected = ($('#changes-table').find('.govuk-checkboxes__input').filter(':checked').length === 0);
    },
    selectReason: function () {
      let reasonText = this.reason;
      const self = this;
      this.noReasonSelectedError = false;

      if (this.reasonIds.length === 0) {
        return this.noReasonSelectedError = true;
      }
      if (reasonText.length > 0) {
        reasonText = reasonText + '\n\n';
      }

      for (let i = 0, len = this.reasonIds.length; i < len; i++) {
        const reason = this.cannedRejections.filter(function (r) {
          if (r.id === Number(self.reasonIds[i])) {
            return r;
          }
        })[0];

        reasonText += reason.title + '\n' + reason.content + '\n\n';

      }
      this.reason = reasonText;
      this.reasonIds = [];
      this.showRejections = false;
      $('#reason').focus().trigger('drop');
    },
    setSort: function (sort) {
      if (sort === this.sortType) {
        this.sortAscending = !this.sortAscending;
      } else {
        this.sortType = sort;
      }

      this.getChangesData();
    },
    detailUrl: function (urn) {
      return '/Establishments/Establishment/Details/' + urn;
    },
    formatDate: function (utcDate) {
      if (utcDate === null) {
        return 'unknown';
      }
      var d = new Date(utcDate);
      return [d.getDate(), monthNames[d.getMonth()], d.getFullYear()].join(' ');
    },
    getChangesData: function (skip, callback) {
      const self = this;
      this.isProcessing = true;
      const sortDir = this.sortAscending ? '-asc' : '-desc';
      $('#changes-table').find(':checkbox').prop('checked', false);

      $.ajax({
        url: self.apiUrl,
        data: {
          take: self.pageSize,
          skip: skip || 0,
          sortBy: self.sortType + sortDir
        },
        success: function (data) {
          self.currentCount = data.count;
          self.changes = data.items;
          self.isProcessing = false;
          if (callback) {
            callback.call(self);
          }
        },
        error: function (jqxhr) {
          if (jqxhr.hasOwnProperty('responseJSON')) {
            self.apiBork = jqxhr.responseJSON;
          }
        }
      });
    },
    approveSuccessCallback: function () {
      this.itemsConfirmedRemoved = true;
      this.approvalMessage = 'Items approved. The editor has been notified by email.'
    },
    rejectSuccessCallBack: function () {
      const self = this;
      this.pendingRejection = false;
      this.itemsConfirmedRejected = true;
      this.rejectionMessage = 'Items rejected. The editor has been notified by email.';
      this.reason = '';
      window.setTimeout(function () {
        self.counter.setCount();
      }, 0);

    },
    updateCount: function (removedCount) {
      this.currentCount = this.currentCount - removedCount;
    },
    setCurrentPage: function (pageIndex) {
      this.currentPage = pageIndex;
      this.getChangesData(pageIndex * this.pageSize);

    },
    showAll: function () {
      this.currentPage = 0;
      this.pageSize = 100000;
      this.getChangesData();
    },
    confirmRejection: function () {
      this.clearErrors();
      const self = this;
      const $reason = $('#reason');
      this.apiError = '';
      this.invalidReason = $reason.val().length < 1;
      this.reasonLength = $reason.val().length > 1024;

      if ($reason.val().length === 0) {
        this.errors.push({
          href: '#reason',
          message: 'Please enter a reason for the rejection'
        });
      } else if ($reason.val().length > 1024) {
        this.errors.push({
          href: '#reason',
          message: 'Please enter a reason less than 1024 characters'
        });
      }

      const selectedItems = $('#changes-table').find('.govuk-checkboxes__input')
        .filter(':checked');

      // this.noneSelectedError = selectedItems.length === 0;
      if (selectedItems.length === 0) {
        this.errors.push({
          href: '#changes-table',
          message: 'Please select some changes to reject'
        });
      }


      if (this.errors.length > 0) {
        this.errorFocus();
      } else {
        this.isProcessing = true;
        let removedIds = selectedItems.map(function (n, item) {
          return Number(item.value);
        });
        removedIds = $.makeArray(removedIds);

        $.ajax({
          url: self.confirmUrl,
          contentType: 'application/json; charset=utf-8',
          dataType: 'json',
          method: 'post',
          data: JSON.stringify({
            action: 'reject',
            'ids': removedIds,
            rejectionReason: $('#reason').val()
          }),
          success: function (data) {
            self.getChangesData(0, self.rejectSuccessCallBack);
          },
          error: function (jqXHR, textStatus, errorThrown) {
            const responses = JSON.parse(jqXHR.responseText);

            for (let i = 0, len = responses.length; i < len; i++) {
              self.errors.push({
                href: '#',
                message: responses[i].message
              });
            }
            self.isProcessing = false;

            if (jqXHR.hasOwnProperty('responseJSON')) {
              self.apiBork = jqXHR.responseJSON;
            }
          }
        });
      }
    },
    approveSelection: function () {
      this.clearErrors();
      const self = this;
      this.apiError = '';
      const selectedItems = $('#changes-table').find('.govuk-checkboxes__input')
        .filter(':checked');

      //this.approvalError = (selectedItems.length === 0);
      if (selectedItems.length === 0) {
        this.errors.push({
          href: '#changes-table',
          message: 'Please select some changes to approve'
        });
      }

      this.itemsConfirmedRejected = false;
      this.itemsConfirmedRemoved = false;


      if (this.errors.length > 0) {
        this.errorFocus();
      } else {
        this.isProcessing = true;
        let removedIds = selectedItems.map(function (n, item) {
          return Number(item.value);
        });

        removedIds = $.makeArray(removedIds);

        $.ajax({
          url: self.confirmUrl,
          contentType: 'application/json; charset=utf-8',
          dataType: 'json',
          method: 'post',
          data: JSON.stringify({
            action: 'approve',
            'ids': removedIds
          }),
          success: function (data) {
            self.getChangesData(0, self.approveSuccessCallback);

          },
          error: function (jqXHR, textStatus, errorThrown) {
            const responses = JSON.parse(jqXHR.responseText);
            for (var i = 0, len = responses.length; i < len; i++) {
              messages.push(responses[i].message);
              self.errors.push({
                href: '#',
                message: responses[i].message
              });
            }
            self.isProcessing = false;

            if (jqXHR.hasOwnProperty('responseJSON')) {
              self.apiBork = jqXHR.responseJSON;
            }
          }
        });
      }
    },
    rejectSelection: function () {
      this.clearErrors();

      const selectedItems = $('#changes-table').find('.govuk-checkboxes__input')
        .filter(':checked');
      if (selectedItems.length === 0) {
        this.errors.push({
          href: '#changes-table',
          message: 'Please select some changes to reject'
        });
      }

      this.errorFocus();

      if (this.errors.length > 0) {
        return;
      } else {
        this.pendingRejection = true;
        this.reason = '';
        this.counter.setCount();
        this.clearErrors();
      }

    },
    errorFocus: function () {
      const $eSummary = $('.error-summary');
      if ($eSummary.length) {
        window.document.title = "Error: Review and approve changes - GOV.UK";
        $('.error-summary').focus();
      } else {
        window.setTimeout(function () {
          if ($eSummary.length) {
            window.document.title = "Error: Review and approve changes - GOV.UK";
            $('.error-summary').focus();
          }
        }, 500);
      }
    },
    clearErrors: function () {
      window.document.title = "Review and approve changes - GOV.UK";
      this.errors = [];
      this.invalidReason = false;
      this.reasonLength = false;
    }
  }

});
