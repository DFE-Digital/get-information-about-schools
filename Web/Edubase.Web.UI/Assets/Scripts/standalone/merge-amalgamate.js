var mergersApp = new Vue({
    el: '#mergers-app',
    data: {
        localAuthorities: localAuthorities,
        types: types,
        phases: phases,
        typeMap: type2PhaseMap,

        mergerType: '',
        mergerTypeConfirmed: false,
        leadEstab: '',
        linkedEstab0: '',
        linkedEstab1: '',
        linkedEstab2: '',
        mergerEstabs: [],
        validMergeUrns: false,
        amalgamationEstabs: [],

        amalgamatedEstab0: '',
        amalgamatedEstab1: '',
        amalgamatedEstab2: '',
        amalgamatedEstab3: '',
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
        mergeLengthError: false,
        mergerComplete: false,

        leadEstabError: false,
        leadEstabLengthError: false,
        leadEstabUrnCheckDone: false,
        leadEstabValid: false,
        leadEstabErrorMessage: '',

        linkedEstab0Error: false,
        linkedEstab1Error: false,
        linkedEstab2Error: false,
        linkedEstabError: false,
        linkedEstabUrnCheckDone: false,

        linkedEstab0Valid: false,
        linkedEstab1Valid: false,
        linkedEstab2Valid: false,

        amalgamatedEstab0Error: false,
        amalgamatedEstab1Error: false,
        amalgamatedEstab2Error: false,
        amalgamatedEstab3Error: false,
        amalgamateUrnError: false,
        amalgamationLengthError: false,

        amalgamatedEstab0Valid: false,
        amalgamatedEstab1Valid: false,
        amalgamatedEstab2Valid: false,
        amalgamatedEstab3Valid: false,

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

        leadEstabEmpty: false,
        leadEstabInvalid: false,
        leadEstabUrnChecked: false,
        leadEstabNoMatch: false,

        linkedEstab0Empty: false,
        linkedEstab0Invalid: false,
        linkedEstab0UrnChecked: false,
        linkedEstab0NoMatch: false
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
                    this.amalgamateUrnError ||
                    this.leadEstabError ||
                    this.linkedEstabError ||
                    this.amalgamationLengthError ||
                    this.nameError ||
                    this.phaseError ||
                    this.typeError ||
                    this.laError ||
                    this.mergeDateError ||
                    this.mergeLengthError ||
                    this.duplicateUrnsError ||
                    this.commitErrors ||
                    this.leadEstabEmpty ||
                    this.leadEstabInvalid ||
                    this.leadEstabNoMatch ||
                    this.linkedEstab0Empty ||
                    this.linkedEstab0Invalid ||
                    this.linkedEstab0NoMatch
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
                return [this.leadEstab, this.linkedEstab0, this.linkedEstab1, this.linkedEstab2];
            }
            return [this.amalgamatedEstab0, this.amalgamatedEstab1, this.amalgamatedEstab2, this.amalgamatedEstab3];
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
        disallowMerge: function () {
            return this.leadEstab.length < 5 || isNaN(this.leadEstab)
                || (($.trim(this.linkedEstab0).length < 5 || isNaN(this.linkedEstab0))
                    && ($.trim(this.linkedEstab1).length < 5 || isNaN(this.linkedEstab1))
                    && ($.trim(this.linkedEstab2).length < 5 || isNaN(this.linkedEstab2)));
        },
        disallowAmalgamate: function () {

            var urns = [];
            if (($.trim(this.amalgamatedEstab0).length >= 5 && !isNaN(this.amalgamatedEstab0))) urns.push(this.amalgamatedEstab0);
            if (($.trim(this.amalgamatedEstab1).length >= 5 && !isNaN(this.amalgamatedEstab1))) urns.push(this.amalgamatedEstab1);
            if (($.trim(this.amalgamatedEstab2).length >= 5 && !isNaN(this.amalgamatedEstab2))) urns.push(this.amalgamatedEstab2);
            if (($.trim(this.amalgamatedEstab3).length >= 5 && !isNaN(this.amalgamatedEstab3))) urns.push(this.amalgamatedEstab3);

            return urns.length < 2;
        },
        clearMergeFields: function(){
            this.leadEstabEmpty = false;
            this.leadEstabInvalid = false;
            this.leadEstabUrnChecked = false;
            this.leadEstabValid = false;
            this.leadEstabNoMatch = false;
            this.linkedEstab0Empty = false;
            this.linkedEstab0Invalid = false;
            this.linkedEstab0bUrnChecked = false;
            this.linkedEstab0Valid = false;
            this.linkedEstab0NoMatch = false;
            console.log('all fields cleared');
        },
        validateMergeFields: function(){
            var self = this;
            this.clearMergeFields();

            if (this.leadEstab == '') {
                this.leadEstabEmpty = true;
            } else if (this.leadEstab.length < 5 || isNaN(this.leadEstab)) {
                this.leadEstabInvalid = true;
            } else {
                this.urnCheck(this.leadEstab, 'leadEstab');
                var loopUntilChecked0;
                loopUntilChecked0 = window.setInterval(function () {
                    if (self.leadEstabUrnChecked) {
                        if (!self.leadEstabValid) {
                            self.leadEstabNoMatch = true;
                        }
                        window.clearInterval(loopUntilChecked0);
                        }
                    },
                    100);
            }

            if (this.linkedEstab0 == '') {
                this.linkedEstab0Empty = true;
            } else if (this.linkedEstab0.length < 5 || isNaN(this.linkedEstab0)) {
                this.linkedEstab0Invalid = true;
            } else {
                console.log('linkedEstab0UrnChecked: ' + self.linkedEstab0UrnChecked);
                this.urnCheck(this.linkedEstab0, 'linkedEstab0');
                var loopUntilChecked1;
                loopUntilChecked1 = window.setInterval(function () {
                    if (self.linkedEstab0UrnChecked) {
                        console.log('linkedEstab0UrnChecked: ' + self.linkedEstab0UrnChecked);
                        console.log('linkedEstab0Valid: ' + self.linkedEstab0Valid);
                        console.log('linkedEstab0NoMatch: ' + self.linkedEstab0NoMatch);
                        if (!self.linkedEstab0Valid) {
                            self.linkedEstab0NoMatch = true;
                        }
                        window.clearInterval(loopUntilChecked1);
                        }
                    },
                    100);
            }
        },
        validateMergeSelection: function () {
            this.fieldCount = 0;
            var self = this;
            this.mergerEstabs = [];

            var presentValidation = function(){
                var urnChecksDone;
                urnChecksDone = window.setInterval(function () {
                    if (self.leadEstabUrnCheckDone && self.linkedEstabUrnCheckDone) {
                        console.log('checks done - leadEstabValid: ' + self.leadEstabValid);
                        console.log('checks done - linkedEstab0Valid: ' + self.linkedEstab0Valid);
                        console.log('checks done - linkedEstab1Valid: ' + self.linkedEstab1Valid);
                        console.log('checks done - linkedEstab2Valid: ' + self.linkedEstab2Valid);
                        if (!self.leadEstabValid) {
                            console.log('present validation: leadEstab is not valid');
                            self.leadEstabError = true;
                            self.errorFocus();
                        }
                        if (!self.linkedEstab0Valid || !self.linkedEstab1Valid || !self.linkedEstab2Valid) {
                            console.log('present validation - some additional est invalid');
                            self.linkedEstabError = true;
                            self.errorFocus();
                        }
                        if (self.linkedEstab0Valid && self.linkedEstab1Valid && self.linkedEstab2Valid && self.leadEstabValid) {
                            console.log('present validation - all additional est valid :)');
                            self.validMergeUrns = true;
                            self.clearErrors();
                        }
                        self.isProcessing = false;
                        window.clearInterval(urnChecksDone);
                        }
                    },
                    100);

            }

            this.linkedEstabError = false;
            this.leadEstabLengthError = false;
            this.leadEstabError = false;
            this.linkedEstab0Valid = true;
            this.linkedEstab1Valid = true;
            this.linkedEstab2Valid = true;
            this.mergeLengthError = false;
            this.commitErrors = '';
            this.duplicateUrnsError = this.hasDuplicateUrn();
            this.errorFocus();
            if (this.duplicateUrnsError) {
                return true;
            }

            var promise = [];

            if (this.leadEstab !== '') {
                //console.log('leadEstab not empty');
                this.leadEstabValid = false;
                this.leadEstabUrnCheckDone = false;
                this.validateUrn(this.leadEstab, 'leadEstab');
                var leadEstCheck;
                leadEstCheck = window.setInterval(function () {
                    if (self.leadEstabUrnCheckDone) {
                        presentValidation();
                        window.clearInterval(leadEstCheck);
                        }
                    },
                    100);
            } else {
                //console.log('leadEstab empty');
                //this.leadEstabError = false;
                this.leadEstabLengthError = true;
            }

            if (this.linkedEstab0 !== '') {
                console.log('linkedEstab0 is not empty');
                this.linkedEstab0Valid = false;
                this.linkedEstabUrnCheckDone = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.linkedEstab0, 'linkedEstab0'));
            }

            if (this.linkedEstab1 !== '') {
                this.linkedEstab1Valid = false;
                this.linkedEstabUrnCheckDone = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.linkedEstab1, 'linkedEstab1'));
            }

            if (this.linkedEstab2 !== '') {
                this.linkedEstab2Valid = false;
                this.linkedEstabUrnCheckDone = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.linkedEstab2, 'linkedEstab2'));
            }

            if (promise.length > 0) {
                this.isProcessing = true;
                $.when(promise.join(',')).done(
                    function () {
                        var tt;
                        if (self.fieldCount > 0) {
                            tt = window.setInterval(function () {
                                    if (self.fieldCount === 0) {
                                        console.log('inside promise: linkedEstab0:' + self.linkedEstab0Valid);
                                        presentValidation();
                                        window.clearInterval(tt);
                                    }
                                },
                                100);
                        } else {
                            console.log('inside promise: linkedEstab0:' + self.linkedEstab0Valid);
                            presentValidation();
                        }
                    });
            } else {
                self.mergeLengthError = true;
                self.linkedEstabUrnCheckDone = true;
                self.errorFocus();
            }
        },
        urnCheck: function(urn, component){
            console.log('Doing the URN check');
            var self = this;

            $.ajax({
                url: self.estabLookup.replace('{0}', urn),
                dataType: 'json',
                method: 'get',
                success: function (data) {
                    console.log('success');
                    self[component + 'Valid'] = !data.notFound;
                    if (self[component + 'Valid']) {
                        if (self.mergerType === 'merger') {
                            self.mergerEstabs.push(data.returnValue);
                        } else {
                            self.amalgamationEstabs.push(data.returnValue);
                        }
                    }
                    //self[component + 'UrnChecked'] = true;
                },
                error: function (jqxhr) {
                    console.log('error');
                    if (jqxhr.hasOwnProperty('responseJSON')) {
                        self.apiError = jqxhr.responseJSON;
                    }
                    self[component + 'Valid'] = false;
                    //self[component + 'UrnChecked'] = true;
                },
                complete: function () {
                    console.log('complete');
                    self[component + 'UrnChecked'] = true;
                }
            });
        },
        validateUrn: function (urn, component) {
            var self = this;

            $.ajax({
                url: self.estabLookup.replace('{0}', urn),
                dataType: 'json',
                method: 'get',
                success: function (data) {
                    console.log('validate ' + component + ' URN: success');
                    self[component + 'Valid'] = !data.notFound;
                    if (self[component + 'Valid']) {
                        if (self.mergerType === 'merger') {
                            self.mergerEstabs.push(data.returnValue);
                        } else {
                            self.amalgamationEstabs.push(data.returnValue);
                        }
                    }
                },
                error: function (jqxhr) {
                    console.log('validate ' + component + ' URN: error');
                    if (jqxhr.hasOwnProperty('responseJSON')) {
                        self.apiError = jqxhr.responseJSON;
                    }
                    self[component + 'Valid'] = false;
                },
                complete: function () {
                    self.fieldCount--;
                    if (component == 'leadEstab') {
                        self.leadEstabUrnCheckDone = true;
                    } else {
                        self.linkedEstabUrnCheckDone = true;
                    }
                }
            });
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

        validateAmalgamationSelection: function () {
            this.fieldCount = 0;
            var self = this;
            this.amalgamationEstabs = [];

            var presentValidation = function () {
                if (!self.amalgamatedEstab0Valid || !self.amalgamatedEstab1Valid || !self.amalgamatedEstab2Valid || !self.amalgamatedEstab3Valid) {
                    self.amalgamateUrnError = true;
                    self.errorFocus();
                }
                if (self.amalgamatedEstab0Valid &&
                    self.amalgamatedEstab1Valid &&
                    self.amalgamatedEstab2Valid &&
                    self.amalgamatedEstab3Valid) {
                        self.validMergeUrns = true;
                        self.clearErrors();
                }
                self.isProcessing = false;
            }

            this.linkedEstabError = false;
            this.amalgamatedEstab0Valid = true;
            this.amalgamatedEstab1Valid = true;
            this.amalgamatedEstab2Valid = true;
            this.amalgamatedEstab3Valid = true;
            this.amalgamationLengthError = false;
            this.amalgamateUrnError = false;
            this.commitErrors = '';
            this.duplicateUrnsError = this.hasDuplicateUrn();
            if (this.duplicateUrnsError) {
                return true;
            }

            var promise = [];

            if (this.amalgamatedEstab0 !== '') {
                this.amalgamatedEstab0Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.amalgamatedEstab0, 'amalgamatedEstab0'));

            }

            if (this.amalgamatedEstab1 !== '') {
                this.amalgamatedEstab1Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.amalgamatedEstab1, 'amalgamatedEstab1'));
            }

            if (this.amalgamatedEstab2 !== '') {
                this.amalgamatedEstab2Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.amalgamatedEstab2, 'amalgamatedEstab2'));
            }

            if (this.amalgamatedEstab3 !== '') {
                this.amalgamatedEstab3Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.amalgamatedEstab3, 'amalgamatedEstab3'));
            }

            if (promise.length >= 2) {
                this.isProcessing = true;
                $.when(promise.join(',')).done(
                    function () {
                        var tt;
                        if (self.fieldCount > 0) {
                            tt = window.setInterval(function () {
                                    if (self.fieldCount === 0) {
                                        presentValidation();
                                        window.clearInterval(tt);
                                    }
                                },
                                100);
                        } else {
                            presentValidation();
                        }
                    });
            } else {
                self.amalgamationLengthError = true;
                self.errorFocus();
            };
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
            this.mergerTypeError = false;
            this.amalgamateUrnError = false;
            this.leadEstabError = false;
            this.leadEstabLengthError = false;
            this.linkedEstabError = false;
            this.amalgamationLengthError = false;
            this.nameError = false;
            this.phaseError = false;
            this.typeError = false;
            this.laError = false;
            this.mergeDateError = false;
            this.mergeLengthError = false;
            this.duplicateUrnsError = false;
            this.commitErrors = false;
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
