
DfE.Views.SchoolDetails = (function () {
    'use strict';

    var $map = $('#map'),
        $tabsWrapper = $('#content').find('.horizontal-tabs-wrapper');

    var initSchoolMap = function () {
        var mobileToggleSwitch = $('#map-toggle'),
        mapInitialised = false;

        // map only shows by default on larger screens
        if ($map.length && $map.css('display') === 'block') {
            DfE.mapInteractions.loadGoogleScript();
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
                    DfE.mapInteractions.loadGoogleScript();
                    mapInitialised = true;
                } 
                $map.css({ display: 'block' });
                $(this).text('Hide map');
            }

            $(this).toggleClass('trigger-open');

        });
    }


    if ($map.length) {
        initSchoolMap();
    }

    if ($tabsWrapper.length) {        
        $tabsWrapper.tabs();
    }
}());
