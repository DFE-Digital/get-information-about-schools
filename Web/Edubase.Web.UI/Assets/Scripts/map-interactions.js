DfE = window.DfE || {};

DfE.mapInteractions = (function () {
    'use strict';
   
     return {
        loadGoogleScript: function (initFunc) {
             if (!initFunc) initFunc = "DfE.mapInteractions.initMap";
             $.getScript("https://maps.googleapis.com/maps/api/js?key=" + DfE.mapConfig.apiKey + "&callback=" + initFunc);
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
         },
        // differs from estab map in that it is using leaflet - where as the single estab map is plain ol' google maps
        initGroupMap: function () {
            'use strict';
            if (!window.hasOwnProperty('establishmentsList')) {
                return;
            }

            var openIcon = L.icon({
                iconUrl: '/public/assets/images/maps/open-marker.png',
                iconSize: [28, 41]
            });

            var closeIcon = L.icon({
                iconUrl: '/public/assets/images/maps/closed-marker.png',
                iconSize: [28, 41]
            });

            var groupMap = new L.map('group-map',
                {
                    center: new L.LatLng(54, -3),
                    zoom: 6,
                    attributionControl: false,
                    zoomControl: false,
                    spiderfyOnMaxZoom: false
                });

            window.groupMap = groupMap;

            var clusterGroup = new L.MarkerClusterGroup();

            var zoomer = new L.control.zoom({
                position: 'bottomright'
            }).addTo(groupMap);

            var googleTiles = new L.Google('ROADMAP');
            var satelliteTiles = new L.Google('SATELLITE');

            L.control.layers({
                'Map': googleTiles,
                'Satellite': satelliteTiles
            }, null, { position: 'topleft', collapsed: false }).addTo(groupMap);
            
            groupMap.addLayer(googleTiles);

            $(window.establishmentsList).each(function (n, elem) {
                var lat = Number(elem.latitude);
                var lon = Number(elem.longitude);
                var iconType = elem.statusName.toLowerCase().indexOf('open') > -1
                    ? openIcon
                    : closeIcon;

                var marker = new L.marker([lat, lon], { icon: iconType });
                var tooltipContent = [
                    '<a href="/Establishments/Establishment/Details/' +
                    elem.urn +
                    '">' +
                    elem.name +
                    '</a>',
                    elem.address, 'School type: ' + elem.typeName,
                    'URN: ' + elem.urn,
                    'Status: ' + elem.statusName
                ];

                marker.bindPopup(tooltipContent.join('<br>'), { offset: new L.Point(0, -20) });

                clusterGroup.addLayer(marker);
            });
            groupMap.addLayer(clusterGroup);
            groupMap.fitBounds(clusterGroup.getBounds());
        }
     }    
}());
