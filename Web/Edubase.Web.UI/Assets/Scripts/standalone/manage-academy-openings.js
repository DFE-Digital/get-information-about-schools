var academyOpeningsJson,
            uniqueDates = [],
            academyOpenings,
            i,
            len,
            monthNames = [
                'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October',
                'November', 'December'
            ],
            getJson = $.getScript('/public/assets/scripts/JSON/academyOpenings.stub.js',
                function (data) {
                    academyOpeningsJson = JSON.parse(data);
                    return academyOpeningsJson;
                }
            );


function buildPages(changes, pageSize) {
    var count = 0,
        changesPages = [];
    for (count; count < changes.length;) {
        var page = changes.slice(count, (count + pageSize));

        changesPages.push(page);
        count += pageSize;
    }

    return changesPages;
}

Vue.component('openings-table',
   {
       template: '#table-template',
       props: {
           currentPage: {
               type: Number,
               default: 0
           },
           maxPageSize: {
               type: Number,
               default: 10
           },
           pages: Array,
           editEstab: Function
       },

       computed: {
           page: function () {
               return this.pages[this.currentPage];
           }
       },
       methods: {
           detailUrl: function (urn) {
               return '/Establishment/Details/' + urn;
           }
       }
   });



$.when(getJson)
    .done(function () {

        function sortOpenings() {
            academyOpeningsJson.sort(function (a, b) {
                var aDate = new Date(a.openingDate),
                    bDate = new Date(b.openingDate),
                    sortOrder = aDate - bDate;

                if (sortOrder === 0) {
                    sortOrder = a.name > b.name ? 1 : -1;
                }
                return sortOrder;

            });
        }

        sortOpenings();

        var openingDates = [];

        for (i = 0, len = academyOpeningsJson.length; i < len; i++) {
            var openingDate = new Date(academyOpeningsJson[i].openingDate);
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



        for (i = 0, len = uniqueDates.length; i < len; i++) {
            var option = document.createElement('option'),
                d = uniqueDates[i];

            option.value = d.sorter;
            option.innerHTML = monthNames[d.month] + ' ' + d.year;

            frag.appendChild(option);
        }

        document.getElementById('opening-date-filter').appendChild(frag);

        academyOpenings = new Vue({
            el: '#academy-opening-app',
            data: {
                openingAcademies: academyOpeningsJson,
                selectedDate: '0',
                initialRecordCount: academyOpeningsJson.length,
                currentPage: 0,
                pageSize: 50,
                pages: buildPages(academyOpeningsJson, 50),
                currentCount: academyOpeningsJson.length,
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
                presentDetail: false

            },
            methods: {
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
                        urns.push(this.openingAcademies[i].Urn);
                    }

                    return urns;
                },
                validateUrn: function () {
                    var validUrns = this.validUrns();
                    this.searchError = validUrns.indexOf(this.searchUrn) === -1;
                    if (!this.searchError) {
                        // this.editEstab(this.searchUrn);
                        this.openingDetail();
                        this.presentDetail = true;
                    }
                },
                updateRecord: function () {
                    var d = parseInt(this.updateDateDay, 10),
                        m = parseInt(this.updateDateMonth, 10),
                        y = parseInt(this.updateDateYear, 10);

                    this.updateNameError = this.updateName.length < 1;

                    this.openDateError = (
                        d < 1 || d > 31 || this.updateDateDay === '' ||
                        m < 1 || m > 12 || this.updateDateMonth === '' ||
                        y < 2000 || y > 2100 || this.updateDateYear === '');

                    if (!this.updateNameError && !this.openDateError) {
                        var urn = this.searchUrn;
                        var record = this.openingAcademies.filter(function (estab) {
                            return parseInt(estab.Urn, 10) === parseInt(urn, 10);
                        });

                        record[0].name = this.updateName;
                        record[0].displayDate = [this.updateDateDay, this.updateDateMonth, this.updateDateYear].join('/');
                        record[0].openingDate = new Date(this.updateDateYear, this.updateDateMonth - 1, this.updateDateDay).toISOString();

                        this.searchUrn = '';
                        this.presentDetail = false;
                        this.editRecord = false;

                        sortOpenings();
                        var self = this;
                        window.setTimeout(function () {
                            self.openingAcademies = academyOpeningsJson;
                            self.pages = buildPages(self.openingAcademies, self.pageSize);
                            self.selectedDate = 0;
                            
                        }, 500);

                    }
                },
                buildDatePages: function () {
                    var dateParts = this.selectedDate.split('.'),
                        month = parseInt(dateParts[0], 10),
                        year = parseInt(dateParts[1], 10);

                    if (isNaN(month) || isNaN(year)) { // user selected all
                        this.pages = buildPages(this.openingAcademies, this.pageSize);
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

                    this.pages = buildPages(datePages, this.pageSize);
                    this.currentCount = datePages.length;
                    this.currentPage = 0;
                },
                openingDetail: function () {
                    var urn = parseInt(this.searchUrn, 10);
                    if (isNaN(urn)) {
                        return false;
                    }
                    var academy = this.openingAcademies.filter(function (estab) {
                        return parseInt(estab.Urn, 10) === urn;
                    });
                    var openDate = academy[0].displayDate.split('/');

                    this.updateName = academy[0].name;
                    this.updateDateDay = openDate[0];
                    this.updateDateMonth = openDate[1];
                    this.updateDateYear = openDate[2];

                    this.od = academy[0];
                    return this.od;
                }
            },
            computed: {
                paginationDescription: function () {
                    var starting = this.currentPage * this.pageSize + 1,
                        ending = this.currentPage * this.pageSize + this.pageSize;

                    ending = ending > this.currentCount ? this.currentCount : ending;

                    return starting + " - " + ending;
                }
            }

        });
        $('.horizontal-tabs-wrapper').tabs();
        $(window).on('tabChange', function () {
            academyOpenings.searchError = false;
        });

    });

