DfE = window.DfE || {};

DfE.mapInteractions =  (function () {
    'use strict';
    var settings = {
        apikey: 'AIzaSyA-eSFOhFG0RnFzwhgFf4fSyQl_ypklraE' // registered to Jon B, resticted to localhost / dev / stage for Edubase
    }

     return {
         loadGoogleScript: function () {
            $.getScript("https://maps.googleapis.com/maps/api/js?key=" + settings.apikey + "&callback=DfE.mapInteractions.initMap");
        },
        
        initMap: function() {
            var $mapElem = $('#map'),
                coords = $.makeArray($mapElem.data().latlng),
                schoolLat = coords[1],
                schoolsLng = coords[0],
                mapCentre = { lat: schoolLat, lng: schoolsLng };

            if (!$mapElem.is(':visible')) { return }

            var map = new google.maps.Map(document.getElementById('map'), {
                center: mapCentre,
                zoom: 12
            });

            var schoolInfo = new google.maps.InfoWindow({
                content: $mapElem.data().schoolName + '<br>' + $mapElem.data().schoolAddress.replace(/,/g, '<br>')
            });

            var marker = new google.maps.Marker({
                position: mapCentre,
                map: map
            });

            marker.addListener('click', function() {
                schoolInfo.open(map, marker);
            });
        }
     }    
}());