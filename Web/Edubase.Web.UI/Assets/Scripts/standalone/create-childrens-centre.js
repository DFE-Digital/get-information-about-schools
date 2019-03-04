var ccGroup = new Vue({
    el: '#create-childrens-centre',
    data: {
        groupType: '8',
        groupName: '',
        groupNameError: false,
        groupNameWarningMessage: '',
        groupNameApiError: '',

        groupStatus: 'Open',
        groupLead: '',

        openDateDay: '',
        openDateMonth: '',
        openDateYear: '',

        la: '',
        laError: false,
        searchUrn: '',
        centresInGroup: [],
        pendingEstab: {},


        step0Complete: false,
        step1Complete: false,

        joinDateValidateUrl: '/Groups/Group/CreateChildrensCentre/Validate/JoinedDate',
        openDateValidateUrl: '/Groups/Group/CreateChildrensCentre/Validate/OpenDate',
        openDateError: false,
        openDateErrorMessage: '',

        joinDateDay: '',
        joinDateMonth: '',
        joinDateYear: '',
        joinDateError: '',
        joinDateErrorMessage: '',

        urnLookupUrl: '/api/establishment/{0}',
        validateUrl: '/Groups/Group/CreateChildrensCentre/Validate/All',
        apiErrors: [],
        urnApiErrors: [],
        urnError: false,
        duplicateUrnError: false,
        tooFewCentresError: false,
        appState: 'initial', // initial || groupDetail || addCentre || detail
        pendingEdit: false,
        detailEdit: false,
        isProcessing: false,
        apiError: {}

    },
    computed: {
        groupTypeName: function() {
            if (this.groupType == 8) {
                return 'group';
            }
            return 'collaboration';
        },
        wasWere: function() {
            if (this.apiErrors.length > 1) {
                return ' were ';
            }
            return ' was an ';
        },
        openDate: function () {
            if (this.openDateDay !== '' && this.openDateMonth !== '' && this.openDateYear !== '') {
                return [this.openDateDay, this.openDateMonth, this.openDateYear].join('/');
            }
            return '';
        },
        joinDate: function () {
            if (this.joinDateDay !== '' && this.joinDateMonth !== '' && this.joinDateYear !== '') {
                return [this.joinDateDay, this.joinDateMonth, this.joinDateYear].join('/');
            }
            return '';
        },
        laName: function () {
            if (document.getElementById('LocalAuthorityId') && document.getElementById('LocalAuthorityId').tagName.toLowerCase() ==='select') {
                return $('#LocalAuthorityId').find('option[value="' + this.la + '"]').text();
            }
            // no dropdown for LA users - LA is uneditable for them
            this.la = document.getElementById('la-code').value;
            return document.getElementById('uneditable-la-name').value;
        },
        tableCount: function () {
            return '1 - ' + this.centresInGroup.length + ' of ' + this.centresInGroup.length;
        },
        addedUrns: function() {
            return this.centresInGroup.map(function(estab) {
                return estab.urn;
            });
        },
        pendingEstabAddress: function () {
            var address = [];
            var estab = this.pendingEstab;

            if (estab.address_Line1) {
                address.push(estab.address_Line1);
            }
            if (estab.address_Line2) {
                address.push(estab.address_Line2);
            }
            if (estab.address_Line3) {
                address.push(estab.address_Line3);
            }
            if (estab.address_Locality) {
                address.push(estab.address_Locality);
            }
            if (estab.address_CityOrTown) {
                address.push(estab.address_CityOrTown);
            }
            if (estab.address_PostCode) {
                address.push(estab.address_PostCode);
            }

            return address.join(', ');
        }
    },
    methods: {
        submitCentres: function () {
            var self = this;
            var centres = this.centresInGroup;
            var len = centres.length;
            var i = 0;
            var frag = document.createDocumentFragment();
            var form = document.getElementById('create-childrens-centres');
            var fields = [
                'Urn',
                'JoinedDateEditable.Day',
                'JoinedDateEditable.Month',
                'JoinedDateEditable.Year',
                'CCIsLeadCentre'
            ];
            var estabs = [];

            this.tooFewCentresError = this.centresInGroup.length < 2;

            if (this.tooFewCentresError) {
                return;
            }

            this.isProcessing = true;

            function formatComDate(date) {
                var d = date.split('/');
                return [d[2], d[1], d[0]].join('-');
            }

            for (i = 0; i < len; i++) {
                var o = {};
                var centre = self.centresInGroup[i];
                o.urn = centre.urn;
                o.joinedDate = formatComDate(centre.joinDate);
                o.cCIsLeadCentre = centre.urn === self.groupLead;
                estabs.push(o);
            }

            var validationObj = {
                "groupTypeId": self.groupType,
                "name": self.groupName,
                "openDate": [self.openDateYear, self.openDateMonth, self.openDateDay].join('-'),
                "localAuthorityId": self.la,
                "establishments": estabs
            }

            self.apiErrors = [];
            function validate() {
                return $.ajax({
                    url: self.validateUrl,
                    method: 'post',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(validationObj),
                    success: function (data) {
                        if (data.HasErrors) {
                            $(data.Errors).each(function (n, error) {
                                var o = {};
                                /// if error.Fields contains a digit, it's a problem with an estab -> look up URN
                                if (/\d/.test(error.Fields)) {
                                    var est = self.centresInGroup[error.Fields.replace(/\D/g, '')];
                                    o.field = est.name + ' (' + est.urn + ')';
                                } else {
                                    o.field = error.Fields;
                                }
                                o.message = error.Message;
                                self.apiErrors.push(o);
                            });
                        }
                    },
                    error: function (jqXhr) {
                        if (jqXhr.hasOwnProperty('responseJSON')) {
                            self.apiError = jqXhr.responseJSON;
                        }
                    }
                });
            }

            $.when(validate()).done(function () {

                if (self.apiErrors.length === 0) {
                    $('#processed-warnings').val(true);
                    for (i = 0; i < len; i++) {
                        var centre = centres[i];
                        for (var j = 0, fLen = fields.length; j < fLen; j++) {
                            var input = document.createElement('input');
                            input.type = 'hidden';
                            input.name = 'LinkedEstablishments.Establishments[' + i + '].' + fields[j];

                            switch (j) {
                            case 0:
                                input.value = centre.urn;
                                break;
                            case 1:
                                input.value = centre.joinDate.split('/')[0];
                                break;
                            case 2:
                                input.value = centre.joinDate.split('/')[1];
                                break;
                            case 3:
                                input.value = centre.joinDate.split('/')[2];
                                break;
                            case 4:
                                input.value = centre.urn === self.groupLead;
                                break;
                            }
                            frag.appendChild(input);
                        }
                    }

                    form.appendChild(frag);

                    window.setTimeout(function() {
                            form.submit();
                        },
                        100);
                } else {
                    self.isProcessing = false;
                }
            });
        },
        capitalise: function(str) {
            return str.charAt(0).toUpperCase() + str.slice(1);
        },
        validateDate: function (dateProp) {
            var self = this;
            var opData = {
                Day: self.openDateDay,
                Month: self.openDateMonth,
                Year: self.openDateYear,
                Label: ""
            };
            var opUrl = dateProp === 'joinDate' ? self.joinDateValidateUrl : self.openDateValidateUrl;

            if (dateProp === 'joinDate') {
                opData = {
                    "JoinDate": {
                        "Day": self.joinDateDay,
                        "Month": self.joinDateMonth,
                        "Year": self.joinDateYear
                    },
                    "GroupOpenDate": {
                        "Day": self.openDateDay,
                        "Month": self.openDateMonth,
                        "Year": self.openDateYear
                    },
                    "GroupType": self.groupTypeName
                }
            }

            self.openDateError = false;
            self.joinDateError = false;
            self.isProcessing = true;

            return $.ajax({
                url: opUrl,
                method: 'post',
                dataType: 'json',
                data: opData,
                success: function (data) {
                    self[dateProp + 'Error'] = data.length > 0;
                    var errors = [];
                    $.each(data,
                        function (n, err) {
                            errors.push(err.Value.Errors[0].ErrorMessage);
                        });

                    self[dateProp + 'ErrorMessage'] = errors.join('<br>');
                    self.isProcessing = false;
                },
                error: function() {
                    self.isProcessing = false;
                }
            });
        },
        lookupUrn: function () {
            var self = this;
            this.urnError = false;
            this.apiErrors = [];
            this.urnApiErrors = [];
            this.tooFewCentresError = false;

            this.duplicateUrnError = this.addedUrns.indexOf(Number(this.searchUrn)) > -1;

            if (this.duplicateUrnError) {  return; }

            if (isNaN(this.searchUrn) || this.searchUrn === '') {
                this.urnError = true;
                return;
            }
            this.isProcessing = true;
            $.ajax({
                url: self.urnLookupUrl.replace('{0}', self.searchUrn),
                dataType: 'json',
                method: 'get',
                success: function (data) {
                    self.urnError = data.notFound;
                    if (!self.urnError) {
                        self.pendingEstab = data.returnValue;
                        self.remoteUrnValidation();
                    }
                },
                error: function (jqxhr) {
                    if (jqxhr.hasOwnProperty('responseJSON')) {
                        self.apiError = jqxhr.responseJSON;
                    }
                    self.urnError = true;
                    self.isProcessing = false;
                }
            });
        },
        remoteUrnValidation: function() {
            var self = this;
            var o = {};
            var today = new Date();
            var centre = self.pendingEstab;
            o.urn = centre.urn;
            o.joinedDate = [self.openDateYear, self.openDateMonth, self.openDateDay].join('-');
            o.cCIsLeadCentre = centre.urn === self.groupLead;

            var validationObj = {
                "groupTypeId": self.groupType,
                "name": self.groupName,
                "openDate": [self.openDateYear, self.openDateMonth, self.openDateDay].join('-'),
                "localAuthorityId": self.la,
                "establishments": [o]
            }
            $.ajax({
                url: self.validateUrl,
                method: 'post',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(validationObj),
                success: function (data) {
                    if (data.HasErrors > 0) {
                        $(data.Errors).each(function(n, error) {
                            var o = {};
                            // ensure that only URN errors are included
                            // and ignore the message about the number of establishments in the group - this validation of a single URN
                            if (/\d/.test(error.Fields) && error.Message.indexOf('must have more than one linked') === -1) {
                                var est = self.pendingEstab;
                                o.field = est.name + ' (' + est.urn + ')';
                                o.message = error.Message;
                                self.urnApiErrors.push(o);
                            }
                        });
                        self.isProcessing = false;
                    } else {
                        self.isProcessing = false;
                    }
                    if (self.urnApiErrors.length === 0) {
                        self.appState = 'detail';
                    }
                },
                error: function (jqXhr) {
                    if (jqXhr.hasOwnProperty('responseJSON')) {
                        self.apiError = jqXhr.responseJSON;
                    }
                    self.isProcessing = false;
                }
            });
        },
        addToGroup: function () {
            var self = this;
            $.when(self.validateDate('joinDate')).done(function () {
                self.isProcessing = false;
                if (!self.joinDateError) {

                    self.pendingEstab.joinDate = self.joinDate;

                    self.centresInGroup.push(self.pendingEstab);
                    self.pendingEstab = {};
                    self.searchUrn = '';
                    self.joinDateDay = '';
                    self.joinDateMonth = '';
                    self.joinDateYear = '';
                    self.appState = 'addCentre';

                    if (self.centresInGroup.length === 1 && self.groupType == 8) {
                        self.groupLead = self.centresInGroup[0].urn;
                    }
                }
            });
        },
        editJoiningEstab: function (urn) {
            this.searchUrn = urn;
            this.pendingEdit = true;
            this.apiErrors = [];
            this.tooFewCentresError = false;
            this.pendingEstab = this.centresInGroup.filter(function (estab) {
                if (urn === estab.urn) {
                    return estab;
                }
            })[0];

            var pos = this.centresInGroup.map(function (estab) {
                return estab.urn;
            }).indexOf(urn);

            this.centresInGroup.splice(pos, 1);
            var d = this.pendingEstab.joinDate;
            this.joinDateDay = d.split('/')[0];
            this.joinDateMonth = d.split('/')[1];
            this.joinDateYear = d.split('/')[2];
            this.appState = 'detail';
        },
        removeJoiningEstab: function (urn) {
            this.apiErrors = [];
            this.tooFewCentresError = false;
            var pos = this.centresInGroup.map(function (estab) {
                return estab.urn;
            }).indexOf(urn);
            this.centresInGroup.splice(pos, 1);
        },
        generateRadioId: function (urn) {
            return 'lead-estab' + urn;
        },
        step0Continue: function () {
            this.step0Complete = true;
            this.appState = 'groupDetail';
        },
        allowStep1Continue: function () {
            return $.trim(this.groupName) !== '' && $.trim(this.la) !== '' && $.trim(this.openDateDay) !== '' && $.trim(this.openDateMonth) !== '' && $.trim(this.openDateYear) !== '';
        },
        allowAddToGroup: function () {
            return $.trim(this.la) !== '' && $.trim(this.joinDateDay) !== '' && $.trim(this.joinDateMonth) !== '' && $.trim(this.joinDateYear) !== '';
        },
        step1Continue: function () {
            var self = this;
            $.when(this.validateDate('openDate')).done(function () {
                self.groupNameError = $.trim(self.groupName) === '';
                self.laError = self.la === '';
                self.groupNameApiError = '';
                if (!self.groupNameError && !self.laError && !self.openDateError) {
                    self.isProcessing = true;
                    var validationObj = {
                        'groupTypeId': self.groupType,
                        'name': self.groupName,
                        'openDate': [self.openDateYear, self.openDateMonth, self.openDateDay].join('-'),
                        'localAuthorityId': self.la,
                        "establishments": []
                    };
                    $.ajax({
                        url: self.validateUrl,
                        method: 'post',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: JSON.stringify(validationObj),
                        success: function(data) {
                            if (data.HasErrors) {
                                self.groupNameApiError = data.Errors[0].Message;
                            } else if (data.HasWarnings) {
                                self.groupNameWarningMessage =
                                    " <strong>There is already a group with a similar name:</strong><br> " +
                                    data.Warnings[0].MessageParameters[0] +
                                    ', <strong>UID:</strong> ' +
                                    data.Warnings[0].MessageParameters[1];
                            }

                            if (!data.HasErrors) {
                                self.appState = 'addCentre';
                            }
                            self.isProcessing = false;
                        },
                        error: function(jqxhr) {
                            if (jqxhr.hasOwnProperty('responseJSON')) {
                                self.apiError = jqxhr.responseJSON;
                            }
                        }
                    });
                }
            });

        },
        cancelEdit : function() {
            this.addToGroup();
            this.pendingEdit = false;
        }
    }
});
