
DfE.Views.SchoolDetails = (function () {
    'use strict';

    var $map = $('#map'),
        $tabsWrapper = $('#content').find('.horizontal-tabs-wrapper');

    var initSchoolMap = function () {
        var mobileToggleSwitch = $('#map-toggle'),
        mapInitialised = false;

        // map only shows by default on larger screens
        if ($map.length && $map.css('display') === 'block') {
          window.setTimeout(function(){
            DfE.mapInteractions.initMap();
            mapInitialised = true;
          }, 0);
        }

        // show the map in collapsable panel
        mobileToggleSwitch.on('click', function (e) {
            e.preventDefault();
            if (mobileToggleSwitch.hasClass('trigger-open')) {
                $map.css({ display: 'none' });
                $(this).text('Show map');

            } else {
              $map.css({ display: 'block' });
              $(this).text('Close map');
                if (!mapInitialised) {
                  DfE.mapInteractions.initMap();
                }
            }
          mobileToggleSwitch.toggleClass('trigger-open');
        });
    };


    if ($map.length) {
        initSchoolMap();
    }

    if ($tabsWrapper.length) {
        $tabsWrapper.tabs();
    }
}());
