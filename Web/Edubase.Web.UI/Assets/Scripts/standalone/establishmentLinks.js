DfE.Views.EstablishmentLinks = {
    randomInt: function(min, max) {
         return Math.floor(Math.random() * (max - min + 1) + min);
    },
    getData: function () {
        var self = this;

        $.getJSON('/public/assets/scripts/JSON/establishmentLinks.js')
            .done(function(data, textStatus) {                
                self.establishmentLinksData = data;
                self.establishments = self.establishmentLinksData.records;
                self.baseEstablishment = self.establishments.splice(self.randomInt(0, self.establishments.length), 1)[0];
                self.linkTypes = self.establishmentLinksData.linkTypes;
                self.linkedEstablishments = [];
                var linkedCount = self.randomInt(5, 12);

                for (var i = 0; i < linkedCount; i++) {
                    self.linkedEstablishments.push(self.establishments[self.randomInt(0, self.establishments.length)]);
                }

                self.linkedEstablishments[1].backLink = true;

                var overlay = $('<div id="demo-establishment-list" class="hidden"><ul></ul></div>'),
                    schoolsContent = [];


                $(self.establishments).each(function (n, school) {
                    schoolsContent.push('<li>' + school.urn + ": " + school.name + '</li>');
                });

                overlay.find('ul').html(schoolsContent.join(''));

                $('body').append(overlay);
                self.overlay = overlay;
                window.setTimeout(function() {
                    DfE.Views.EstablishmentLinks.init();
                }, 1500);
                
            })
            .fail(function(jqxhr, setting, ex) {
                console.log(ex);
            });
    },
    init: function () {
        var self = this;
        
        var establishmentLinks = new Vue({
            el: '#establishment-links',
            data: {
                baseEstab: self.baseEstablishment,
                linkedEstablishments: self.linkedEstablishments,
                availableEstablishments: self.establishments,
                linkTypes: self.linkTypes,
                urnSearchError: false,
                urnInvalidError: false,
                linkingEstab: '',
                viewMode: true,
                editMode: false,
                addMode: false,
                editEstab: {},
                reverseLink: false,
                userUrn: '',
                forwardLinkType: null,
                reverseLinkType: null,
                cloneDate: false,
                linkDateDay: '',
                linkDateMonth: '',
                linkDateYear: '',
                reverseLinkDateDay: '',
                reverseLinkDateMonth: '',
                reverseLinkDateYear: '',
                linkDateError: false,
                forwardLinkError: false,
                backLinkError: false,
                reverseLinkDateError: false
            },
            methods: {
                addNewLink: function () {
                    this.viewMode = false;
                    this.addMode = true;
                },
                editLink: function (urn) {
                    var self = this;
                    this.viewMode = false;
                    this.editMode = true;
                    this.userUrn = urn;
                    this.findEstablishment();
                    this.linkedEstablishments.filter(function (estab) {
                        if (estab.urn === urn) {
                            var dateParts = estab.linkDate.split('/');

                            self.linkDateDay = dateParts[0];
                            self.linkDateMonth = dateParts[1];
                            self.linkDateYear = dateParts[2];

                            self.forwardLinkType = estab.linkType.code;
                        }
                    });

                    
                },
                validateUrn: function () {
                    var linkedUrns = this.linkedEstablishments.map(function(estab) {
                        return estab.urn;
                    });
                    this.urnInvalidError = false;
                    this.urnSearchError = false;
                    if (!$.isNumeric(this.userUrn)) {
                        this.urnInvalidError = true;
                        return;
                    } 
                    if (linkedUrns.indexOf(this.userUrn) >-1) {
                        this.urnSearchError = true;
                    }

                    if (!this.urnInvalidError && !this.urnSearchError) {
                        this.findEstablishment();
                    }
                    
                },
                findEstablishment: function () {
                    var urn = this.userUrn;
                    var self = this;
                    var found = false;

                    this.availableEstablishments.filter(function (estab) {
                        if (estab.urn === urn) {
                            self.linkingEstab = estab;
                            found = true;
                        }
                    });

                    if (!found) {
                        this.urnInvalidError = true;
                        return;
                    }
                    this.updateModalMessaging();
                },
                toggleDateClone: function() {
                    if (this.cloneDate) {
                        this.reverseLinkDateDay = this.linkDateDay;
                        this.reverseLinkDateMonth = this.linkDateMonth;
                        this.reverseLinkDateYear = this.linkDateYear;
                    }
                },
                validateEstablishmentLink: function () {
                    var self = this;

                    this.forwardLinkError = false;
                    this.linkDateError = false;
                    this.backLinkError = false;
                    this.reverselinkDateError = false;

                    function validDate(d, m, y, prop) {
                        var isLeap = new Date(y, 1, 29).getMonth() === 1;
                        console.log('isLeap == ' + isLeap);
                       if (isNaN(d) ||
                           isNaN(m) ||
                           isNaN(y) ||
                           d === '' ||
                           m === '' ||
                           y === '' ||
                           d < 1 ||
                           d > 31 ||
                           m < 1 ||
                           m > 12 ||
                           y < 1000 ||
                           y > 3000) {
                                self[prop] = true;
                       }

                        if (isLeap && m === 2 && d > 29) {
                            self[prop] = true;
                        }
                        if (!isLeap && m === 2 && d > 28) {
                            self[prop] = true;
                        }

                    }

                    if (!this.forwardLinkType) {
                        this.forwardLinkError = true;
                    }

                   
                    validDate(Number(this.linkDateDay), Number(this.linkDateMonth), Number(this.linkDateYear), 'linkDateError');

                    if (this.reverseLink) {
                        if (!this.reverseLinkType) {
                            this.backLinkError = true;
                        }

                        validDate(Number(this.reverseLinkDateDay), Number(this.reverseLinkDateMonth), Number(this.reverseLinkDateYear), 'reverseLinkDateError');
                    }

                    if (!this.forwardLinkError &&
                        !this.linkDateError &&
                        !this.backLinkError &&
                        !this.reverselinkDateError) {
                        this.saveLinkedEstablishment();
                    }
                    
                },
                saveLinkedEstablishment: function () {
                    var est = this.linkingEstab;
                    var self = this;
                    var selectedLinkType = self.linkTypes.filter(function (lt) {
                        if (lt.code === self.forwardLinkType) {
                            return lt;
                        }
                    });
                    est.linkType = selectedLinkType[0];
                    est.linkDate = [this.linkDateDay, this.linkDateMonth, this.linkDateYear].join('/');
                    est.backLink = this.reverseLink;
                    if (this.addMode) {
                        this.linkedEstablishments.push(est);
                    } else {
                        var index = self.linkedEstablishments.map(function(estab) {
                            return estab.urn;
                        }).indexOf(est.urn);
                        self.linkedEstablishments[index] = est;
                    }
                    
                    this.reset();
                },
                reset: function() {
                    this.urnSearchError = false;
                    this.urnInvalidError = false;
                    this.linkingEstab = '';
                    this.viewMode = true;
                    this.editMode = false;
                    this.addMode=false;
                    this.editEstab = {};
                    this.reverseLink = false;
                    this.userUrn = '';
                    this.forwardLinkType = null;
                    this.reverseLinkType = null;
                    this.cloneDate = false;
                    this.linkDateDay = '';
                    this.linkDateMonth = '';
                    this.linkDateYear = '';
                    this.reverseLinkDateDay = '';
                    this.reverseLinkDateMonth = '';
                    this.reverseLinkDateYear = '';
                    this.linkDateError = false;
                    this.forwardLinkError = false;
                    this.backLinkError = false;
                    this.reverselinkDateError = false;
                },
                updateModalMessaging: function () {
                    var mTitle = "Are you sure you want to delete this link?";
                    var mContent = "This will remove the link entry on this page";
                    
                    if (this.linkingEstab.backLink) {
                        mContent = "This will also remove the link entry on the linked establishments page";
                    }

                    $('#content').find('.estab-link-delete').data().okCancel.updateModalContent(mTitle, mContent);
                }
            },
            computed: {
                linkingEstabAddress: function () {
                    var estab = this.linkingEstab;
                    var out = [];
                    if (estab.street !== '') {
                        out.push(estab.street);
                    }
                    if (estab.locality !== '') {
                        out.push(estab.locality);
                    }
                    if (estab.address3 !== '') {
                        out.push(estab.address3);
                    }
                    if (estab.town !== '') {
                        out.push(estab.town);
                    }
                    if (estab.postcode !== '') {
                        out.push(estab.postcode);
                    }

                    return out.join(', ');
                }
            }
        });

        $('#content').find('.estab-link-delete').okCancel({
            triggerEvent: 'click',
            title: 'Are you sure you want to delete this link?',
            content: 'This will remove the link entry on this page',
            ok: function () {
                var linkedEstabUrn = establishmentLinks.linkingEstab.urn;
                var updatedEstabList = establishmentLinks.linkedEstablishments.filter(function (estab) {
                    if (estab.urn !== linkedEstabUrn) {
                        return estab;
                    }
                });
                establishmentLinks.linkedEstablishments = updatedEstabList;
                establishmentLinks.reset();
            }
        });

        $('#page-heading').on('click', function() {
            self.overlay.toggleClass('hidden');
        });
    }
}


DfE.Views.EstablishmentLinks.getData();