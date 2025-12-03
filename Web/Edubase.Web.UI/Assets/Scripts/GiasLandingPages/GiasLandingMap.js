import 'leaflet/dist/leaflet';
const L = window['L'];
import 'leaflet.markercluster/dist/leaflet.markercluster';

class GiasLandingMap {
  constructor() {
    this.isEstab = typeof window.mapMode !== 'undefined' && window.mapMode === 'establishment';
    this.init();
  }

  init() {
    const self = this;
    const $mapElem = this.isEstab ? $('#map') : $('#group-map');

    const openIcon = L.icon({
      iconUrl: '/public/assets/images/maps/open-marker.png',
      iconSize: [28, 41]
    });

    const closeIcon = L.icon({
      iconUrl: '/public/assets/images/maps/closed-marker.png',
      iconSize: [28, 41]
    });

    let coords;
    let schoolLat;
    let schoolsLng;
    let plotIcon;

    let mapId = this.isEstab ? 'map' : 'group-map';

    if (this.isEstab) {
      coords = $.makeArray($mapElem.data().latlng);
      schoolLat = coords[1];
      schoolsLng = coords[0];
      plotIcon = window.estabOpen ? openIcon : closeIcon;
    }
    const mapCenter = this.isEstab ? new L.LatLng(schoolLat, schoolsLng) : new L.LatLng(54, -3);
    const initialZoom = this.isEstab ? 12 : 6;
    const clusterGroup = new L.MarkerClusterGroup();

    if (this.isEstab) {
      if (!$mapElem.is(':visible')) {
        $(window).on('tabChange', function (e) {
          if (e.tab === '#school-dashboard') {
            $(window).off('tabChange');
            self.init();
          }
        });
        return;
      }
    } else {
      if (!$mapElem.is(':visible') && !this.isEstab) {
        $(window).on('tabChange', function (e) {
          if (e.tab === '#list') {
            $(window).off('tabChange');
            self.init();
          }
        });
        return;
      }
    }

    let estabMap;

    if (this.isEstab) {
      estabMap = new L.map(mapId, {
        center: mapCenter,
        zoom: initialZoom,
        attributionControl: false,
        zoomControl: false,
      });
      const schoolMarker = L.marker([schoolLat, schoolsLng], {icon: plotIcon}).addTo(estabMap);
      const toolTipContent = $mapElem.data().schoolName + '<br>' + $mapElem.data().schoolAddress.replace(/,/g, '<br>');
      schoolMarker.bindPopup(toolTipContent, {offset: new L.Point(0, -20)});

    } else {
      estabMap = new L.map(mapId,
        {
          center: new L.LatLng(54, -3),
          zoom: 6,
          attributionControl: false,
          zoomControl: false,
          spiderfyOnMaxZoom: false
        });

      $(window.establishmentsList).each(function (n, elem) {
        const lat = Number(elem.latitude);
        const lon = Number(elem.longitude);
        const iconType = elem.statusName.toLowerCase().indexOf('open') > -1
          ? openIcon
          : closeIcon;

        const marker = new L.marker([lat, lon], {icon: iconType});
        const tooltipContent = [
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

    }

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

    if (!this.isEstab) {
      estabMap.addLayer(clusterGroup);
      estabMap.fitBounds(clusterGroup.getBounds(), {padding: [10, 10]});
    }
  }
}

export default GiasLandingMap;
