import { createApp } from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
import GiasWaitSpinner from '../GiasVueComponents/GiasWaitSpinner';

import GiasTabs from '../GiasModules/GiasTabs';

const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
let raw = [];

const academyOpenings = createApp({
  data: {
    openingAcademies: [],
    selectedDate: '0',
    initialRecordCount: 0,
    currentPage: 0,
    slicePage: 0,
    pageSize: 50,
    pages: [],
    currentCount: 0,
    searchError: false,
    searchUrn: '',
    editRecord: false,
    openDateError: false,
    updateName: '',
    updateNameError: false,
    updateDateDay: '',
    updateDateMonth: '',
    updateDateYear: '',
    selectedOpeningDetails: {},
    presentDetail: false,
    isProcessing: true,
    userHasEdited: false,
    presentExitWarning: false,
    loadDataError: false,
    apiError: {},
    sortKey: 'openingDate',
    sortAscending: true,
    recordUpdateErrors: [],
    anchorTarget: '',
    tableColumns: {
      openingDate: 'Opening date',
      urn: 'URN',
      name: 'Establishment name',
      establishmentType: 'Establishment type',
      predecessorUrn: 'Predecessor URN',
      predecessorName: 'Predecessor name'
    },
    uniqueDates: [],
    raw: [],
    errors: []
  },
  components: {
    GiasWaitSpinner,
    errorSummary
  },
  created: function () {
    this.loadData();
    blockExits();
  },
  methods: {
    sortOpenings: function (key) {

      if (key === this.sortKey) {
        this.sortAscending = !this.sortAscending;
      }
      this.sortKey = key;
      const asc = this.sortAscending === true;

      let sortFn;

      const sortDate = function (a, b) {
        if (asc) {
          return new Date(a.openingDate) - new Date(b.openingDate);
        } else {
          return new Date(b.openingDate) - new Date(a.openingDate);
        }

      };
      const sortNumeric = function (a, b) {
        if (asc) {
          return Number(a[key]) - Number(b[key]);
        } else {
          return Number(b[key]) - Number(a[key]);
        }

      };
      const sortText = function (a, b) {
        const textA = (a[key] !== null) ? a[key].toLowerCase() : '';
        const textB = (b[key] !== null) ? b[key].toLowerCase() : '';
        if (asc) {
          if (textA < textB) {
            return -1;
          }
          if (textA > textB) {
            return 1;
          }
          return 0;
        } else {
          if (textA > textB) {
            return -1;
          }
          if (textA < textB) {
            return 1;
          }
          return 0;
        }

      };

      if (key === 'urn' || key === 'predecessorUrn') {
        sortFn = sortNumeric;
      } else if (key === 'openingDate') {
        sortFn = sortDate;
      } else {
        sortFn = sortText;
      }

      const temp = raw.sort(sortFn);
      this.openingAcademies = temp;
      this.buildPages(this.openingAcademies, this.pageSize);
    },
    detailUrl: function (urn) {
      return '/Establishments/Establishment/Details/' + urn;
    },
    formatDate: function (date, separator) {
      if (!date) {
        date = new Date();
      }
      if (!separator) {
        separator = '/';
      }
      const dd = date.getDate();
      const mm = date.getMonth() + 1;
      const yyyy = date.getFullYear();

      return [yyyy, mm, dd].join(separator);
    },
    showAll: function () {
      this.pageSize = 100000;
      this.setCurrentPage(0);
      this.buildPages(this.openingAcademies, this.pageSize);
    },
    buildDateDropDown: function () {
      let openingDates = [];
      let i;
      let len;
      const self = this;

      document.getElementById('opening-date-filter').innerHTML = '';

      for (i = 0, len = self.currentCount; i < len; i++) {
        const openingDate = new Date(self.openingAcademies[i].openingDate);
        let tempObj = {};

        tempObj.sorter = openingDate.getMonth() + '.' + openingDate.getFullYear();
        tempObj.month = openingDate.getMonth();
        tempObj.year = openingDate.getFullYear();

        openingDates.push(tempObj);
      }

      let spotted = {};
      const uniqueDates = openingDates.filter(function (item) {
        return spotted.hasOwnProperty(item.sorter) ? false : (spotted[item.sorter] = true);
      });

      let frag = document.createDocumentFragment();
      const opt = document.createElement('option');
      opt.value = 0;
      opt.innerHTML = 'All months';
      frag.appendChild(opt);

      for (i = 0, len = uniqueDates.length; i < len; i++) {
        const option = document.createElement('option'),
          d = uniqueDates[i];

        option.value = d.sorter;
        option.innerHTML = monthNames[d.month] + ' ' + d.year;

        frag.appendChild(option);
      }


      document.getElementById('opening-date-filter').appendChild(frag);
      this.selectedDate = 0;
    },
    buildPages: function (changes, pageSize) {
      let count = 0;
      let changesPages = [];
      for (count; count < changes.length;) {
        const page = changes.slice(count, (count + pageSize));

        changesPages.push(page);
        count += pageSize;
      }

      this.pages = changesPages;

      this.isProcessing = false;
      $('#content').find('.gias-tabs__tab').slice(0, 1).click();
    },
    loadData: function () {
      const now = new Date();
      const currentYear = now.getFullYear();
      const nowPlus30 = new Date();
      const self = this;
      let totalRecords = 0;

      now.setDate(1);
      nowPlus30.setDate(1);
      nowPlus30.setFullYear(currentYear + 30);

      let establishmentTypeId=this.getQueryStringValue("establishmentTypeId");

      // step into callback hell
      let params = `${self.formatDate(now, '-')}/${self.formatDate(nowPlus30, '-')}/0/1`;
      params = `${params}/${establishmentTypeId}`;
      $.getJSON(`/api/academy-openings/list/${params}`,
        function (data) {
          totalRecords = data.count;
          self.initialRecordCount = totalRecords;
          self.currentCount = totalRecords;
          params = `${self.formatDate(now, '-')}/${self.formatDate(nowPlus30, '-')}/0/${totalRecords}`;
          params = `${params}/${establishmentTypeId}`;
          $.getJSON(`/api/academy-openings/list/${params}`,
            function (data) {
              self.openingAcademies = data.items;
              self.buildPages(data.items, self.pageSize);
              self.buildDateDropDown();
              raw = data.items;
            }
          ).fail(function (jqxhr) {
            if (jqxhr.hasOwnProperty('responseJSON')) {
              self.apiError = jqxhr.responseJSON;
            } else {
              self.loadDataError = true;
            }
            self.loadDataError = true;
            self.isProcessing = false;
          });
        }
      ).fail(function (jqxhr) {

        if (jqxhr.hasOwnProperty('responseJSON')) {
          self.apiError = jqxhr.responseJSON;
        } else {
          self.loadDataError = true;
        }
        self.isProcessing = false;
      });
    },
    getQueryStringValue: function (queryParameterKey){
      const queryString = window.location.search;
      const urlParams = new URLSearchParams(queryString);
      let value=" "; // This is intentionally set to whitespace so a value is registered to the called Api
      if (urlParams.has(queryParameterKey)) value = urlParams.get(queryParameterKey);

      return value;
    },
    setCurrentPage: function (pageIndex) {
      this.currentPage = pageIndex;
      if (this.currentPage < 3) {
        this.slicePage = 0;
      } else {
        if (this.currentPage > this.pages.length - 3) {
          this.slicePage = this.pages.length - 5;
        } else {
          this.slicePage = this.currentPage - 2;
        }
      }
      if (this.slicePage < 0) {
        this.slicePage = 0;
      }
    },
    editEstab: function (urn) {
      this.searchUrn = urn;
      this.openingDetail();
      this.editRecord = true;
      this.searchError = false;
    },
    validUrns: function () {
      let urns = [];
      let i = 0;
      const len = this.openingAcademies.length;

      for (i; i < len; i++) {
        urns.push(this.openingAcademies[i].urn);
      }

      return urns;
    },
    validateUrn: function () {
      this.clearErrors();
      const validUrns = this.validUrns();
      this.presentDetail = false;
      this.searchError = validUrns.indexOf(Number(this.searchUrn)) === -1;
      if (!this.searchError) {
        this.openingDetail();
        this.presentDetail = true;
      } else {
        this.errors.push({
          href: '#academy-search-field',
          message: 'Please enter a valid <abbr title="Unique Reference Number">URN</abbr>'
        })
        this.errorFocus();
      }
    },
    updateRecord: function () {
      this.clearErrors();
      const d = parseInt(this.updateDateDay, 10);
      const m = parseInt(this.updateDateMonth, 10);
      const y = parseInt(this.updateDateYear, 10);
      const self = this;


      this.updateNameError = this.updateName.length < 1;

      if (this.updateNameError) {
        this.errors.push({
          href: '#est-name',
          message: 'Please enter the establishment name'
        });
      }

      this.openDateError = (
        isNaN(d) || d < 1 || d > 31 || this.updateDateDay === '' ||
        isNaN(m) || m < 1 || m > 12 || this.updateDateMonth === '' ||
        isNaN(y) || y < 2000 || y > 2100 || this.updateDateYear === '');

      if (this.openDateError) {
        this.errors.push({
          href: '#opening-date',
          message: 'Please enter the establishment opening date'
        });

        this.errorFocus();
      }

      if (!this.updateNameError && !this.openDateError) {
        const urn = this.searchUrn;
        this.searchUrn = '';
        this.presentDetail = false;
        this.editRecord = false;
        this.isProcessing = true;
        this.userHasEdited = false;
        this.recordUpdateErrors = [];

        $(window).off('beforeunload');

        $.ajax({
          url: '/api/academy/' + urn,
          type: 'post',
          contentType: 'application/json; charset=utf-8',
          dataType: 'json',
          data: JSON.stringify({
            openDate: [this.updateDateYear, this.updateDateMonth, this.updateDateDay].join('-'),
            name: this.updateName
          }),
          success: function (data) {
            self.loadData();
          },
          error: function (jqxhr) {

            self.searchUrn = urn;
            self.presentDetail = true;
            self.editRecord = true;
            self.isProcessing = false;
            self.userHasEdited = true;

            if (jqxhr.hasOwnProperty('responseJSON')) {
              self.recordUpdateErrors = jqxhr.responseJSON.errors;
              for (let i = 0, len = Object.keys(self.recordUpdateErrors).length; i < len; i++) {
                self.errors.push({
                  href: '#',
                  message: self.recordUpdateErrors[i].message
                });
              }

            } else {
              self.loadDataError = true;
            }
          }
        });
      }
    },
    buildDatePages: function () {
      const dateParts = this.selectedDate.split('.');
      const month = parseInt(dateParts[0], 10);
      const year = parseInt(dateParts[1], 10);

      if (isNaN(month) || isNaN(year)) { // user selected all
        this.buildPages(this.openingAcademies, this.pageSize);
        this.currentCount = this.openingAcademies.length;
        return;
      }

      const datePages = this.openingAcademies.filter(function (opening) {
        const oDateParts = opening.openingDate.split('-');
        const oDateMonth = parseInt(oDateParts[1], 10) - 1;
        const oDateYear = parseInt(oDateParts[0], 10);

        return oDateMonth === month && oDateYear === year;

      });

      this.buildPages(datePages, this.pageSize);
      this.currentCount = datePages.length;
      this.currentPage = 0;
    },
    openingDetail: function () {
      const urn = parseInt(this.searchUrn, 10);
      if (isNaN(urn)) {
        return false;
      }
      const academy = this.openingAcademies.filter(function (estab) {
        return parseInt(estab.urn, 10) === urn;
      });
      const openDate = academy[0].openingDate.split('-');

      this.updateName = academy[0].name;
      this.updateDateDay = openDate[2].replace('T00:00:00', '');
      this.updateDateMonth = openDate[1];
      this.updateDateYear = openDate[0];

      this.selectedOpeningDetails = academy[0];
      return this.selectedOpeningDetails;
    },
    cancelEditClick: function () {
      this.recordUpdateErrors = [];
      if (this.userHasEdited) {
        window.setTimeout(function () {
          $('#button-ok').focus();

        }, 0);
        return this.presentExitWarning = true;
      }
      this.editRecord = false;
      this.searchUrn = '';
      this.presentDetail = false;
    },
    isUserEditing: function () {
      return this.userHasEdited;
    },
    exitWarningOkClick: function () {
      this.clearErrors();
      $(window).off('beforeunload');
      if (this.anchorTarget === '') {
        this.presentExitWarning = false;
        this.userHasEdited = false;
        this.editRecord = false;
        this.searchUrn = '';
        this.presentDetail = false;
      } else {
        window.location.href = this.anchorTarget;
      }
    },
    errorFocus: function () {
      window.setTimeout(function () {
        window.document.title = "Error: Manage academy openings - GOV.UK";
        $('.error-summary').focus();
      }, 0);
    },
    clearErrors: function () {
      window.document.title = "Manage academy openings - GOV.UK";
      this.searchError = false;
      this.updateNameError = false;
      this.openDateError = false;
      this.errors = [];
    },
    attachUnload: function () {
      this.userHasEdited = true;
      $(window).on('beforeunload', function (e) {
        return 'Any unsaved changes will be lost';
      });
    },

  },

  computed: {
    paginationDescription: function () {
      const starting = this.currentPage * this.pageSize + 1;
      let ending = this.currentPage * this.pageSize + this.pageSize;

      ending = ending > this.currentCount ? this.currentCount : ending;

      return starting + " - " + ending;
    },
    page: function () {
      return this.pages[this.currentPage];
    }
  }

});


$(window).on('tabChange', function () {
  academyOpenings.clearErrors();
});
$('#main-content').find('.gias-tabs-wrapper').giasTabs();
$('#main-content').find('.gias-tabs__tab').eq(0).click();

function blockExits() {
  $('a').on('click', function (e) {
    if (academyOpenings.isUserEditing()) {
      e.preventDefault();
      academyOpenings.presentExitWarning = true;
      academyOpenings.anchorTarget = $(this).attr('href');
      window.setTimeout(function () {
        $('#button-ok').focus();
      }, 0);
    }
  });
}

academyOpenings.mount('#academy-opening-app');

