const $filterForm = $('#filter-form');
const _throttle = require('lodash.throttle');
import OptionSelect from "../GiasSearchFilters/OptionSelect";
import GiasAdditionalFilters from "../GiasSearchFilters/GiasAdditionalFilters";
import GiasSearchWithin from "../GiasSearchFilters/GiasSearchWithin";
import GiasNestedFilters from "../GiasSearchFilters/GiasNestedFilters";
import GiasSelectAll from "../GiasSearchFilters/GiasSelectAll";
import GiasFiltering from "../GiasSearchFilters/GiasFiltering";
import GiasSearchMap from "../GiasModules/GiasSearchMap";
import GiasGovernorsDownloadWarning from "../GiasSearchFilters/GiasGovernorsDownloadWarning";
import GiasFilterToggle from '../GiasSearchFilters/GiasFilterToggle';
import throttle from '../GiasHelpers/throttle';

new GiasAdditionalFilters();
$filterForm.find('.govuk-option-select').optionSelect();
$filterForm.find('.nested-items').giasNestedFilters();

$filterForm.find('.select-all').giasSelectAll();

const filtering = new GiasFiltering();
const gMap = new GiasSearchMap();
gMap.bindActions();

window.gMap = gMap;
window.disableFilters = filtering.disableFilters;
window.enableFilters = filtering.enableFilters;
$filterForm.find('.searchable-filters').giasSearchWithin();

if (document.getElementById('governors-search-results')) {
  new GiasGovernorsDownloadWarning();
}
let popupFilters;
if ($(window).width() < 835) {
  popupFilters = new GiasFilterToggle();
}

window.addEventListener('resize',
  _throttle(function(){
    if($(window).width() < 835) {
      if (typeof popupFilters === 'undefined') {
        popupFilters = new GiasFilterToggle();
      } else {
        if(!popupFilters.initialised) {
          popupFilters.init();
        }
      }
    } else {
      if (typeof popupFilters !== 'undefined') {
        popupFilters.destroy()
      }
    }
  }, 250));
