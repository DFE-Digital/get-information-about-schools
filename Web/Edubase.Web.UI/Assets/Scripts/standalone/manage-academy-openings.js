(function () {
    var uniqueDates = [],
        academyOpenings,
        i,
        len,
        raw = [],
        monthNames = [
            'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October',
            'November', 'December'
        ];

  
    academyOpenings = new Vue({
        el: '#academy-opening-app',
        data: {
            openingAcademies: [],
            selectedDate: '0',
            initialRecordCount: 0,
            currentPage: 0,
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
            od: {},
            presentDetail: false,
            isProcessing: true,
            userHasEdited: false,
            presentExitWarning: false,
            loadDataError: false,
            apiError: {},
            sortKey: 'openingDate',
            sortAscending: true,
            recordUpdateErrors: {}
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
                var asc = this.sortAscending === true;

                var sortFn;

                var sortDate = function (a, b) {
                    if (asc) {
                        return new Date(a.openingDate) - new Date(b.openingDate);
                    } else {
                        return new Date(b.openingDate) - new Date(a.openingDate);
                    }

                };
                var sortNumeric = function (a, b) {
                    if (asc) {
                        return Number(a[key]) - Number(b[key]);
                    } else {
                        return Number(b[key]) - Number(a[key]);
                    }

                };
                var sortText = function (a, b) {
                    var textA = (a[key]!== null)? a[key].toLowerCase(): '';
                    var textB = (b[key] !== null)? b[key].toLowerCase() : '';
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

                var temp = raw.sort(sortFn);
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
                var dd = date.getDate();
                var mm = date.getMonth() + 1;
                var yyyy = date.getFullYear();

                return [yyyy, mm, dd].join(separator);
            },
            showAll: function () {
                this.pageSize = 100000;
                this.setCurrentPage(0);
                this.buildPages(this.openingAcademies, this.pageSize);
            },
            buildDateDropDown: function () {
                var openingDates = [];
                var i;
                var self = this;

                document.getElementById('opening-date-filter').innerHTML = '';

                for (i = 0, len = self.currentCount; i < len; i++) {
                    var openingDate = new Date(self.openingAcademies[i].openingDate);
                    var tempObj = {},
                        year = openingDate.getFullYear(),
                        month = openingDate.getMonth(),
                        sorter = month + '.' + year;

                    tempObj.sorter = sorter;
                    tempObj.month = month;
                    tempObj.year = year;

                    openingDates.push(tempObj);
                }

                var spotted = {};
                uniqueDates = openingDates.filter(function (item) {
                    return spotted.hasOwnProperty(item.sorter) ? false : (spotted[item.sorter] = true);
                });

                var frag = document.createDocumentFragment();
                var opt = document.createElement('option');
                opt.value = 0;
                opt.innerHTML = 'All months';
                frag.appendChild(opt);

                for (i = 0, len = uniqueDates.length; i < len; i++) {
                    var option = document.createElement('option'),
                        d = uniqueDates[i];

                    option.value = d.sorter;
                    option.innerHTML = monthNames[d.month] + ' ' + d.year;

                    frag.appendChild(option);
                }

                
                document.getElementById('opening-date-filter').appendChild(frag);
                this.selectedDate = 0;
            },
            buildPages: function (changes, pageSize) {
                var count = 0,
                    changesPages = [];
                for (count; count < changes.length;) {
                    var page = changes.slice(count, (count + pageSize));

                    changesPages.push(page);
                    count += pageSize;
                }

                this.pages = changesPages;
                
                this.isProcessing = false;
                $('#content').find('.horizontal-tab').slice(0, 1).click();
            },
            loadData: function () {
                var now = new Date();
                var currentYear = now.getFullYear();
                var nowPlus30 = new Date();
                var self = this;
                var totalRecords = 0;

                now.setDate(1);
                nowPlus30.setDate(1);

                nowPlus30.setFullYear(currentYear + 30);
                // step into callback hell
                $.getJSON('/api/academy-openings/list/' + self.formatDate(now, '-') + '/' + self.formatDate(nowPlus30, '-') + '/0/1',
                    function (data) {
                        totalRecords = data.count;
                        self.initialRecordCount = totalRecords;
                        self.currentCount = totalRecords;
                        $.getJSON('/api/academy-openings/list/' + self.formatDate(now, '-') + '/' + self.formatDate(nowPlus30, '-') + '/0/' + totalRecords,
                            function (data) {
                                self.openingAcademies = data.items;
                                self.buildPages(data.items, self.pageSize);
                                self.buildDateDropDown();
                                raw = data.items;
                            }
                        ).fail(function(jqxhr) {
                            if (jqxhr.hasOwnProperty('responseJSON')) {
                                self.apiError = jqxhr.responseJSON;
                            } else {
                                self.loadDataError = true;
                            }
                            self.loadDataError = true;
                            self.isProcessing = false;
                        });
                    }
                ).fail(function(jqxhr) {
                    
                    if (jqxhr.hasOwnProperty('responseJSON')) {
                        self.apiError = jqxhr.responseJSON;
                    } else {
                        self.loadDataError = true;
                    }
                    self.isProcessing = false;
                });
            },
            setCurrentPage: function (pageIndex) {
                this.currentPage = pageIndex;
            },
            editEstab: function (urn) {
                this.searchUrn = urn;
                this.openingDetail();
                this.editRecord = true;
                this.searchError = false;
            },
            validUrns: function () {
                var urns = [],
                    i = 0,
                    len = this.openingAcademies.length;
                for (i; i < len; i++) {
                    urns.push(this.openingAcademies[i].urn);
                }

                return urns;
            },
            validateUrn: function () {
                var validUrns = this.validUrns();
                this.presentDetail = false;
                this.searchError = validUrns.indexOf(Number(this.searchUrn)) === -1;
                if (!this.searchError) {
                    this.openingDetail();
                    this.presentDetail = true;
                }
            },
            updateRecord: function () {
                var d = parseInt(this.updateDateDay, 10),
                    m = parseInt(this.updateDateMonth, 10),
                    y = parseInt(this.updateDateYear, 10),
                    self = this;


                this.updateNameError = this.updateName.length < 1;

                this.openDateError = (
                    isNaN(d)|| d < 1 || d > 31 || this.updateDateDay === '' ||
                    isNaN(m) || m < 1 || m > 12 || this.updateDateMonth === '' ||
                    isNaN(y) || y < 2000 || y > 2100 || this.updateDateYear === '');

                if (!this.updateNameError && !this.openDateError) {
                    var urn = this.searchUrn;
                    this.searchUrn = '';
                    this.presentDetail = false;
                    this.editRecord = false;
                    this.isProcessing = true;
                    this.userHasEdited = false;
                    this.recordUpdateErrors = {};

                    $(window).off('beforeunload');

                    $.ajax({
                        url: '/api/academy/' + urn,
                        type: 'post',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: JSON.stringify({
                            openDate: [this.updateDateYear, this.updateDateMonth , this.updateDateDay].join('-'),
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
                                self.recordUpdateErrors = jqxhr.responseJSON;
                            } else {
                                self.loadDataError = true;
                            }
                        }
                    });
                }
            },
            buildDatePages: function () {
                var dateParts = this.selectedDate.split('.'),
                    month = parseInt(dateParts[0], 10),
                    year = parseInt(dateParts[1], 10);

                if (isNaN(month) || isNaN(year)) { // user selected all
                    this.buildPages(this.openingAcademies, this.pageSize);
                    this.currentCount = this.openingAcademies.length;
                    return;
                }

                var datePages = this.openingAcademies.filter(function (opening) {
                    var oDateParts = opening.displayDate.split('/'),
                        oDateMonth = parseInt(oDateParts[1], 10) - 1,
                        oDateYear = parseInt(oDateParts[2], 10);

                    if (oDateMonth === month && oDateYear === year) {
                        return true;
                    }
                    return false;
                });

                this.buildPages(datePages, this.pageSize);
                this.currentCount = datePages.length;
                this.currentPage = 0;
            },
            openingDetail: function () {
                var urn = parseInt(this.searchUrn, 10);
                if (isNaN(urn)) {
                    return false;
                }
                var academy = this.openingAcademies.filter(function (estab) {
                    return parseInt(estab.urn, 10) === urn;
                });
                var openDate = academy[0].displayDate.split('/');

                this.updateName = academy[0].name;
                this.updateDateDay = openDate[0];
                this.updateDateMonth = openDate[1];
                this.updateDateYear = openDate[2];

                this.od = academy[0];
                return this.od;
            },
            cancelEditClick: function () {
                this.recordUpdateErrors = {};
                if (this.userHasEdited) {
                    return this.presentExitWarning = true;
                }
                this.editRecord = false;
                this.searchUrn = '';
                this.presentDetail = false;
            },
            isUserEditing: function() {
                return this.userHasEdited;

            },
            attachUnload: function() {
                this.userHasEdited = true;
                $(window).on('beforeunload', function (e) {
                        return 'Any unsaved changes will be lost';
                });
            }
        },
        
        computed: {
            paginationDescription: function () {
                var starting = this.currentPage * this.pageSize + 1,
                    ending = this.currentPage * this.pageSize + this.pageSize;

                ending = ending > this.currentCount ? this.currentCount : ending;

                return starting + " - " + ending;
            },
            page: function () {
                return this.pages[this.currentPage];
            }
        }

    });


    $(window).on('tabChange', function () {
        academyOpenings.searchError = false;
    });
    $('.horizontal-tabs-wrapper').tabs();
    function blockExits() {
         $('a').on('click', function (e) {

            if (academyOpenings.isUserEditing()) {
               e.preventDefault();
               academyOpenings.presentExitWarning = true;
            }
        });
    }
   

}());
