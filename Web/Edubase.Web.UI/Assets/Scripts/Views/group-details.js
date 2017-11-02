
DfE.Views.GroupDetails = (function () {
    'use strict';
    
    var initFunc = "DfE.mapInteractions.initGroupMap", $map = $('#group-map');

    var initMap = function () {
        var mobileToggleSwitch = $('#map-toggle'), mapInitialised = false;

        // map only shows by default on larger screens
        if ($map.length && $map.css('display') === 'block') {
            DfE.mapInteractions.loadGoogleScript(initFunc);
            mapInitialised = true;
        }

        // show the map in collapsable panel
        mobileToggleSwitch.on('click', function (e) {
            e.preventDefault();
            if ($(this).hasClass('trigger-open')) {
                $map.css({ display: 'none' });
                $(this).text('Show map');
            } else {
                if (!mapInitialised) {
                    DfE.mapInteractions.loadGoogleScript(initFunc);
                    mapInitialised = true;
                } 
                $map.css({ display: 'block' });
                window.groupMap.invalidateSize();
                $(this).text('Close map');
            }
            $(this).toggleClass('trigger-open');
        });
    }
    
    if ($map.length) {
        initMap();
    }
}());
