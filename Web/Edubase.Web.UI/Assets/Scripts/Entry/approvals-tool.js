import Vue from 'vue';
import axios from "axios";
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
      selectedItems: [],
      apiUrl: '/api/approvals/change-requests',
      confirmUrl: '/api/approvals/change-request',
      reason: '',
      currentPage: 0,
      slicePage: 0,
      pages: [],
      initialCount: 0,
      currentCount: 0,
      startIndex: 0,
      pageSize: 100,
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
  mounted: function () {
    this.counter = new GiasTextCounter(document.getElementById('reason'), { maxLength: 1024 });
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
  },
  methods: {
    formatValue(dateString) {
      const RE = /^\d{1,2}\/\d{1,2}\/\d{2,4}$/;
      if (!RE.test(dateString)) {
        return dateString;
      }

      const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
      const dateSplit = dateString.split('/');
      const curdate = parseInt(dateSplit[0], 10);
      const monthInt = parseInt(dateSplit[1], 10) - 1;

      return `${curdate} ${monthNames[monthInt]} ${dateSplit[2]}`;
    },
    showRejectionsModal: function () {
      this.clearErrors();
      this.showRejections = true;
      window.setTimeout(function () {
        document.getElementById('modal-label').focus();
      }, 0);
    },
    selectItem: function () {
      this.noItemsSelected = this.selectedItems.length == 0;
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
      document.getElementById('reason').focus().trigger('drop');
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

      this.selectedItems = [];

      const params = {
        take: self.pageSize,
        skip: skip || 0,
        sortBy: self.sortType + sortDir
      }

      axios.get(self.apiUrl, { params })
        .then(res => {
          self.currentCount = res.data.count;
          self.changes = res.data.items;
          self.isProcessing = false;
          if (callback) {
            callback.call(self);
          }
        })
        .catch(error => {
          if (error.response) {
            self.apiBork = error.response.data;
          }
          self.isProcessing = false;
        })
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
      const reason = document.getElementById('reason');
      this.apiError = '';
      this.invalidReason = reason.value.length < 1;
      this.reasonLength = reason.value.length > 1024;

      if (reason.value.length === 0) {
        this.errors.push({
          href: '#reason',
          message: 'Please enter a reason for the rejection'
        });
      } else if (reason.value.length > 1024) {
        this.errors.push({
          href: '#reason',
          message: 'Please enter a reason less than 1024 characters'
        });
      }

      if (this.selectedItems.length === 0) {
        this.errors.push({
          href: '#changes-table',
          message: 'Please select some changes to reject'
        });
      }

      if (this.errors.length > 0) {
        this.errorFocus();
      } else {
        this.isProcessing = true;

        const params = {
          action: 'reject',
          'ids': this.selectedItems,
          rejectionReason: document.getElementById('reason').value
        };

        const headers = {
          'Content-Type': 'application/json; charset=utf-8',
          'Data-Type': 'json',
        }

        axios.post(self.confirmUrl, params, {
          headers: headers
        })
        .then(response => {
          self.getChangesData(0, self.rejectSuccessCallBack);
        })
        .catch(error => {
          if (error.response) {
            const responses = error.response.data;

            for (let i = 0, len = responses.length; i < len; i++) {
              self.errors.push({
                href: '#',
                message: responses[i].message
              });
            }

            self.apiBork = error.response.data;
            self.isProcessing = false;
          }
        });
      }
    },
    approveSelection: function () {
      this.clearErrors();
      const self = this;
      this.apiError = '';

      if (this.selectedItems.length === 0) {
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

        const params = {
          action: 'approve',
          'ids': this.selectedItems
        };

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        axios.post(self.confirmUrl, params, {
          headers: {
            'RequestVerificationToken': token,
            'Content-Type': 'application/json; charset=utf-8',
            'Data-Type': 'json'
          }
        })
        .then(response => {
          self.getChangesData(0, self.approveSuccessCallback);
        })
        .catch(error => {
          if (error.response) {
            const responses = error.response.data;

            for (let i = 0, len = responses.length; i < len; i++) {
              self.errors.push({
                href: '#',
                message: responses[i].message
              });
            }

            self.apiBork = error.response.data;
            self.isProcessing = false;
          }
        });
      }
    },
    rejectSelection: function () {
      this.clearErrors();

      if (this.selectedItems.length === 0) {
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
      const eSummary = document.getElementsByClassName('error-summary');
      if (eSummary.length) {
        window.document.title = "Error: Review and approve changes - GOV.UK";
        eSummary.focus();
      } else {
        window.setTimeout(function () {
          if (eSummary.length) {
            window.document.title = "Error: Review and approve changes - GOV.UK";
            eSummary.focus();
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

