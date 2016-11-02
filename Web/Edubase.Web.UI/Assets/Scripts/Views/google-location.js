(function (GOVUK, Views) {
    'use strict';

    function GoogleLocationViewModel() {
        var findByTown = /** @type {!HTMLInputElement} */(document.getElementById('LocationSearchModel_Text'));
        var options = {
            types: ['geocode'],
            componentRestrictions: { country: 'GB' }
        };
        this.bindEvents(new google.maps.places.Autocomplete(findByTown, options));
    }

    
    GoogleLocationViewModel.prototype = {

        autocomplete: {},

        appendSearchType: function() {
            // Add a new hidden field to ensure that we get the searchType
            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'searchtype';
            input.value = 'search-by-location';

            document.forms[1].appendChild(input);
        },

        bindEvents: function (autocomplete) {
            this.autocomplete = autocomplete;
            var self = this;
            autocomplete.addListener('place_changed', this.placeChangedHandler.bind(this));
            $("form").submit(function (e) {
                if ($("input#TownOrCity").attr("checked") === "checked" && $("input#LocationSearchModel_Text").val().length >= 2) {
                    self.appendSearchType();
                    $('#LocationSearchModel_AutoSuggestValue').val("");
                    setTimeout(function () { self.submitLocation(self); }, 100);
                    e.preventDefault();
                    return false;
                }
            });
        },

        submitLocation: function (self) {
            var place = self.autocomplete.getPlace();
            if (place && place.geometry) {
                self.setLatLng(place);
            }

            // Submit the form
            document.forms[1].submit();
        },

        setLatLng: function (place) {
            if (place.geometry != null) {
                var location = place.geometry.location;
                $('#LocationCoordinates').val(location.lat() + ',' + location.lng());
            }
        },

        placeChangedHandler: function () {
            var place = this.autocomplete.getPlace();
            this.setLatLng(place);
        }
    };

    GoogleLocationViewModel.Load = function () {
        new DfE.Views.GoogleLocationViewModel()
    };

    Views.GoogleLocationViewModel = GoogleLocationViewModel;

}(GOVUK, DfE.Views));