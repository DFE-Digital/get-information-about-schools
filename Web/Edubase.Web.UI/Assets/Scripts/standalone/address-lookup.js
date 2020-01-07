var postcodeLookUp = new Vue({
    el: '#postcode-lookup',
    data: {
        postcodeUrl: '/public/assets/scripts/JSON/postcodeResults.js',
        addressUrl: '/public/assets/scripts/JSON/addressResult.js',
        postcode: '',
        addressLineOne: '',
        addressLineTwo: '',
        addressLineThree: '',
        town: '',
        county: '',
        country: '',
        selectedCountry: 'UK',
        postcodeError: false,
        ajaxError: false,
        addressLookupError: false,
        step: 0,
        isUk: true
    },
    methods: {
        incrementStep: function() {
            this.step += 1;
        },
        allDone: function() {
            window.history.back();
        },
        resetSearch: function() {
            this.ajaxError = false;
            this.postcodeError = false;
            this.postcode = '';
            this.addressLineOne = '';
            this.addressLineTwo = '';
            this.addressLineThree = '';
            this.town = '';
            this.county = '';
            this.country = '';
            this.selectAddress = 0;

            this.step = 0;
        },
        switchCountry: function() {
            this.isUk = this.selectedCountry === 'UK';
        },
        validatePostcode: function() {
            var postCode = this.postcode;
            var postCodeUrl = this.postcodeUrl;
            var self = this;

            if (postCode.length > 0) { // TODO replace with more robust validation - backlog item #38716
                self.postcodeError = false;

                $('#address-results').find('option:gt(0)').remove();

                $.getScript(postCodeUrl)
                    .done(function (data) {
                        
                        var addressOptions = document.createDocumentFragment();
                        var results = JSON.parse(data);

                        for (var i = 0, len = results[0].matches.length; i < len; i++) {
                            var match = results[0].matches[i];
                            var opt = document.createElement('option');
                            opt.value = match.id;
                            opt.innerHTML = match.name;

                            addressOptions.appendChild(opt);
                        }

                        document.getElementById('address-results').appendChild(addressOptions);

                        if (self.step === 0) {
                            self.incrementStep();
                        }

                    })
                    .fail(function() {
                        self.ajaxError = true;
                    });
            } else {
                self.postcodeError = true;
            }
        },
        selectAddress: function() {
            var lookupUrl = this.addressUrl;
            var self = this;
            $.getScript(lookupUrl)
                .done(function(data) {
                    var result = JSON.parse(data)[0];

                    self.addressLineOne = result.addressLineOne;
                    self.addressLineTwo = result.addressLineTwo;
                    self.addressLineThree = result.addressLineThree;
                    self.town = result.town;
                    self.county = result.county;

                    self.incrementStep();
                })
                .fail(function() {
                    self.addressLookupError = true;
                });
        }
    },
    computed: {
        globalError: function() {
            return (this.ajaxError || this.postcodeError);
        }
    }
});
