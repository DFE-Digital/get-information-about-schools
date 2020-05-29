import 'leaflet/dist/leaflet';
const L = window['L'];
import 'leaflet.markercluster/dist/leaflet.markercluster';

class GiasLandingMap {
  constructor() {
    this.init();
  }

  init() {
    const self = this;
    const $mapElem = $('#map'),
      coords = $.makeArray($mapElem.data().latlng),
      schoolLat = coords[1],
      schoolsLng = coords[0];

    const openIcon = L.icon({
      iconUrl: '/public/assets/images/maps/open-marker.png',
      iconSize: [28, 41]
    });

    const closeIcon = L.icon({
      iconUrl: '/public/assets/images/maps/closed-marker.png',
      iconSize: [28, 41]
    });


    const plotIcon = window.estabOpen ? openIcon : closeIcon;

    if (!$mapElem.is(':visible')) {
      $(window).on('tabChange', function (e) {
        if (e.tab === '#school-dashboard') {
          $(window).off('tabChange');
          self.init();
        }
      });
      return;
    }

    const estabMap = new L.map('map', {
      center: new L.LatLng(schoolLat, schoolsLng),
      zoom: 12,
      attributionControl: false,
      zoomControl: false
    });

    const schoolMarker = L.marker([schoolLat, schoolsLng], {icon: plotIcon}).addTo(estabMap);
    const toolTipContent = $mapElem.data().schoolName + '<br>' + $mapElem.data().schoolAddress.replace(/,/g, '<br>');
    schoolMarker.bindPopup(toolTipContent, {offset: new L.Point(0, -20)});

    estabMap.addControl(L.control.attribution({
      prefix: ''
    }));

    const zoomer = new L.control.zoom({
      position: 'bottomright'
    }).addTo(estabMap);

    L.tileLayer('https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key=' + window.azureMapsApiKey, {
      attribution: '© ' + new Date().getFullYear() + ' Microsoft, © 1992 - ' + new Date().getFullYear() + ' TomTom',
      maxZoom: 18,
      minZoom: 4,
      id: 'azuremaps.road',
      crossOrigin: true,
      subscriptionKey: azureMapsApiKey
    }).addTo(estabMap);

  }
}

export default GiasLandingMap;
