DfE = window.DfE || {};

DfE.mapInteractions =  (function () {
    'use strict';
   
     return {
         loadGoogleScript: function () {
             $.getScript("https://maps.googleapis.com/maps/api/js?key=" + DfE.mapConfig.apiKey + "&callback=DfE.mapInteractions.initMap");
        },
        
        initMap: function() {
            var $mapElem = $('#map'),
                coords = $.makeArray($mapElem.data().latlng),
                schoolLat = coords[1],
                schoolsLng = coords[0],
                mapCentre = { lat: schoolLat, lng: schoolsLng },
                iconUrl = window.estabOpen
                    ? '/public/assets/images/maps/google-open-marker.png'
                    : '/public/assets/images/maps/google-closed-marker.png';
               

            if (!$mapElem.is(':visible')) {
                $(window).on('tabChange', function (e) {
                    if (e.tab === '#school-dashboard') {
                        $(window).off('tabChange');
                        DfE.mapInteractions.initMap();
                    }
                });
                return;
            }

            var map = new google.maps.Map(document.getElementById('map'), {
                center: mapCentre,
                zoom: 12
            });
            
            var marker = new google.maps.Marker({
               icon: iconUrl,
               position: mapCentre,
               map: map
            });

            var schoolInfo = new google.maps.InfoWindow({
                content: $mapElem.data().schoolName + '<br>' + $mapElem.data().schoolAddress.replace(/,/g, '<br>')
            });


            marker.addListener('click', function() {
                schoolInfo.open(map, marker);
            });
        }
     }    
}());