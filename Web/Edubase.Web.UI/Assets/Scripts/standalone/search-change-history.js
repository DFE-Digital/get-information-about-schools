var changesPages = [];


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



Vue.component('changes-table',
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
        pages: Array
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


var SearchChangeHistory = new Vue({
    el: '#change-history-app',
    data: {
        errorFromDate: false,
        errorToDate: false,
        filterTypeSelectedError: false,
        ajaxError: false,
        changeHistory: [],
        resultsUrl: '/public/assets/scripts/JSON/establishmentChanges.js',
        filterView: true,
        fakeWait: false,
        resultsView: false,


        // shared
        searchType: '',
        dateType: 'applied',
        fromDateDay: '',
        fromDateMonth: '',
        fromDateYear: '',
        toDateDay: '',
        toDateMonth: '',
        toDateYear: '',
        
        fromDate: null,
        toDate: null,
        changeSuggestedBy: 0,
        changeApprovedBy: 0,

        establishmentTypes: [
            'academies',
            'children-centres',
            'free-schools',
            'higher-education',
            'independent',
            'la-maintained',
            'non-maintained',
            'other'],
        establishmentTypesCleared: false,

        establishmentFields: [
            'name',
            'locality',
            'city',
            'postcode',
            'additional-address',
            'head-title',
            'head-name',
            'statutory-low-age',
            'statutory-high-age',
            'education-phase',
            'type',
            'further-education-type',
            'gender',
            'la',
            'establishment-number',
            'ukprn',
            'status',
            'admissions-policy',
            'website',
            'phone',
            'inspectorate',
            'proprieter-name',
            'religious-character',
            'diocese',
            'religious-ethos',
            'provision-boarding',
            'provision-nursery',
            'provision-6thform',
            'capacity',
            'section41',
            'opendate',
            'reason-opened',
            'closedate',
            'reason-closed',
            'special-classes',
            'sen1',
            'sen2',
            'sen3',
            'sen4',
            'type-provision',
            'provision-roll',
            'sen-roll',
            'sen-unit-capacity',
            'bso-inspectorate',
            'bso-last-inspectorate-visit',
            'bso-next-inspectorate-visit',
            'rsc-region',
            'gov-region',
            'admin-district',
            'admin-ward',
            'constituency',
            'urban-rural',
            'gssla',
            'easting',
            'northing',
            'casward',
            'msoa',
            'lsoa'
        ],
        establishmentFieldsCleared: false,
        groupTypes: [
            'academies',
            'academy',
            'sponsor',
            'trust',
            'federation',
            'childrens-group',
            'childrens-collaboration'
        ],
        groupTypesCleared: false,

        //download view
        downloadType: 'csv',
        currentPage: 0,
        pages: [],
        currentCount: 0,
        pageSize: 100,
        preparingDownload: false,
        availableDownload: false

    },
    computed: {
        datesFromInput: function() {
            if (this.fromDateDay !== '' && this.fromDateMonth !== '' && this.fromDateYear !== '') {
                this.fromDate = new Date(this.fromDateYear, this.fromDateMonth - 1, this.fromDateDay).toISOString();
            }
            if (this.toDateDay !== '' && this.toDateMonth !== '' && this.toDateYear !== '') {
                this.toDate = new Date(this.toDateYear, this.toDateMonth - 1, this.toDateDay).toISOString();
            }
        },
        pageCount: function () {
            return this.currentPage * this.pageSize;
        },
        paginationDescription: function () {
            var starting = this.currentPage * this.pageSize + 1,
                ending = this.currentPage * this.pageSize + this.pageSize;

            ending = ending > this.currentCount ? this.currentCount : ending;

            return starting + " - " + ending;
        },
        globalError: function() {
            return (this.ajaxError || this.filterTypeSelectedError || this.errorFromDate || this.errorToDate);
        },
        downloadLinkText: function() {
            return 'Establishment change history in '+ this.downloadType.toUpperCase() +' format'
        }
    },
    methods: {
        getResults: function () {
            this.filterTypeSelectedError = this.searchType === '';

            if (this.filterTypeSelectedError){
                return false;
            }
            var url = this.resultsUrl;
            var self = this;
            self.fakeWait = true;
            self.filterView = false;
            $.getScript(url)
                .done(function(data) {
                    self.changeHistory = JSON.parse(data);
                    self.pages = buildPages(self.changeHistory, self.pageSize);
                    self.currentCount = self.changeHistory.length;
                    self.ajaxError = false;
                    window.setTimeout(function() {
                        self.resultsView = true;
                        self.fakeWait = false;
                    }, 1500);
                    

                })
                .fail(function() {
                    self.ajaxError = true;
                    self.fakeWait = false;
                });
        },
        prepareDownload: function () {
            var self = this;
            this.fakeWait = true;
            this.preparingDownload = true;
            this.resultsView = false;

            window.setTimeout(function(){
                self.fakeWait = false;
                self.preparingDownload = false;

                self.availableDownload = true;
            }, 3000);
        },
        setCurrentPage: function (pageIndex) {
            this.currentPage = pageIndex;
        },
        resetForm: function(){
            this.groupTypes = [];
            this.establishmentFields = [];
            this.establishmentTypes = [];
        },
        selectSingle: function (panel, item) {
            if (!this[panel + 'Cleared']) {
                this[panel] = [];
                this[panel].push(item);
                this[panel + 'Cleared'] = true;
            }
            
        }
    }

});

/// accordions
(function() {
    var panels = document.querySelectorAll('.expander-panel');
    
    Array.prototype.forEach.call(panels, function(el) {
        var trigger = el.querySelectorAll('.panel-trigger')[0];

        trigger.addEventListener('click', function (e) {
            e.preventDefault();
            el.classList.toggle('open-expander');
        });
    });
}());