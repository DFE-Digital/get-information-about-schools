import 'leaflet/dist/leaflet';
const L = window['L'];
import 'leaflet.markercluster/dist/leaflet.markercluster';

class GiasSearchMap {
  constructor() {
    this.$filterForm = $('#filter-form');
    this.$resultsNotification = $('#results-notification');
    this.config = {
      scriptsLoaded: false,
      softLimit: 900,
      step: 100,
      breachLimit: false,
      dataRefreshRequired: false,
      azureApiKey: window.azureMapsApiKey,
      establishmentDetailBaseUrl: '/Establishments/Establishment/Details/',
      openIcon: L.icon({
        iconUrl: '/public/assets/images/maps/open-marker.png',
        iconSize: [28, 41]
      }),
      closeIcon: L.icon({
        iconUrl: '/public/assets/images/maps/closed-marker.png',
        iconSize: [28, 41]
      }),
      currentView: 'list',
    }

    this.setSearchParams();
  }

  setUp() {
    $('#results-container').addClass('hidden');
    $('#map-container').removeClass('hidden');

    if (!this.config.scriptsLoaded) {
      this.initMap();
      this.config.scriptsLoaded = true;

    } else {
      this.getSearchData('setUp');
    }
  }

  setSearchParams() {
    function deDupeParams(qs) {
      const paramArray = qs.split('&');
      return paramArray.sort().filter(function (item, pos, ary) {
        return !pos || item !== ary[pos - 1];
      }).join('&');
    }

    this.searchParams = deDupeParams($('#filter-form').find(':input').filter(function (n, ele) {
      return ele.value !== '';
    }).serialize());
  }

  getSearchData() {
    const self = this;
    let resultCount = 0;
    let startIndex = 0;
    let pointCount = 0;
    this.clusterGroup = new L.MarkerClusterGroup();
    this.breachLimit = false;
    $('#map-count').text('0');

    getPoints();

    function getPoints() {
      self.isLoadingPoints = true;
      function pointsLoaded() {
        self.isLoadingPoints = false;
        window.enableFilters();
        self.clusterGroup.setZIndex(10);
        if (!self.mapObj.hasLayer(self.clusterGroup)) {
          self.mapObj.addLayer(self.clusterGroup);
        }

        self.mapObj.fitBounds(self.clusterGroup.getBounds());
        window.setTimeout(function () {
          $('.map-header').removeClass('loading');
          const count = $('#map-count').text();
          self.$resultsNotification.text('Search results loaded. Showing ' + count + ' establishments in map view.');
        }, 1500);
      }

      $('.map-header').addClass('loading');
      $('#map-data-warning').addClass('hidden');
      $('#zero-results-message').addClass('hidden');

      $.ajax({
        url: '/Establishments/Search/results-json',
        data: self.searchParams + '&startIndex=' + startIndex,
        dataType: 'JSON',
        success: function (data, status, xhr) {
          if (Number(xhr.getResponseHeader("x-count")) > 0) {
            resultCount = Number(xhr.getResponseHeader("x-count"));
          }
          startIndex += self.config.step;
          $(data).each(function (n, elem) {
            if (elem.location) {
              const lat = Number(elem.location.latitude);
              const lon = Number(elem.location.longitude);
              const iconType = elem.status.toLowerCase().indexOf('open') > -1
                ? self.config.openIcon
                : self.config.closeIcon;

              const marker = new L.marker([lat, lon], {icon: iconType});
              const tooltipContent = [
                '<a href="' +
                self.config.establishmentDetailBaseUrl +
                elem.urn +
                '">' +
                elem.name +
                '</a>',
                elem.address, 'Phase/type: ' + elem.phaseType,
                'URN: ' + elem.urn + '  LAESTAB: ' + elem.laestab,
                'Status: ' + elem.status
              ];

              marker.bindPopup(tooltipContent.join('<br>'), {offset: new L.Point(0, -20)});

              self.clusterGroup.addLayer(marker);
              pointCount++;
            }

          });
          const countDisplayed = Number($('#map-count').text());
          if (pointCount > countDisplayed) {
            $('#map-count').text(pointCount);
          }

          if (pointCount === 0) {
            $('#zero-results-message').removeClass('hidden');

            $('.map-header').removeClass('loading');
            window.enableFilters();
            return;
          }

          if (startIndex <= resultCount) {
            if (pointCount > self.config.softLimit && !self.config.breachLimit) {
              pointsLoaded();
              $('#map-data-warning').removeClass('hidden');
              $('#map-count').text('1000');

              $('#show-all-map').one('click',
                function (e) {
                  e.preventDefault();
                  self.config.breachLimit = true;
                  startIndex += self.config.step;
                  $('#map-data-warning').addClass('hidden');
                  getPoints();
                });

            } else {
              getPoints();
            }

          } else {
            pointsLoaded();
          }

        },
        error: function (xhr) {
          console.log(xhr);
        }
      });
    }
  }

