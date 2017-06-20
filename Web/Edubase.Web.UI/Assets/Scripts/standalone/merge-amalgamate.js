(function () {


    var mergersApp = new Vue({
        el: '#mergers-app',
        data: {
            mergerType: '',
            validSelection: false,
            mergerTypeConfirmed: false,
            leadEstablishmentUrn: '',
            leadEstablishmentName: '',
            linkedEstablishments: [],
            linkedEstab0: '',
            linkedEstab1: '',
            linkedEstab2: '',
            linkedEstabsValid: [],
           
            amalgamatedEstablishments: [],
            validatedAmalgamatedEstablishments: [],
            establishmentsSelected: false,
            mergerTypeError: false,
            leadEstablishmentError: false,
            linkedEstablishmentError: false,
            amalgamateUrnError: false,
            validLinkDate: false,
            mergedDateDay: '',
            mergedDateMonth: '',
            mergedDateYear: '',
            mergedDateError: false,
            amalgamationDateDay: '',
            amalgamationDateMonth: '',
            amalgamationDateYear: '',
            amalgamationDateError: false,
            selectedLa: '',
            selectedLaName: '',
            selectedEstablishmentTypeId: '',
            selectedEstablishmentType: '',
            selectedEstablishmentPhase: '',
            newEstablishmentName: '',
            amalgamationLengthError: false,
            amalgamationUrnError: false,
            amalgamationEstablishmentsConfirmed: false,
            amalgamationDetailsConfirmed: false,
            amalgamationNameError: false,
            phaseError: false,
            establishmentTypeError: false,
            amalgamationLaError: false,
            completeAmalgamation: false,
            duplicateUrnsError: false,

            estabLookup: '/api/establishment/{0}',
            commitApi: '/api/amalgamate-merge',

            estabTypeId: '',
            estabPhaseId: '',
            la: '',
            typePhases: typePhases,
            commitErrors: '',
            amalgUrn: '',
            presentExitWarning: false
        },
        created: function () {
            this.buildTypesDropDown();
            blockExits();
        },
        computed: {
            showGlobalError: function () {
                return (this.mergerTypeError ||
                    this.amalgamateUrnError ||
                    this.leadEstablishmentError ||
                    this.linkedEstablishmentError ||
                    this.mergedDateError ||
                    this.amalgamationLengthError ||
                    this.amalgamationUrnError ||
                    this.amalgamationNameError ||
                    this.phaseError ||
                    this.establishmentTypeError ||
                    this.amalgamationLaError ||
                    this.amalgamationDateError ||
                    this.duplicateUrnsError ||
                    this.commitErrors);
            },
            displayDate: function () {
                return this.mergedDateDay + '/' + this.mergedDateMonth + '/' + this.mergedDateYear;
            },
            amalgamationDate: function () {
                return this.amalgamationDateDay + '/' + this.amalgamationDateMonth + '/' + this.amalgamationDateYear;
            },
            schoolDetailUrl: function (urn) {
                 return "/Establishment/Details/" + this.leadEstablishmentUrn;                               
            },
            amalgUrl: function() {
                return '/Establishment/Details/' + this.amalgUrn;
            }

        },
        methods: {
            checkMergeType: function () {
                if (this.mergerType === '') {
                    this.mergerTypeError = true;
                } else {
                    this.mergerTypeError = false;
                    this.mergerTypeConfirmed = true;
                }
            },
            setLeadEstablishment: function (urn) {
                var self = this;
                $.ajax({
                    url: self.estabLookup.replace('{0}', urn),
                    dataType: 'json',
                    method: 'get',
                    success: function (data) {
                        self.leadEstablishmentError = data.notFound;
                        if (!self.urnError) {
                            self.leadEstablishmentUrn = data.returnValue.urn;
                            self.leadEstablishmentName = data.returnValue.name;
                        }
                    },
                    error: function () {
                        self.leadEstablishmentError = true;
                    }

                });

            },
            addEstablishment: function (urn, pos) {
                var estabDetails;
                var self = this;
                
                if (urn === '') {
                    return true;
                }               

                $.ajax({
                    url: self.estabLookup.replace('{0}', urn),
                    dataType: 'json',
                    method: 'get',
                    success: function (data) {
                        self.linkedEstablishmentError = data.notFound;
                        self.linkedEstabsValid[pos] = data.notFound;
                        if (!self.linkedEstablishmentError) {
                            estabDetails = data.returnValue;
                            self.linkedEstablishments.splice(pos, 1, estabDetails);
                            self.linkedEstabsValid[pos] = true;
                        }
                    },
                    error: function () {
                        self.linkedEstablishmentError = true;
                        self.linkedEstabsValid[pos] = false;
                    }
                });

            },
           
            validateAmalgamationUrns: function (urn) {
                var self = this,
                    estabDetails;

                self.amalgamateUrnError = false;

                function checkAmalgamationUrn() {
                    return $.ajax({
                        url: self.estabLookup.replace('{0}', urn),
                        dataType: 'json',
                        method: 'get',
                        success: function (data) {
                            self.amalgamateUrnError = data.notFound;
                            if (!self.amalgamateUrnError) {
                                estabDetails = data.returnValue;
                                self.amalgamatedEstablishments.push(estabDetails);
                            }
                        },
                        error: function () {
                            self.amalgamateUrnError = true;
                        }

                    });
                }

                if (urn !== '') {
                    var addedUrns = self.amalgamatedEstablishments.map(function (estab) {
                        return estab.id;
                    });

                    self.duplicateUrnsError = (addedUrns.indexOf(urn) > -1);

                    if (self.duplicateUrnsError) {
                        return false;
                    }
                    checkAmalgamationUrn();
                }


            },
            moveToAmalgationDetails: function () {
                if (!this.duplicateUrnsError) {
                    if (this.amalgamatedEstablishments.length < 2) {
                        this.amalgamationLengthError = true;
                    } else {
                        this.amalgamationLengthError = false;
                        this.amalgamationEstablishmentsConfirmed = true;
                    }
                }
            },
            processAmalgamation: function () {
                var self = this;
                var postData = {};

                this.amalgamationNameError = (this.newEstablishmentName.length < 1);
                this.establishmentTypeError = (this.estabTypeId === '');
                this.amalgamationLaError = (this.la === '');
                this.phaseError = (this.estabPhaseId === '');
                this.amalgamationDateError = this.checkMergeDate();

                if (!this.amalgamationNameError &&
                    !this.establishmentTypeError &&
                    !this.amalgamationLaError &&
                    !this.phaseError &&
                    !this.amalgamationDateError &&
                    !this.duplicateUrnsError) {

                    postData.operationType = 'amalgamate';
                    postData.MergeOrAmalgamationDate = [this.amalgamationDateYear, this.amalgamationDateMonth, this.amalgamationDateDay].join('-');
                    postData.UrnsToMerge = this.amalgamatedEstablishments.map(function (estab) {
                        return estab.urn;
                    });
                    postData.NewEstablishmentName = this.newEstablishmentName;
                    postData.NewEstablishmentPhaseId = this.estabPhaseId;
                    postData.NewEstablishmentTypeId = this.estabTypeId;
                    postData.NewEstablishmentLocalAuthorityId = this.la;


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

            },
            processMerger: function () {
                var self = this;
                var postData = {};

                postData.operationType = 'merge';
                postData.MergeOrAmalgamationDate = [this.mergedDateYear, this.mergedDateMonth, this.mergedDateDay].join('-');
                postData.LeadEstablishmentUrn = this.leadEstablishmentUrn;
                postData.UrnsToMerge = this.linkedEstablishments.map(function (estab) {
                    return estab.urn;
                });
                this.mergedDateError = this.checkMergeDate();


                if (!this.mergedDateError) {
                    this.validLinkDate = true;
                    $.ajax({
                        url: self.commitApi,
                        method: 'post',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: JSON.stringify(postData),
                        success: function (data) {
                            console.log(data);
                            if (data.hasOwnProperty('successful') && data.successful) {

                            }
                        },
                        error: function () {

                        }
                    });
                }
            },
            checkMergeDate: function () {
                var day = this.mergerType === 'merger'
                        ? parseInt(this.mergedDateDay, 10)
                        : parseInt(this.amalgamationDateDay, 10),
                    month = this.mergerType === 'merger'
                        ? parseInt(this.mergedDateMonth, 10)
                        : parseInt(this.amalgamationDateMonth, 10),
                    year = this.mergerType === 'merger'
                        ? parseInt(this.mergedDateYear, 10)
                        : parseInt(this.amalgamationDateYear, 10),
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
            establishmentSelectionValid: function () {
                var self = this;
                this.linkedEstablishmentError = false;


                $.when(
                        self.addEstablishment(self.linkedEstab0, 0),
                        self.addEstablishment(self.linkedEstab1, 1),
                        self.addEstablishment(self.linkedEstab2, 2))
                    .done(function () {
                        var addedUrns = self.linkedEstablishments.map(function (estab) {
                            return estab.urn;
                        });
                        addedUrns.push(self.leadEstablishmentUrn);

                      //  self.duplicateUrnsError = (addedUrns.indexOf(urn) > -1);
                        var linkedEstabsValid = self.linkedEstabsValid.every(function(elem) {
                            return elem === true;
                        });
                        console.log(linkedEstabsValid);
                        self.validSelection = (
                        self.leadEstablishmentUrn !== '' &&
                        !self.leadEstablishmentError &&
                        self.linkedEstablishments.length > 0);

                

                        if (!self.validSelection) {
                            if (!self.leadEstablishmentName) {
                                self.leadEstablishmentError = true;

                            } else if (self.linkedEstablishments.length < 1) {
                                self.linkedEstablishmentError = true;
                            }
                        }
                    });               
            },
            restart: function () {
                if (!this.mergerTypeConfirmed) {
                    window.location = '/Tools';
                    return;
                }
                window.location = '/Tools/MergersTool';
            },
            updatePhases: function () {
                var tp = this.typePhases;
                var self = this;
                var options = '';
                console.log(self.estabTypeId);

                var allowedValuesObj = typePhases.filter(function (estabType) {
                    if (estabType.Code == self.estabTypeId) {
                        return estabType;
                    }
                })[0];

                $.each(allowedValuesObj.AllowedPhases, function (n, val) {
                    for (var k in val) {
                        if (val.hasOwnProperty(k)) {
                            options += '<option value="' + k + '">' + val[k] + '</option>';
                        }
                    }
                });


                $('#new-establishment-phase').html(options);
            },

            buildTypesDropDown: function () {
                var options = '';
                $.each(typePhases, function (i, val) {
                    options += '<option value="' + val.Code + '">' + val.EstablishmentType + '</option>';
                });

                $('#new-establishment-type').html(options);
            }
        }
    });

    function blockExits() {
        $('a, [value="cancel"]').on('click', function (e) {

            if (!mergersApp.completeAmalgamation || !mergersApp.validLinkDate) {
                e.preventDefault();
                mergersApp.presentExitWarning = true;
            }
        });
    }
}());
