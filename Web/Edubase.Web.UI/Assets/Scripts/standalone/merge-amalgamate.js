var mergersApp = new Vue({
    el: '#mergers-app',
    data: {
        localAuthorities: localAuthorities,
        types: types,
        phases: phases,

        mergerType: '',
        mergerTypeConfirmed: false,
        leadEstab: '',
        linkedEstab1: '',
        linkedEstab2: '',
        linkedEstab3: '',
        mergerEstabs: [],
        validMergeUrns: false,
        amalgamationEstabs: [],

        amalgamatedEstab1: '',
        amalgamatedEstab2: '',
        amalgamatedEstab3: '',
        amalgamatedEstab4: '',
        duplicateUrnsError: false,
        mergeDateDay: '',
        mergeDateMonth: '',
        mergeDateYear: '',
        newName: '',
        typeId: '',
        laId: '',
        phaseId: '',

        mergerTypeError: false,
        mergeDateError: false,
        mergerComplete: false,

        leadEstabValid: false,
        linkedEstab1Valid: false,
        linkedEstab2Valid: false,
        linkedEstab3Valid: false,

        nameError: false,
        typeError: false,
        phaseError: false,
        laError: false,

        estabLookup: '/api/establishment/{0}',
        commitApi: '/api/amalgamate-merge',

        commitErrors: '',
        presentExitWarning: false,
        completeAmalgamation: false,
        amalgUrn: '',
        exitUrl: '',
        isProcessing: false,
        apiError: {},

        amalgamatedEstab1Empty: false,
        amalgamatedEstab1Invalid: false,
        amalgamatedEstab1UrnChecked: false,
        amalgamatedEstab1NoMatch: false,

        amalgamatedEstab2Empty: false,
        amalgamatedEstab2Invalid: false,
        amalgamatedEstab2UrnChecked: false,
        amalgamatedEstab2NoMatch: false,

        amalgamatedEstab3Empty: false,
        amalgamatedEstab3Invalid: false,
        amalgamatedEstab3UrnChecked: false,
        amalgamatedEstab3NoMatch: false,

        amalgamatedEstab4Empty: false,
        amalgamatedEstab4Invalid: false,
        amalgamatedEstab4UrnChecked: false,
        amalgamatedEstab4NoMatch: false,

        leadEstabEmpty: false,
        leadEstabInvalid: false,
        leadEstabUrnChecked: false,
        leadEstabNoMatch: false,

        linkedEstab1Empty: false,
        linkedEstab1Invalid: false,
        linkedEstab1UrnChecked: false,
        linkedEstab1NoMatch: false,

        linkedEstab2Empty: false,
        linkedEstab2Invalid: false,
        linkedEstab2UrnChecked: false,
        linkedEstab2NoMatch: false,

        linkedEstab3Empty: false,
        linkedEstab3Invalid: false,
        linkedEstab3UrnChecked: false,
        linkedEstab3NoMatch: false
    },
    created: function () {
        this.populateSelect('new-establishment-type', this.types);
        this.populateSelect('LocalAuthorityId', this.localAuthorities);
        this.blockExits();
    },
    computed: {
        displayDate: function () {
            return this.mergeDateDay + '/' + this.mergeDateMonth + '/' + this.mergeDateYear;
        },
        showGlobalError: function () {
            return (
                this.mergerTypeError ||
                    this.nameError ||
                    this.phaseError ||
                    this.typeError ||
                    this.laError ||
                    this.mergeDateError ||
                    this.duplicateUrnsError ||
                    this.commitErrors ||
                    this.leadEstabEmpty ||
                    this.leadEstabInvalid ||
                    this.leadEstabNoMatch ||
                    this.linkedEstab1Empty ||
                    this.linkedEstab1Invalid ||
                    this.linkedEstab1NoMatch ||
                    this.linkedEstab2Invalid ||
                    this.linkedEstab2NoMatch ||
                    this.linkedEstab3Invalid ||
                    this.linkedEstab3NoMatch ||
                    this.amalgamatedEstab1Empty ||
                    this.amalgamatedEstab1Invalid ||
                    this.amalgamatedEstab1NoMatch ||
                    this.amalgamatedEstab2Empty ||
                    this.amalgamatedEstab2Invalid ||
                    this.amalgamatedEstab2NoMatch ||
                    this.amalgamatedEstab3Invalid ||
                    this.amalgamatedEstab3NoMatch ||
                    this.amalgamatedEstab4Invalid ||
                    this.amalgamatedEstab4NoMatch
            );
        },
        schoolDetailUrl: function () {
            return "/Establishments/Establishment/Details/" + this.leadEstab;
        },
        amalgUrl: function () {
            return '/Establishments/Establishment/Details/' + this.amalgUrn;
        },
        leadEstablishmentName: function () {
            var self = this;
            if (self.validMergeUrns && self.mergerType === 'merger') {
                var leadName = self.mergerEstabs.filter(function (estab) {
                    return estab.urn === Number(self.leadEstab);
                })[0].name;

                return leadName;
            }
            return '';
        },
        selectedEstablishmentType: function () {
            var self = this;
            if (self.typeId !== '') {
                var typeName = types.filter(function(t) {
                    return t.id == self.typeId;
                })[0].name;

                return typeName;
            }
        },
        addedUrns: function() {
            if (this.mergerType === 'merger') {
                return [this.leadEstab, this.linkedEstab1, this.linkedEstab2, this.linkedEstab3];
            }
            return [this.amalgamatedEstab1, this.amalgamatedEstab2, this.amalgamatedEstab3, this.amalgamatedEstab4];
        },
        leadEstablishmentType: function() {
          var self = this;
          if (self.validMergeUrns && self.mergerType === 'merger') {
            var leadType = self.mergerEstabs.filter(function (estab) {
              return estab.urn === Number(self.leadEstab);
            })[0].typeName;

            return leadType;
          }
          return '';
        }
    },
    methods: {
        hasDuplicateUrn: function() {
            var arr = this.addedUrns.sort();
            for (var i = 0, len = arr.length; i < len; i++) {
                if (arr[i] !== '' && arr[i + 1] === arr[i]) {
                    return true;
                }
            }
            return false;
        },
        populateSelect: function (control, data) {
            var frag = document.createDocumentFragment();

            document.getElementById(control).options.length = 0;

            $.each(data, function (n, item) {
                var option = document.createElement('option');
                option.value = item.id;
                option.innerHTML = item.name;

                frag.appendChild(option);
            });
            document.getElementById(control).appendChild(frag);
        },
        updatePhases: function () {
            var tp = type2PhaseMap;
            var self = this;
            var validOptions = [];

            var validPhaseIds =[];
            for (var k in tp) {
                if (k === self.typeId) {
                    validPhaseIds = tp[k];
                }
            }

            for (var i = 0, len = validPhaseIds.length; i < len; i++) {
                var obj = phases.filter(function(phase) {
                    return phase.id === validPhaseIds[i];
                })[0];
                validOptions.push(obj);
            }

            this.populateSelect('new-establishment-phase', validOptions);
        },
        checkMergeType: function () {
            this.mergerTypeError = this.mergerType === '';
            if (!this.mergerTypeError) {
                this.mergerTypeConfirmed = true;
                this.clearErrors();
            } else {
                this.errorFocus();
            }
        },
        checkIfInvalid: function(component){
            return this[component].length < 5 || isNaN(this[component])
        },
        validateField: function(component){
            if (this[component] == '') {
                this[component + 'Empty'] = true;
            } else if (this.checkIfInvalid(component)) {
                this[component + 'Invalid'] = true;
            } else {
                this.urnCheck(this[component], component);
            }
        },
        clearMergeFields: function(){
            this.leadEstabEmpty = false;
            this.leadEstabInvalid = false;
            this.leadEstabUrnChecked = false;
            this.leadEstabValid = false;
            this.leadEstabNoMatch = false;

            this.linkedEstab1Empty = false;
            this.linkedEstab1Invalid = false;
            this.linkedEstab1bUrnChecked = false;
            this.linkedEstab1Valid = false;
            this.linkedEstab1NoMatch = false;

            this.linkedEstab2Empty = false;
            this.linkedEstab2Invalid = false;
            this.linkedEstab2bUrnChecked = false;
            this.linkedEstab2Valid = false;
            this.linkedEstab2NoMatch = false;

            this.linkedEstab3Empty = false;
            this.linkedEstab3Invalid = false;
            this.linkedEstab3bUrnChecked = false;
            this.linkedEstab3Valid = false;
            this.linkedEstab3NoMatch = false;
        },
        validateMergeFields: function(){
            var self = this;
            this.clearErrors();
            this.clearMergeFields();
            this.mergerEstabs = [];

            this.duplicateUrnsError = this.hasDuplicateUrn();
            //this.errorFocus();
            if (this.duplicateUrnsError) {
                if (this.leadEstab == '') { this.leadEstabEmpty = true; }
                if (this.linkedEstab1 == '') { this.linkedEstab1Empty = true; }
                if (this.linkedEstab2 == '') { this.linkedEstab2Empty = true; }
                if (this.linkedEstab3 == '') { this.linkedEstab3Empty = true; }
                return true;
            }

            this.validateField('leadEstab');
            this.validateField('linkedEstab1');
            this.validateField('linkedEstab2');
            this.validateField('linkedEstab3');

            var bothChecked;
            bothChecked = window.setInterval(function () {
                if (self.leadEstabUrnChecked && self.linkedEstab1UrnChecked) {
                    if (self.leadEstabValid && self.linkedEstab1Valid && !self.showGlobalError) {
                        self.clearErrors();
                        self.validMergeUrns = true;
                    }
                    window.clearInterval(bothChecked);
                    }
                },
                100);

            this.errorFocus();
        },
        validateMergerDate: function () {
            var day = parseInt(this.mergeDateDay, 10),
                month = parseInt(this.mergeDateMonth -1, 10),
                year = parseInt(this.mergeDateYear, 10),

                dateError = false,
                months31 = [0, 2, 4, 6, 7, 9, 11],
                currentYear = new Date().getFullYear();

            if (!day || !month || !year || isNaN(day) || isNaN(month) || isNaN(year)) {
                dateError = true;
            }

            var isLeap = new Date(year, 1, 29).getMonth() === 1; // will return march for non leap years

            if (isLeap && month === 1) {
                if (day > 29) {
                    dateError = true;
                }
            } else if (month === 1) {
                if (day > 28) {
                    dateError = true;
                }
            }

            if (months31.indexOf(month - 1)) {
                if (day < 1 || day > 31) {
                    dateError = true;
                }
            } else {
                if (day < 1 || day > 30) {
                    dateError = true;
                }
            }

            if (month < 1 || month > 12) {
                dateError = true;
            }

            return dateError;
        },
        clearAmalgamationFields: function(){
            this.amalgamatedEstab1Empty = false;
            this.amalgamatedEstab1Invalid = false;
            this.amalgamatedEstab1UrnChecked = false;
            this.amalgamatedEstab1Valid = false;
            this.amalgamatedEstab1NoMatch = false;

            this.amalgamatedEstab2Empty = false;
            this.amalgamatedEstab2Invalid = false;
            this.amalgamatedEstab2bUrnChecked = false;
            this.amalgamatedEstab2Valid = false;
            this.amalgamatedEstab2NoMatch = false;

            this.amalgamatedEstab3Empty = false;
            this.amalgamatedEstab3Invalid = false;
            this.amalgamatedEstab3bUrnChecked = false;
            this.amalgamatedEstab3Valid = false;
            this.amalgamatedEstab3NoMatch = false;

            this.amalgamatedEstab4Empty = false;
            this.amalgamatedEstab4Invalid = false;
            this.amalgamatedEstab4bUrnChecked = false;
            this.amalgamatedEstab4Valid = false;
            this.amalgamatedEstab4NoMatch = false;
        },
        validateAmalgamationFields: function(){
            var self = this;
            this.clearErrors();
            this.clearAmalgamationFields();
            this.amalgamationEstabs = [];

            this.duplicateUrnsError = this.hasDuplicateUrn();
            //this.errorFocus();
            if (this.duplicateUrnsError) {
                if (this.amalgamatedEstab1 == '') { this.amalgamatedEstab1Empty = true; }
                if (this.amalgamatedEstab2 == '') { this.amalgamatedEstab2Empty = true; }
                if (this.amalgamatedEstab3 == '') { this.amalgamatedEstab3Empty = true; }
                if (this.amalgamatedEstab4 == '') { this.amalgamatedEstab4Empty = true; }
                return true;
            }

            this.validateField('amalgamatedEstab1');
            this.validateField('amalgamatedEstab2');
            this.validateField('amalgamatedEstab3');
            this.validateField('amalgamatedEstab4');

            var bothChecked;
            bothChecked = window.setInterval(function () {
                if (self.amalgamatedEstab1UrnChecked && self.amalgamatedEstab2UrnChecked) {
                    if (self.amalgamatedEstab1Valid && self.amalgamatedEstab2Valid && !self.showGlobalError) {
                        self.validMergeUrns = true;
                        self.clearErrors();
                    }
                    window.clearInterval(bothChecked);
                    }
                },
                100);

            this.errorFocus();
        },
        urnCheck: function(urn, component){
            var self = this;
            $.ajax({
                url: self.estabLookup.replace('{0}', urn),
                dataType: 'json',
                method: 'get',
                async: false,
                success: function (data) {
                    self[component + 'Valid'] = !data.notFound;
                    if (self[component + 'Valid']) {
                        if (self.mergerType === 'merger') {
                            self.mergerEstabs.push(data.returnValue);
                        } else {
                            self.amalgamationEstabs.push(data.returnValue);
                        }
                    }
                    self[component + 'UrnChecked'] = true;
                },
                error: function (jqxhr) {
                    if (jqxhr.hasOwnProperty('responseJSON')) {
                        self.apiError = jqxhr.responseJSON;
                    }
                    self[component + 'Valid'] = false;
                    self[component + 'NoMatch'] = true;
                    self[component + 'UrnChecked'] = true;
                },
                complete: function () {}
            });
        },
        processMerger: function () {
            var self = this;
            var postData = {};

            postData.operationType = 'merge';
            postData.MergeOrAmalgamationDate = [this.mergeDateYear, this.mergeDateMonth, this.mergeDateDay].join('-');
            postData.LeadEstablishmentUrn = this.leadEstab;
            postData.UrnsToMerge = this.mergerEstabs.map(function (estab) {
                return estab.urn;
            });

            postData.UrnsToMerge = postData.UrnsToMerge.filter(function(item) {
                return item != self.leadEstab;
            });

            this.mergeDateError = this.validateMergerDate();
            this.errorFocus();

            if (!this.mergeDateError) {
                this.validLinkDate = true;
                this.isProcessing = true;
                $.ajax({
                    url: self.commitApi,
                    method: 'post',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(postData),
                    success: function (data) {
                        if (data.hasOwnProperty('successful') && data.successful) {
                            self.mergerComplete = true;
                            self.isProcessing = false;
                            self.clearErrors();
                        }
                    },
                    error: function (jqXHR) {
                        var errObj = JSON.parse(jqXHR.responseText);
                        var errMessage = '';

                        if (jqXHR.hasOwnProperty('responseJSON')) {
                            self.apiError = jqXHR.responseJSON;
                        }

                        for (var item in errObj) {
                            if (item === 'validationEnvelope') {
                                var env = errObj[item][0].errors;
                                env.forEach(function (er) {
                                    errMessage += er.message + '<br>';
                                });
                            } else if (!errObj.validationEnvelope && item === 'errors') {
                                errMessage = errObj.errors[0].message;
                                self.validMergeUrns = false;
                            }
                        }
                        self.commitErrors = errMessage;
                        self.isProcessing = false;
                        self.errorFocus();
                    }
                });
            }
        },
        processAmalgamation: function () {
            var self = this;
            var postData = {};

            this.nameError = (this.newName.length < 1);
            this.typeError = (this.typeId === '');
            this.laError = (this.laId === '');
            this.phaseError = (this.phaseId === '');
            this.mergeDateError = this.validateMergerDate();

            this.errorFocus();

            if (!this.nameError &&
                !this.typeError &&
                !this.laError &&
                !this.phaseError &&
                !this.mergeDateError &&
                !this.duplicateUrnsError) {

                this.isProcessing = true;
                postData.operationType = 'amalgamate';
                postData.MergeOrAmalgamationDate = [this.mergeDateYear, this.mergeDateMonth, this.mergeDateDay].join('-');
                postData.UrnsToMerge = this.amalgamationEstabs.map(function (estab) {
                    return estab.urn;
                });
                postData.NewEstablishmentName = this.newName;
                postData.NewEstablishmentPhaseId = this.phaseId;
                postData.NewEstablishmentTypeId = this.typeId;
                postData.NewEstablishmentLocalAuthorityId = this.laId;


                $.ajax({
                    url: self.commitApi,
                    method: 'post',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(postData),
                    success: function (data) {
                        if (data.hasOwnProperty('successful') && data.successful) {
                            self.completeAmalgamation = true;
                            self.amalgUrn = data.response.amalgamateNewEstablishmentUrn;
                            self.isProcessing = false;
                            self.clearErrors();
                        }
                    },
                    error: function (jqXHR) {
                        var errObj = JSON.parse(jqXHR.responseText);
                        var errMessage = '';

                        if (jqXHR.hasOwnProperty('responseJSON')) {
                            self.apiError = jqXHR.responseJSON;
                        }

                        for (var item in errObj) {
                            if (item === 'validationEnvelope' && errObj.validationEnvelope) {
                                var env = errObj[item][0].errors;
                                env.forEach(function (er) {
                                    //console.log(errObj[item][0].errors);
                                    //console.log(er);
                                    errMessage += er.message + '<br>';
                                });
                            } else if (!errObj.validationEnvelope && item === 'errors') {
                                errMessage = errObj.errors[0].message;
                                self.validMergeUrns = false;
                            }
                        }
                        self.commitErrors = errMessage;
                        self.isProcessing = false;
                        self.errorFocus();
                    }
                });
            }
        },
        errorFocus: function(){
            if ($('.error-summary').length) {
                window.document.title = "Error: Amalgamations and mergers tool - GOV.UK";
                $('.error-summary').focus();
            } else {
                window.setTimeout(function(){
                    if ($('.error-summary').length) {
                        window.document.title = "Error: Amalgamations and mergers tool - GOV.UK";
                        $('.error-summary').focus();
                    }
                },500);
            }
        },
        clearErrors: function(){
            window.document.title = "Amalgamations and mergers tool - GOV.UK";
            this.apiError = {};
            this.mergerTypeError = false;
            this.nameError = false;
            this.phaseError = false;
            this.typeError = false;
            this.laError = false;
            this.mergeDateError = false;
            this.duplicateUrnsError = false;
            this.commitErrors = false;
            this.leadEstabEmpty = false;
            this.leadEstabInvalid = false;
            this.leadEstabNoMatch = false;
            this.linkedEstab1Empty = false;
            this.linkedEstab1Invalid = false;
            this.linkedEstab1NoMatch = false;
            this.linkedEstab2Invalid = false;
            this.linkedEstab2NoMatch = false;
            this.linkedEstab3Invalid = false;
            this.linkedEstab3NoMatch = false;
            this.amalgamatedEstab1Empty = false;
            this.amalgamatedEstab1Invalid = false;
            this.amalgamatedEstab1NoMatch = false;
            this.amalgamatedEstab2Empty = false;
            this.amalgamatedEstab2Invalid = false;
            this.amalgamatedEstab2NoMatch = false;
            this.amalgamatedEstab3Invalid = false;
            this.amalgamatedEstab3NoMatch = false;
            this.amalgamatedEstab4Invalid = false;
            this.amalgamatedEstab4NoMatch = false;
        },
        exitConfirmed: function() {
            window.location = this.exitUrl;
        },
        blockExits: function () {
            var self = this;
            $('a, [value="cancel"]').on('click', function (e) {
                self.exitUrl = $(this).attr('href');
                if ((self.mergerType === 'amalgamation' && !self.completeAmalgamation) ||
                    (self.mergerType == 'merger' && !self.mergerComplete)) {
                    e.preventDefault();
                    self.presentExitWarning = true;
                    // Focus on main CTA when modal available
                    $(document).ready(function() {
                        if ($('#modal-content').length > 0) {
                            $('#button-ok').focus();
                        }
                  });
                }
            });
        },
        restart: function () {
            if (!this.mergerTypeConfirmed) {
                window.location = '/Tools';
                return;
            }
            window.location = '/Tools/MergersTool';
        }
    }

});