  clearPoints() {
    this.mapObj.removeLayer(this.clusterGroup);
  }

  bindActions() {
    const self = this;
    const $main = $('#main-content');
    $main.on('click', '#view-list', (e) => {
      e.preventDefault();
      $('#map-container').addClass('hidden');
      $('#results-container').removeClass('hidden');
      $('#option-select-sort-by').removeClass('hidden');
      const count = $('#list-count').text();
      this.$resultsNotification.text('Search results loaded. Showing ' + count + ' establishments in list view.');
      this.config.currentView = 'list';
    });

    $main.on('click', '#view-map', (e) => {
      e.preventDefault();
      if (!this.config.scriptsLoaded) {
        this.setUp();
      } else {
        $('#results-container').addClass('hidden');
        $('#map-container').removeClass('hidden');
        if (this.config.dataRefreshRequired) {
          this.clearPoints();
          this.getSearchData('view map');
          this.config.dataRefreshRequired = false;
        }
      }

      $('#option-select-sort-by').addClass('hidden');
      this.config.currentView = 'map';
    });

    $(document).on('change', '.trigger-result-update', function () {
      self.setSearchParams();
    });

    this.$filterForm.find('.filter-button').on('click', function () {
      window.enableFilters();
      self.setSearchParams();
      window.disableFilters();
    });

  }

  refreshMap() {
    // called by other scripts to notify map to update data either now or at next view switch
    if (this.config.currentView === 'map') {
      this.clearPoints();
      this.getSearchData('map refresh');
      if (this.isLoadingPoints) {
        window.disableFilters();
      }
    } else {
      this.config.dataRefreshRequired = true;
    }
  }

  initMap() {
    const self = this;
    self.currentView = 'map';
    const searchMap = new L.map('search-map',
      {
        center: new L.LatLng(54, -3),
        zoom: 6,
        attributionControl: false,
        zoomControl: false,
        spiderfyOnMaxZoom:  false

      });

    searchMap.addControl(L.control.attribution({
      prefix: ''
    }));

    const zoomer = new L.control.zoom({
      position: 'bottomright'
    }).addTo(searchMap);



    L.tileLayer('https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key='+azureMapsApiKey, {
      attribution: '© ' + new Date().getFullYear() + ' Microsoft, © 1992 - ' + new Date().getFullYear() + ' TomTom',
      maxZoom: 18,
      minZoom: 4,
      id: 'azuremaps.road',
      crossOrigin: true,
      subscriptionKey: this.config.azureMapsApiKey
    }).addTo(searchMap);


    this.mapObj = searchMap;
    this.getSearchData('init');
    this.bindActions();

    searchMap.on('zoomend',
      function() {
        if (searchMap.getZoom() === 18) {
          window.setTimeout(function() {
            $('#search-map').find('.marker-cluster').filter(':visible').click();
          },750);

        }
      });

  }
}

export default GiasSearchMap;
