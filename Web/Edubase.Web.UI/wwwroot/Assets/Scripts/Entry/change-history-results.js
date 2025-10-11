import OptionSelect from '../GiasSearchFilters/OptionSelect';
import giasSearchChangeHistory from '../GiasSearchFilters/GiasSearchChangeHistory';
import GiasFilterToggle from '../GiasSearchFilters/GiasFilterToggle';
const _throttle = require('lodash.throttle');

$('.govuk-option-select').optionSelect();

giasSearchChangeHistory.bindEvents();

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
