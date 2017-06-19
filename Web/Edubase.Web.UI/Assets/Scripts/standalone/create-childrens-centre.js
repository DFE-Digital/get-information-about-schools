(function () {
    var ccGroup = new Vue({
        el: '#create-childrens-centre',
        data: {
            groupType: '9',
            groupName: '',
            groupNameError: false,

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

            dateValidateUrl: '/Groups/Group/CreateChildrensCentre/Validate/JoinedDate',
            openDateError: false,
            openDateErrorMessage: '',

            joinDateDay: '',
            joinDateMonth: '',
            joinDateYear: '',
            joinDateError: '',
            joinDateErrorMessage: '',

            urnLookupUrl: '/api/establishment/{0}',
            urnError: false,
            appState: 'initial' // initial || groupDetail || addCentre || detail

        },
        computed: {
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
                return $('#LocalAuthorityId').find('option[value="' + this.la + '"]').text();
            },
            tableCount: function () {
                return '1 - ' + this.centresInGroup.length + ' of ' + this.centresInGroup.length;
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
            validateDate: function (dateProp) {
                var self = this;
                self.openDateError = false;
                self.joinDateError = false;

               return $.ajax({
                    url: self.dateValidateUrl,
                    method: 'post',
                    dataType: 'json',
                    data: {
                        Day: self[dateProp + 'Day'],
                        Month: self[dateProp + 'Month'],
                        Year: self[dateProp + 'Year'],
                        Label: ""
                    },
                    success: function (data) {
                        self[dateProp + 'Error'] = data.length > 0;
                        var errors = [];
                        $.each(data,
                            function (n, err) {
                                errors.push(err.Value.Errors[0].ErrorMessage);
                            });

                        self[dateProp + 'ErrorMessage'] = errors.join('<br>');

                        //return errors;
                    }
                });
            },
            lookupUrn: function () {
                var self = this;
                this.urnError = false;
                if (isNaN(this.searchUrn) || this.searchUrn === '') {
                    this.urnError = true;
                    return;
                }
                $.ajax({
                    url: self.urnLookupUrl.replace('{0}', self.searchUrn),
                    dataType: 'json',
                    method: 'get',
                    success: function (data) {
                        self.urnError = data.notFound;
                        if (!self.urnError) {
                            self.pendingEstab = data.returnValue;
                            self.appState = 'detail';
                        }
                    },
                    error: function () {
                        self.urnError = true;
                    }
                });
            },
            addToGroup: function () {
                var self = this;
                $.when(self.validateDate('joinDate')).done(function () {
                    if (!self.joinDateError) {
                        
                        self.pendingEstab.joinDate = self.joinDate;

                        self.centresInGroup.push(self.pendingEstab);
                        self.pendingEstab = {};
                        self.searchUrn = '';
                        self.joinDateDay = '';
                        self.joinDateMonth = '';
                        self.joinDateYear = '';
                        self.appState = 'addCentre';

                        if (self.centresInGroup.length === 1) {
                            self.groupLead = self.centresInGroup[0].urn;
                        }
                    }
                });
            },
            editJoiningEstab: function (urn) {
                var self = this;
                this.searchUrn = urn;
                this.pendingEstab = this.centresInGroup.filter(function (estab) {
                    if (urn === estab.urn) {
                        return estab;
                    }
                })[0];
                var pos = this.centresInGroup.map(function (estab) {
                    return estab.urn === urn;
                });
                this.centresInGroup.splice(pos, 1);

                var d = this.pendingEstab.joinDate;
                this.joinDateDay = d.split('/')[0];
                this.joinDateMonth = d.split('/')[1];
                this.joinDateYear = d.split('/')[2];
                this.appState = 'detail';
            },
            removeJoiningEstab: function (urn) {
                var pos = this.centresInGroup.map(function (estab) {
                    return estab.urn === urn;
                });
                this.centresInGroup.splice(pos, 1);
            },
            generateRadioId: function (urn) {
                return 'lead-estab' + urn;
            },
            step0Continue: function () {
                this.step0Complete = true;
                this.appState = 'groupDetail';
            },
            step1Continue: function () {
                var self = this;
                $.when(this.validateDate('openDate')).done(function () {
                    self.groupNameError = $.trim(self.groupName) === '';
                    self.laError = self.la === '';

                    if (!self.groupNameError && !self.laError && !self.openDateError) {
                        self.appState = 'addCentre';
                    }
                });

            }
        }
    });
}());