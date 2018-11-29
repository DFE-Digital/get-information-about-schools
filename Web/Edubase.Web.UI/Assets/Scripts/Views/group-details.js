
DfE.Views.GroupDetails = (function () {
    'use strict';

    var initFunc = "DfE.mapInteractions.initGroupMap", $map = $('#group-map');

    var initMap = function () {
        var mobileToggleSwitch = $('#map-toggle'), mapInitialised = false;

        // map only shows by default on larger screens
        if ($map.length && $map.css('display') === 'block' && $map.is(":visible")) {
          window.setTimeout(function(){
            DfE.mapInteractions.initGroupMap();
            mapInitialised = true;
          }, 0);

        }

        // show the map in collapsable panel
        mobileToggleSwitch.on('click', function (e) {
            e.preventDefault();
            if ($(this).hasClass('trigger-open')) {
                $map.css({ display: 'none' });
                $(this).text('Show map');
            } else {
                if (!mapInitialised) {
                    DfE.mapInteractions.initGroupMap();
                    mapInitialised = true;
                }
                $map.css({ display: 'block' });
                window.groupMap.invalidateSize();
                $(this).text('Close map');
            }
            $(this).toggleClass('trigger-open');
        });

        $(window).on('tabChange', function (e) {
            if (!mapInitialised) {
                DfE.mapInteractions.initGroupMap();
                mapInitialised = true;
            } else window.groupMap.invalidateSize();
        });
    };

    if ($map.length) {
        initMap();
    }
}());
