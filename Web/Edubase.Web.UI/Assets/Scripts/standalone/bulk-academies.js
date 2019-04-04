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
        leadEstabValid: false,
        leadEstabErrorMessage: '',

        linkedEstab0Error: false,
        linkedEstab1Error: false,
        linkedEstab2Error: false,
        linkedEstabError: false,

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
        amalgUrn: ''
    },
    created: function () {
        this.populateSelect('new-establishment-type', this.types);
        this.populateSelect('LocalAuthorityId', this.localAuthorities);
    },
    computed: {

        displayDate: function () {
            console.log('display date calc');
            return this.mergeDateDay + '/' + this.mergeDateMonth + '/' + this.mergeDateYear;
        },
        showGlobalError: function () {
            console.log('computing globael');

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
                    this.commitErrors
            );
        },
        schoolDetailUrl: function () {
            return "/Establishments/Establishment/Details/" + this.leadEstab;
        },
        amalgUrl: function () {
            return '/Establishments/Establishment/Details/' + this.amalgUrn;
        },
        showGlobalError: function () {
            return false;
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
                var typeName = self.types.filter(function (t) {
                    return t.id == self.typeId;
                })[0].name;
            }
        }
    },
    methods: {
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
            var validPhaseIds = tp[this.typeId];

            validPhaseIds.forEach(function (j) {
                var obj = {}
                obj.name = phases[j].name;
                obj.id = j;
                validOptions.push(obj);
            });

            this.populateSelect('new-establishment-phase', validOptions)
        },
        checkMergeType: function () {
            this.mergerTypeError = this.mergerType === '';
            if (!this.mergerTypeError) {
                this.mergerTypeConfirmed = true;
            }
        },
        validateMergeSelection: function () {
            this.fieldCount = 0;
            var self = this;
            var presentValidation = function () {
                if (!self.leadEstabValid) {
                    self.leadEstabError = true;
                }
                if (!self.linkedEstab0Valid || !self.linkedEstab1Valid || !self.linkedEstab2Valid) {
                    self.linkedEstabError = true;
                }
            }

            self.linkedEstabError = false;
            self.leadEstabError = false;
            self.linkedEstab0Valid = true;
            self.linkedEstab1Valid = true;
            self.linkedEstab2Valid = true;
            self.mergeLengthError = false;
            var promise = [];
            if (this.leadEstab !== '') {
                this.leadEstabValid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.leadEstab, 'leadEstab'));

            }

            if (this.linkedEstab0 !== '') {
                this.linkedEstab0Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.linkedEstab0, 'linkedEstab0'));
            }

            if (this.linkedEstab1 !== '') {
                this.linkedEstab1Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.linkedEstab1, 'linkedEstab1'));
            }

            if (this.linkedEstab2 !== '') {
                this.linkedEstab2Valid = false;
                this.fieldCount++;
                promise.push(this.validateUrn(this.linkedEstab2, 'linkedEstab2'));
            }

            if (promise.length > 1) {
                $.when(promise.join(',')).done(
                    function () {
                        var tt;
                        if (self.fieldCount > 0) {
                            tt = window.setInterval(function () {
                                    if (self.fieldCount === 0) {
                                        presentValidation();
                                        window.clearInterval(tt);
                                        self.validMergeUrns = true;
                                    }
                                },
                                100);
                        } else {
                            presentValidation();
                        }
                    });
            } else {
                self.mergeLengthError = true;
            }

        },
        validateUrn: function (urn, component) {
            var self = this;

            $.ajax({
                url: self.estabLookup.replace('{0}', urn),
                dataType: 'json',
                method: 'get',
                success: function (data) {
                    self[component + 'Valid'] = !data.notFound;
                    if (self[component + 'Valid']) {
                        if (self.mergerType === 'merger') {
                            self.mergerEstabs.push(data.returnValue);
                        } else {
                            self.amalgamationEstabs.push(data.returnValue);
                        }
                    }
                },
                error: function () {
                    self[component + 'Valid'] = false;
                },
                complete: function () {
                    self.fieldCount--;
                }
            });
        },
        validateMergerDate: function () {
            var day = parseInt(this.mergeDateDay, 10),
                month = parseInt(this.mergeDateMonth, 10),
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
            var amalgamationUrns = [];

            var presentValidation = function () {
                if (!self.amalgamatedEstab0Valid || !self.amalgamatedEstab1Valid || !self.amalgamatedEstab2Valid || !self.amalgamatedEstab3Valid) {
                    self.amalgamateUrnError = true;
                }
            }

            self.linkedEstabError = false;
            self.amalgamatedEstab0Valid = true;
            self.amalgamatedEstab1Valid = true;
            self.amalgamatedEstab2Valid = true;
            self.amalgamatedEstab3Valid = true;
            self.amalgamationLengthError = false;
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
                $.when(promise.join(',')).done(
                    function () {
                        var tt;
                        if (self.fieldCount > 0) {
                            tt = window.setInterval(function () {
                                    if (self.fieldCount === 0) {
                                        presentValidation();
                                        window.clearInterval(tt);
                                        self.validMergeUrns = true;
                                    }
                                },
                                100);
                        } else {
                            presentValidation();
                        }
                    });
            } else {
                self.amalgamationLengthError = true;
            }

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

            postData.UrnsToMerge = postData.UrnsToMerge.filter(function (item) {
                return item != self.leadEstab;
            })
            console.log(postData.UrnsToMerge);


            this.mergeDateError = this.validateMergerDate();


            if (!this.mergeDateError) {
                this.validLinkDate = true;
                $.ajax({
                    url: self.commitApi,
                    method: 'post',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(postData),
                    success: function (data) {
                        if (data.hasOwnProperty('successful') && data.successful) {
                            self.mergerComplete = true;
                        }
                    },
                    error: function (jqXHR) {
                        var errObj = JSON.parse(jqXHR.responseText);
                        var errMessage = '';

                        for (var item in errObj) {
                            if (item === 'validationEnvelope') {
                                var env = errObj[item][0].errors;
                                env.forEach(function (er) {
                                    console.log(errObj[item][0].errors);
                                    console.log(er);
                                    errMessage += er.message + '<br>';
                                });
                            }
                        }
                        self.commitErrors = errMessage;
                    }
                });
            }
        },
        processAmalgamation: function () {
            var self = this;
            var postData = {};

            this.nameError = (this.newName.length < 1);
            this.establishmentTypeError = (this.estabTypeId === '');
            this.amalgamationLaError = (this.la === '');
            this.phaseError = (this.estabPhaseId === '');
            this.amalgamationDateError = this.validateMergerDate();

            if (!this.nameError &&
                !this.establishmentTypeError &&
                !this.amalgamationLaError &&
                !this.phaseError &&
                !this.amalgamationDateError &&
                !this.duplicateUrnsError) {

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
                        console.log(data);
                        if (data.hasOwnProperty('successful') && data.successful) {
                            self.completeAmalgamation = true;
                            self.amalgUrn = data.response.amalgamateNewEstablishmentUrn;
                        }
                    },
                    error: function (jqXHR) {
                        var errObj = JSON.parse(jqXHR.responseText);
                        var errMessage = '';

                        for (var item in errObj) {
                            if (item === 'validationEnvelope') {
                                var env = errObj[item][0].errors;
                                env.forEach(function (er) {
                                    console.log(errObj[item][0].errors);
                                    console.log(er);
                                    errMessage += er.message + '<br>';
                                });
                            }
                        }
                        self.commitErrors = errMessage;
                    }
                });
            }
        }
    }

});
