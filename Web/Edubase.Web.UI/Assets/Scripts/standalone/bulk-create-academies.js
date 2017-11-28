﻿(function () {
    var today = new Date();
        var bulkAcademies = new Vue({
            el: '#bulk-academies',
            data: {
                establishments: [],
                addedUrns: [],
                searchUrn: '',
                establishmentType: '',
                invalidUrn: false,
                duplicateUrn: false,
                urnLookUpError: '',
                apiValidationError: '',

                estabTypeError: false,

                pendingEstab: {},
                pendingEdit: false,
                establishmentFound: false,
                openDateDay: today.getDate(),
                openDateMonth: today.getMonth() + 1,
                openDateYear: today.getFullYear(),
                openDateError: false,
                academyType: '',
                academyUnselectedError: false,
                dateError: false,
                noAcademyError: false,
                commitError: false,
                commitErrorMessage: '',
                displayExitWarning: false,
                exitUrl: '',
                urnApiUrl: '/api/establishment/{0}',
                validationUrl: '/api/bulk-create-academies/validate',
                commitUrl: '/api/bulk-create-academies',
                progressUrl: '/api/bulk-create-academies/progress/{0}',
                commitErrors: '',

                isProcessing: false,
                isCommiting: false,
                isSearching: false,
                createdCount: 0,
                isComplete: false,
                createdAcademies: [],
                pendingDelete: false,
                pendingDeleteItem: '',
                validTypes: [],
                apiError: {}


            },
            created: function() {
                this.blockExits();
            },
            computed: {
                showGlobalError: function() {
                    return (
                        this.estabTypeError ||
                            this.duplicateUrn ||
                            this.noAcademyTypeError ||
                            this.academyUnselectedError ||
                            this.openDateError ||
                            this.invalidUrn ||
                            this.commitError ||
                            this.urnLookUpError !== '' ||
                            this.apiValidationError !== ''
                    );
                },
                
                validationParams: function() {
                    return {
                        PredecessorEstablishmentUrn: self.searchUrn,
                        OpeningDate: [today.getFullYear(), today.getMonth(), today.getDate()].join('-'),
                        TypeId: 28
                    }

                },
                pageHeading: function() {
                    if (this.pendingEdit) {
                        return 'Edit details';
                    }
                    if (Object.keys(this.pendingEstab).length > 0) {
                        return 'Enter new academy details';
                    }
                    if (this.isComplete) {
                        return 'Academies created';
                    }
                    if (this.isCommiting) {
                        return 'Creating academies';
                    }

                    if (this.isProcessing) {
                        return '';
                    }
                    return 'Bulk create new academies';

                },

                displayDate: function() {

                    return [this.pad(this.openDateDay), this.pad(this.openDateMonth), this.openDateYear].join('/');
                },

                pendingEstabAddress: function() {
                    var self = this;
                    if (Object.keys(self.pendingEstab).length > 1) {
                        var address = [];
                        if (self.pendingEstab.address_Line1) {
                            address.push(self.pendingEstab.address_Line1);
                        }

                        if (self.pendingEstab.address_Line2) {
                            address.push(self.pendingEstab.address_Line2);
                        }

                        if (self.pendingEstab.address_Line3) {
                            address.push(self.pendingEstab.address_Line3);
                        }

                        if (self.pendingEstab.address_Locality) {
                            address.push(self.pendingEstab.address_Locality);
                        }

                        if (self.pendingEstab.address_CityOrTown) {
                            address.push(self.pendingEstab.address_CityOrTown);
                        }

                        if (self.pendingEstab.addresaddress_PostCodes_Locality) {
                            address.push(self.pendingEstab.address_PostCode);
                        }

                        return address.join(", ");
                    } else {
                        return '';
                    }
                }
            },
            methods: {
                pad: function(num) {
                    if (num < 10 && num.toString().length < 2) {
                        return '0' + num;
                    }
                    return num;
                },
                buildTypesDropDown: function () {
                    var self = this;
                    var frag = document.createDocumentFragment();
                    var select = document.getElementById('academy-type');
                    select.innerHTML = '';
                    var ps = document.createElement('option');
                    ps.appendChild(document.createTextNode('Please select'));
                   
                    frag.appendChild(ps);
                    for (var i = 0, len = this.validTypes.length; i < len; i++) {
                        var opt = document.createElement('option');
                        var t = this.validTypes[i];
                       
                        opt.value = t.id;
                        opt.innerHTML = t.name;
                        frag.appendChild(opt);
                    }
                    select.appendChild(frag);

                    window.setTimeout(function() {
                        select.options[0].selected = true;
                        self.pendingEstab.academyType = 'Please select';
                    },100);
                    
                },
                detailUrl: function (urn) {
                    return '/Establishments/Establishment/Details/' + urn;
                },
                lookUpType: function(typeId){
                    var definition = academyTypes.filter(function (item) {
                        if (item.id == typeId) {
                            return item;
                        }
                    });
                    return definition[0].name;
            },
            lookupEstabName: function (urn) {
                var self = this;
                return  $.ajax({
                            url: self.urnApiUrl.replace('{0}', urn),
                            method: 'get',
                            dataType: 'json',
                            success: function (data) {
                                return data;
                            },
                            error: function() {
                                return 'Error looking up new establishment name';
                            }
                        });
            },
            validateType: function (estabType) {
                var self = this;
                var typeIds = academyTypes.map(function (type) {
                    return type.id;
                });
                if (typeIds.indexOf(estabType) > -1) {
                    self.estabTypeError = true;
                }

            },
            formatDisplayDate: function (d) {                               
                var date = new Date(d);
                return [this.pad(date.getDate()), this.pad(date.getMonth()+1) , date.getFullYear()].join('/');
            },

            estabLookUp: function () {
                var self = this;
                var estabValidationReqs = [];
                this.pendingEdit = false;
                this.invalidUrn = this.searchUrn === '' || isNaN(this.searchUrn);
                this.duplicateUrn = this.addedUrns.indexOf(self.searchUrn) > -1;
                this.urnLookUpError = '';

                if (this.invalidUrn || this.duplicateUrn) {
                    return true;
                }

                this.isProcessing = true;
                this.isSearching = true;
                function lookUp() {
                    return $.ajax({
                        url: self.urnApiUrl.replace('{0}', self.searchUrn),
                        method: 'get',
                        dataType: 'json',
                        success: function(data) {
                            return data;
                        },
                        error: function (jqxhr) {
                            if (jqxhr.hasOwnProperty('responseJSON')) {
                                self.apiError = jqxhr.responseJSON;
                            } else {
                                self.invalidUrn = true;
                            }
                            
                            self.isProcessing = false;
                            self.isSearching = false;
                        }
                    });
                }

                function checkEstabType(eType) {
                    return $.ajax({
                            url: self.validationUrl,
                            method: 'post',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify([
                                {
                                    PredecessorEstablishmentUrn: self.searchUrn,
                                    OpeningDate: [
                                        today.getFullYear(), self.pad(today.getMonth() + 1), self.pad(today.getDate())
                                    ].join('-'),
                                    TypeId: eType
                                }
                            ]),
                            success: function(data) {
                                self.isProcessing = false;

                            },
                            error: function (jqXHR) {
                                if (jqXHR.hasOwnProperty('responseJSON')) {
                                    self.apiError = jqXHR.responseJSON;
                                } else {
                                    self.urnLookUpError = "The urn in is invalid";
                                }
                                
                                self.isProcessing = false;
                            }
                        });
                }


                $.when(lookUp())
                    .then(
                        function (d1) { // d1 is the data from the estab lookup
                            self.validTypes = [];
                            for (var i = 0, len = academyTypes.length; i <len; i++ ) {
                                estabValidationReqs.push(checkEstabType(academyTypes[i].id));
                            }

                            $.when.apply($, estabValidationReqs).done(function () {
                                $.each(arguments, // the data from the API requests to determine valid est types
                                    function (i, validationResult) {
                                        if (validationResult[0].length === 0) {
                                            self.validTypes.push(academyTypes[i]);
                                        }
                                    });
                                window.setTimeout(function () {
                                    if (self.validTypes.length > 0) {
                                        var estab = d1.returnValue;
                                        self.buildTypesDropDown();
                                        self.pendingEstab = estab;
                                    } else {
                                        self.urnLookUpError = "The urn in is invalid";
                                        self.isSearching = false;
                                    }
                                }, 0);
                            });
                        }
                    );
            },
            validateOpenDate: function () {
                var day = parseInt(this.openDateDay, 10),
                    enteredMonth = parseInt(this.openDateMonth, 10),
                    year = parseInt(this.openDateYear, 10),
                    dateError = false,
                    months31 = [0, 2, 4, 6, 7, 9, 11];

                if (!day
                    || !enteredMonth
                    || !year
                    || isNaN(day)
                    || isNaN(enteredMonth)
                    || isNaN(year)
                    || enteredMonth > 12
                    || enteredMonth < 1
                    || day < 1)
                {
                    dateError = true;
                } else {
                    var monthIndex = enteredMonth - 1;
                    var isLeap = new Date(year, 1, 29).getMonth() === 1;

                    if (isLeap && monthIndex === 1) {
                        if (day > 29) {
                            dateError = true;
                        }
                    } else if (monthIndex === 1) {
                        if (day > 28) {
                            dateError = true;
                        }
                    }

                    if (months31.indexOf(monthIndex) > -1) {
                        if (day < 1 || day > 31) {
                            dateError = true;
                        }
                    } else {
                        if (day < 1 || day > 30) {
                            dateError = true;
                        }
                    }

                    if (monthIndex < 0 || monthIndex > 11) {
                        dateError = true;
                    }
                }
                return dateError;
            },
            addEstablishment: function () {
                var self = this;
                this.openDateError = this.validateOpenDate();
                this.academyUnselectedError = this.pendingEstab.academyType === 'Please select';
                this.urnLookUpError = '';
                this.apiValidationError = '';
                var dateArray = [this.pad(this.openDateDay), this.pad(this.openDateMonth), this.openDateYear];

                if (!this.openDateError && !this.academyUnselectedError) {
                    self.isProcessing = true;

                    $.ajax({
                        url: self.validationUrl,
                        method: 'post',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify([
                            {
                                PredecessorEstablishmentUrn: self.pendingEstab.urn,
                                OpeningDate: [self.openDateYear, self.pad(self.openDateMonth), self.pad(self.openDateDay)].join('-'),
                                TypeId: self.pendingEstab.academyType
                            }
                        ]),
                        success: function(data) {
                            if (data.length === 0) {
                                self.pendingEstab.displayDate = dateArray.join('/');
                                self.pendingEstab.comDate = dateArray.reverse().join('-');
                                self.pendingEstab.typeName = self.lookUpType(self.pendingEstab.academyType);
                                if (self.pendingEdit) {
                                    var estabs = self.establishments.filter(function (estab) {
                                        if (estab.urn !== self.pendingEstab.urn) {
                                            return estab;
                                        }
                                    });
                                    self.establishments = estabs;                                    

                                } else {
                                    self.addedUrns.push(self.searchUrn);

                                }

                                self.establishments.push(self.pendingEstab);
                                self.addedUrns.push(self.searchUrn);

                                self.pendingEstab = {};
                                self.searchUrn = '';
                                self.openDateDay = today.getDate();
                                self.openDateMonth = today.getMonth() + 1;
                                self.openDateYear = today.getFullYear();
                                self.pendingEdit = false;
                                

                            } else {
                                var messages = data[0].errors.map(function(elem) { return elem.message });
                                self.apiValidationError = messages.join('<br>');
                            }
                            self.isProcessing = false;
                            self.isSearching = false;
                        },
                        error: function (jqXHR) {
                            if (jqXHR.hasOwnProperty('responseJSON')) {
                                self.apiError = jqXHR.responseJSON;
                            } else {
                                self.apiValidationError = "Something went wrong checking the establishment.";
                            }
                            
                            self.isProcessing = false;
                            self.isSearching = false;
                        }
                    });
                }

               
            },
            editRecord: function (urn) {
                this.pendingEdit = true;
                this.pendingEstab = this.establishments.filter(function(estab) {
                    if (estab.urn === urn) {
                        return estab;
                    }
                })[0];

                var openingDate = this.pendingEstab.displayDate.split('/');
                this.openDateDay = openingDate[0];
                this.openDateMonth = openingDate[1];
                this.openDateYear = openingDate[2];
            },
            cancelAddEdit: function () {               
                this.pendingEdit = false;
                this.pendingEstab = {};
                this.searchUrn = '';
                this.apiValidationError = '';
            },
            couldDelete: function (urn) {
                this.pendingDelete = true;
                this.pendingDeleteItem = urn;

            },
            confirmDelete: function() {
                var self = this;
                var estabs = this.establishments.filter(function(estab) {
                    if (estab.urn !== self.pendingDeleteItem) {
                        return estab;
                    }
                });
                var urns = this.addedUrns.filter(function(urn) {
                    if (urn != self.pendingDeleteItem) {
                        return urn;
                    }
                });

                this.addedUrns = urns;
                this.establishments = estabs;
                this.pendingDelete = false;
                this.pendingDeleteItem = '';
            },
            blockExits: function () {
                var self = this;                
                $('a, [value="cancel"]').not('.modal-button').on('click', function (e) {
                    self.exitUrl = $(this).attr('href');
                    
                    if (self.establishments.length > 0 && !self.isComplete)  {
                        e.preventDefault();
                        self.displayExitWarning = true;
                    }
                });
            },
            confirmedExit: function() {
                window.location = this.exitUrl;
            },
            commitChanges: function () {
                // hold. on. to. your. butts.
                var self = this;
                var envelope = [];
                var responseId;
                $.each(self.establishments, function (n, estab) {
                    var newAcademy = {};
                    newAcademy.PredecessorEstablishmentUrn = estab.urn;
                    newAcademy.OpeningDate = estab.comDate;
                    newAcademy.TypeId = estab.academyType;
                    envelope.push(newAcademy);
                });

                self.isProcessing = true;
                self.isCommiting = true;
                self.pendingEstab = {};

                $.ajax({ // post all converting academies
                    url: self.commitUrl,
                    dataType: 'json',
                    method: 'post',
                    data:  JSON.stringify(envelope),
                    contentType: 'application/json; charset=utf-8',
                    success: function(data) {
                        responseId = data.response.id;
                        var doneYet = window.setInterval(function() { // poll state of conversions
                            $.ajax({
                                url: self.progressUrl.replace('{0}', responseId),
                                method: 'get',                                
                                dataType: 'json',
                                success: function(data) {
                                    self.createdCount = data.result.length;
                                    if (data.isComplete) {
                                        window.clearInterval(doneYet);
                                        // get the new establishment names from their new URNs
                                        if (data.result.length > 1) {
                                            var requests = [];
                                            for (var i = 0, len = data.result.length; i < len; i++) {
                                                requests.push(self.lookupEstabName(data.result[i].urn));
                                            }

                                            $.when.apply($, requests).done(function() {
                                                $.each(arguments,
                                                    function (i, estab) {
                                                        data.result[i].name = estab[0].returnValue.name;
                                                    });
                                                self.createdAcademies = data.result;
                                                self.isComplete = true;
                                                self.isProcessing = false;
                                            });

                                        } else {
                                            self.createdAcademies = data.result;
                                            $.when(self.lookupEstabName(self.createdAcademies[0].urn)).then(
                                                function (d1) {
                                                    self.createdAcademies[0].name = d1.returnValue.name;
                                                    self.isComplete = true;
                                                    self.isProcessing = false;
                                                });                                            
                                        }                                        
                                    }                                    
                                },
                                error: function(jqXHR, textStatus, errorThrown) {
                                    window.clearInterval(doneYet);
                                    if (jqXHR.hasOwnProperty('responseJSON')) {
                                        self.apiError = jqXHR.responseJSON;
                                    }
                                    self.isProcessing = false;
                                }
                            });
                        }, 1500);

                    },
                    error: function (jqXHR) {
                        var res = JSON.parse(jqXHR.responseText);
                        var errMessage = [];
                        console.log(jqXHR.hasOwnProperty('responseJSON'));
                        for (var i = 0, len = res.validationEnvelope.length; i < len; i++) {
                            var er = res.validationEnvelope[i];
                            errMessage.push(er.urn + ': ');
                            if (er.hasWarnings && !er.hasErrors) {
                                er.warnings.forEach(function(msg) {
                                    errMessage.push(msg.message);
                                });
                            } else if (er.hasErrors) {
                                er.errors.forEach(function(msg) {
                                    errMessage.push(msg.message);
                                });
                            } else {
                                errMessage.push('No specific error message suplied');
                            }                            
                        }
                        self.isProcessing = false;
                       // console.log(errMessage.join('\n'));
                    }
                });
            }
        }

    });

}());

