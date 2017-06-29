(function () {

    var bulkAcademies = new Vue({
        el: '#bulk-academies',
        data: {
            establishments: [],
            addedUrns: [],
            searchUrn: '',
            establishmentType: '',
            invalidUrn: false,
            duplicateUrn: false,

            estabTypeError: false,

            pendingEstab : {},
            pendingEdit: false,
            establishmentFound: false,
            openDateDay: '',
            openDateMonth: '',
            openDateYear: '',
            openDateError: false,
            academyType: '',
            academyUnselectedError: false,
            dateError: false,
            noAcademyError: false,
            commitError: false,
            commitErrorMessage: '',
            displayExitWarning: false,
            urnApiUrl: '/api/establishment/{0}',
            validationUrl: '/api/bulk-create-academies/validate',
            commitUrl: '/api/bulk-create-academies',
            progressUrl: '/api/bulk-create-academies/progress/{0}',
            commitErrors: '',

            isProcessing: false,
            isComplete: false,
            pendingDelete: false,
            pendingDeleteItem: ''


        },
        created: function() {
            var frag = document.createDocumentFragment();
            for (var i = 0, len = academyTypes.length; i < len; i++) {
                var opt = document.createElement('option');
                var t = types[i];
                opt.value = t.id;
                opt.innerHTML = t.name;
                frag.appendChild(opt);
            }
            document.getElementById('academy-type').appendChild(frag);
        },
        computed: {
            showGlobalError: function () {
                return (
                    this.estabTypeError ||
                        this.duplicateUrn ||
                        this.noAcademyTypeError ||
                        this.academyUnselectedError ||
                        this.openDateError ||
                        this.invalidUrn ||
                        this.commitError
                );
            },
            pageHeading: function() {
                if (this.pendingEdit) {
                    return 'Edit details';
                }
                if (Object.keys(this.pendingEstab).length > 0) {
                    return 'Enter new academy details';
                }
                if (this.isProcessing) {
                    return 'Creating academies';
                }
                if (this.isComplete) {
                    return 'Academies created';
                }

                return 'Bulk create new academies';

            },
            displayDate: function () {
                return [this.openDateDay, this.openDateMonth, this.openDateYear].join('/');
            },
            estabTypeName: function () {
                if (true) {
                    return true;
                }
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
            
            lookUpType: function (typeId) {
                var definition = types.filter(function (item) {
                    if (item.id === typeId) {
                        return item;
                    }
                });
                return definition[0].name;
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
                return [date.getDate(), date.getMonth()+1 , date.getFullYear()].join('/');
            },
            estabLookUp: function () {
                var self = this;
                this.pendingEdit = false;
                this.invalidUrn = this.searchUrn === '' || isNaN(this.searchUrn);
                this.duplicateUrn = this.addedUrns.indexOf(self.searchUrn) > -1;

                if (this.invalidUrn || this.duplicateUrn) {
                    return true;
                }

                $.ajax({
                    url: self.urnApiUrl.replace('{0}', self.searchUrn),
                    method: 'get',
                    dataType: 'json',
                    success: function (data) {
                        console.log(data);
                        var estab = data.returnValue;
                        self.validateType(estab.typeId);
                        if (!self.estabTypeError) {
                            estab.typeName = self.lookUpType(estab.typeId);                            
                            estab.academyType = 28; // acad convertor as default
                            self.pendingEstab = estab;                            
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
            },
            validateOpenDate: function () {
                var day = parseInt(this.openDateDay, 10),
                    month = parseInt(this.openDateMonth, 10) - 1,
                    year = parseInt(this.openDateYear, 10),

                    dateError = false,
                    months31 = [0, 2, 4, 6, 7, 9, 11],
                    currentYear = new Date().getFullYear();

                if (!day || !month || !year || isNaN(day) || isNaN(month) || isNaN(year)) {
                    dateError = true;
                }

                var isLeap = new Date(year, 1, 29).getMonth() === 1; 

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
            addEstablishment: function () {
                var self = this;
                this.openDateError = this.validateOpenDate();
                var dateArray = [this.openDateDay, this.openDateMonth, this.openDateYear];

                if (!this.openDateError) {
                    this.pendingEstab.displayDate = dateArray.join('/');
                    this.pendingEstab.comDate = dateArray.reverse().join('-');
                    
                    if (this.pendingEdit) {
                        var estabs = this.establishments.filter(function(estab) {
                            if (estab.urn !== self.pendingEstab.urn) {
                                return estab;
                            }
                        });
                        this.establishments = estabs;

                    } else {
                        this.addedUrns.push(this.searchUrn);

                    }
                    
                    this.establishments.push(this.pendingEstab);
                    this.addedUrns.push(this.searchUrn);

                    this.pendingEstab = {};
                    this.searchUrn = '';
                    this.openDateDay = '';
                    this.openDateMonth = '';
                    this.openDateYear = '';
                    
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
                this.establishments = estabs;
                this.pendingDelete = false;
                this.pendingDeleteItem = '';
            },           
            commitChanges: function () {
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
                console.log(JSON.stringify(envelope));

                $.ajax({
                    url: self.commitUrl,
                    dataType: 'json',
                    method: 'post',
                    data:  JSON.stringify(envelope),
                    contentType: 'application/json; charset=utf-8',
                    success: function(data) {
                        console.log(data);
                        responseId = data.response.id;

                        var doneYet = window.setInterval(function() {
                            $.ajax({
                                url: self.progressUrl.replace('{0}', responseId),
                                method: 'get',
                                //data: { id: responseId },
                               // contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function(data) {
                                    console.log(data);
                                    window.clearInterval(doneYet);
                                },
                                error: function(jqXHR, textStatus, errorThrown) {
                                    console.log(jqXHR);
                                    console.log(textStatus);
                                    console.log(errorThrown);
                                    window.clearInterval(doneYet);
                                }
                            });
                        },1000);

                    },
                    error: function (jqXHR) {
                        //console.log(jqXHR);
                        var res = JSON.parse(jqXHR.responseText);
                        var errMessage = [];
                        console.log(jqXHR.responseText);

                        console.log(res.length);

                        for (var i = 0, len = res.validationEnvelope.length; i < len; i++) {
                            var er = res.validationEnvelope[i];
                            console.log(er);
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

                        //for (var item in res) {
                        //    console.log(item);
                        //    if (item === 'validationEnvelope') {
                        //        var env = res[item][0].errors;
                        //        var estab = res[item][0].urn;
                        //        errMessage.push(res[item][0].urn + ': ');
                        //        env.forEach(function (er) {
                        //            //console.log(errObj[item][0].errors);
                        //            //console.log(er);
                        //            errMessage.push(er.message);
                        //        });
                        //    }
                        //}
                        console.log(errMessage.join('\n'));

                        // var p = JSON.parse(jqXHR).responseText;
                        //console.log(p);
                    }
                });
            }
        }

    });

}());

