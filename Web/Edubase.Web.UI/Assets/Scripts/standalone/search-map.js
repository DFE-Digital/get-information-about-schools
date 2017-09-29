DfE = window.DfE || {};
DfE.searchMap = (function () {
    var $filterForm = $('#filter-form');
    return {
        scriptsLoaded: false,
        softLimit: 900,
        step: 100,
        breachLimit: false,
        dataRefreshRequired: false,
        googleApiKey: DfE.mapConfig.apiKey,
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
        setUp: function() {
            'use strict';
            $('#results-container').addClass('hidden');
            $('#map-container').removeClass('hidden');

            if (!DfE.searchMap.scriptsLoaded) {
                var s = document.createElement('script');
                s.src = 'https://maps.googleapis.com/maps/api/js?key=' +
                    DfE.searchMap.googleApiKey +
                    '&callback=DfE.searchMap.initMap';
                document.body.appendChild(s);
                DfE.searchMap.scriptsLoaded = true;

            } else {
                DfE.searchMap.getSearchData();
            }

        },
        getSearchData: function() {
            'use strict';
            
            var self = this;
            var resultCount = 0;
            var startIndex = 0;
            var pointCount = 0;
            this.clusterGroup = new L.MarkerClusterGroup();
            this.breachLimit = false;
            $('#map-count').text('0');
           

            DfE.searchResults.disableFilters();
            getPoints();

            function getPoints() {
                $('.map-header').addClass('loading');
                $('#map-data-warning').addClass('hidden');
                $.ajax({
                    url: '/Establishments/Search/results-json',
                    data: DfE.searchResults.params() + '&startIndex=' + startIndex,
                    dataType: 'JSON',
                    success: function(data, status, xhr) {
                        if (Number(xhr.getResponseHeader("x-count")) > 0) {
                            resultCount = Number(xhr.getResponseHeader("x-count"));
                        }

                        $(data).each(function(n, elem) {
                            if (elem.location) {
                                var lat = Number(elem.location.latitude);
                                var lon = Number(elem.location.longitude);
                                var iconType = elem.status.toLowerCase().indexOf('open') > -1
                                    ? self.openIcon
                                    : self.closeIcon;

                                var marker = new L.marker([lat, lon], { icon: iconType });
                                var tooltipContent = [
                                    '<a href="' +
                                    self.establishmentDetailBaseUrl +
                                    elem.urn +
                                    '">' +
                                    elem.name +
                                    '</a>',
                                    elem.address, 'Phase/type: ' + elem.phaseType,
                                    'URN: ' + elem.urn + '  LAESTAB: ' + elem.laestab,
                                    'Status: ' + elem.status
                                ];

                                marker.bindPopup(tooltipContent.join('<br>'), { offset: new L.Point(0, -20) });

                                self.clusterGroup.addLayer(marker);
                                pointCount ++;

                            }

                        });
                        var countDisplayed = Number($('#map-count').text());
                        if (pointCount > countDisplayed) {
                            $('#map-count').text(pointCount);
                        }

                        if (startIndex <= resultCount) {
                            if (pointCount > self.softLimit && !self.breachLimit) {
                                pointsLoaded();
                                $('#map-data-warning').removeClass('hidden');
                                $('#map-count').text('1000');
                                $('#estimated-remaining')
                                    .text('Approximately ' +
                                        Math.round((resultCount - pointCount) / 10) * 10 +
                                        ' remaining.');
                                $('#show-all-map').one('click',
                                    function(e) {
                                        e.preventDefault();
                                        DfE.searchMap.breachLimit = true;
                                        startIndex += DfE.searchMap.step;
                                        $('#map-data-warning').addClass('hidden');
                                        getPoints();
                                    });

                            } else {
                                startIndex += DfE.searchMap.step;
                                getPoints();
                            }

                        } else {
                            pointsLoaded();
                        }

                    },
                    error: function(xhr) {
                        console.log(xhr);
                    }
                });
            }

            function pointsLoaded() {
                self.clusterGroup.setZIndex(10);
                if (!self.mapObj.hasLayer(self.clusterGroup)) {
                    self.mapObj.addLayer(self.clusterGroup);
                }

                self.mapObj.fitBounds(self.clusterGroup.getBounds());
                window.setTimeout(function() {
                    $('.map-header').removeClass('loading');
                    DfE.searchResults.enableFilters();
                },1500);
            }

        },
        clearPoints: function() {
            this.mapObj.removeLayer(this.clusterGroup);

        },
        bindActions: function() {
            var self = this;
            $('#full-content').on('click',
                '#view-list',
                function(e) {
                    e.preventDefault();
                    $('#map-container').addClass('hidden');
                    $('#results-container').removeClass('hidden');
                    DfE.searchMap.currentView = 'list';
                });

            $('#full-content').on('click',
                '#view-map',
                function(e) {
                    e.preventDefault();
                    if (!DfE.searchMap.scriptsLoaded) {
                        DfE.searchMap.setUp();
                    } else {
                        $('#results-container').addClass('hidden');
                        $('#map-container').removeClass('hidden');
                        if (DfE.searchMap.dataRefreshRequired) {
                            DfE.searchMap.getSearchData();
                            DfE.searchMap.dataRefreshRequired = false;
                        }
                    }

                    DfE.searchMap.currentView = 'map';
                });
        },
        initMap: function() {
            'use strict';
            var self = this;
            self.currentView = 'map';
            var searchMap = new L.map('search-map',
                {
                    center: new L.LatLng(51.51, -0.11),
                    zoom: 9,
                    attributionControl: false,
                    zoomControl: false,
                    spiderfyOnMaxZoom:  false

                });

            var zoomer = new L.control.zoom({
                position: 'bottomright'
            }).addTo(searchMap);

            var googleTiles = new L.Google('ROADMAP');
            var satelliteTiles = new L.Google('SATELLITE');

            L.control.layers({
                    'Map': googleTiles,
                    'Satellite': satelliteTiles
                },
                null,
                { position: 'topleft', collapsed: false }
            ).addTo(searchMap);


            searchMap.addLayer(googleTiles);

            this.mapObj = searchMap;
            this.getSearchData(true);
            this.bindActions();

            searchMap.on('zoomend',
                function() {
                    if (searchMap.getZoom() === 18) {
                       window.setTimeout(function() {
                           $('#search-map').find('.marker-cluster').filter(':visible').click();
                           console.log('Attempted click');
                       },750);
                       
                   }
                });            

        }
    };
}());
