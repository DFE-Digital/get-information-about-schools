DfE = window.DfE || {};

DfE.mapInteractions = (function() {
  'use strict';

  return {
    initMap: function () {
      var $mapElem = $('#map'),
        coords = $.makeArray($mapElem.data().latlng),
        schoolLat = coords[1],
        schoolsLng = coords[0];

      var openIcon = L.icon({
        iconUrl: '/public/assets/images/maps/open-marker.png',
        iconSize: [28, 41]
      });

      var closeIcon = L.icon({
        iconUrl: '/public/assets/images/maps/closed-marker.png',
        iconSize: [28, 41]
      });


      var plotIcon = window.estabOpen ? openIcon : closeIcon;

      if (!$mapElem.is(':visible')) {
        $(window).on('tabChange', function (e) {
          if (e.tab === '#school-dashboard') {
            $(window).off('tabChange');
            DfE.mapInteractions.initMap();
          }
        });
        return;
      }

      var estabMap = new L.map('map', {
        center: new L.LatLng(schoolLat, schoolsLng),
        zoom: 12,
        attributionControl: false,
        zoomControl: false
      });

      var schoolMarker = L.marker([schoolLat, schoolsLng], {icon: plotIcon}).addTo(estabMap);
      var toolTipContent = $mapElem.data().schoolName + '<br>' + $mapElem.data().schoolAddress.replace(/,/g, '<br>');
      schoolMarker.bindPopup(toolTipContent ,{offset: new L.Point(0, -20)} );

      estabMap.addControl(L.control.attribution({
        prefix: ''
      }));

      var zoomer = new L.control.zoom({
        position: 'bottomright'
      }).addTo(estabMap);

      L.tileLayer('https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key=' + azureMapsApiKey, {
        attribution: '© ' + new Date().getFullYear() + ' Microsoft, © 1992 - ' + new Date().getFullYear() + ' TomTom',
        maxZoom: 18,
        id: 'azuremaps.road',
        crossOrigin: true,
        subscriptionKey: azureMapsApiKey
      }).addTo(estabMap);

    },
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

      groupMap.addControl(L.control.attribution({
        prefix: ''
      }));

      var zoomer = new L.control.zoom({
        position: 'bottomright'
      }).addTo(groupMap);

      L.tileLayer('https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key=' + azureMapsApiKey, {
        attribution: '© ' + new Date().getFullYear() + ' Microsoft, © 1992 - ' + new Date().getFullYear() + ' TomTom',
        maxZoom: window.establishmentsList.length > 1 ? 18 : 16,
        id: 'azuremaps.road',
        crossOrigin: true,
        subscriptionKey: azureMapsApiKey
      }).addTo(groupMap);


      $(window.establishmentsList).each(function (n, elem) {
        var lat = Number(elem.latitude);
        var lon = Number(elem.longitude);
        var iconType = elem.statusName.toLowerCase().indexOf('open') > -1
          ? openIcon
          : closeIcon;

        var marker = new L.marker([lat, lon], {icon: iconType});
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

        marker.bindPopup(tooltipContent.join('<br>'), {offset: new L.Point(0, -20)});

        clusterGroup.addLayer(marker);
      });
      groupMap.addLayer(clusterGroup);
      groupMap.fitBounds(clusterGroup.getBounds(), {padding: [10, 10]});
    }
  }
}());
