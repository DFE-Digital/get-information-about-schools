import GiasTableSort from "../GiasModules/GiasTableSort";
import GiasTabs from "../GiasModules/GiasTabs";

import GiasLandingMap from "../GiasLandingPages/GiasLandingMap";

const $main = $('#main-content');

const $map = $('#map, #group-map');
const mapToggleSwitch = $('#map-toggle');
let mapInitialised = false;

$main.find('.gias-tabs-wrapper').giasTabs();

$('#school-governance, #governance').find('.sortable-table').giasTableSort();

if ($map.length && $map.css('display') === 'block') {
  new GiasLandingMap();
  mapInitialised = true;
} else {
  mapToggleSwitch.on('click', function(e) {
    e.preventDefault();
    if (mapToggleSwitch.hasClass('trigger-open')) {
      $map.css({ display: 'none' });
      $(this).text('Show map');

    } else {
      $map.css({ display: 'block' });
      $(this).text('Hide map');
      if (!mapInitialised) {
        new GiasLandingMap();
        mapInitialised = true;
      }
    }
    mapToggleSwitch.toggleClass('trigger-open');
  });
}

