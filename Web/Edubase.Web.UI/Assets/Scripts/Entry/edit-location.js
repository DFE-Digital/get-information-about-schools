import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
const locationAutoSuggest = require('../GiasEdit/locationAutoSuggests');
const  $main = $('#main-content');

if (window.isConfimingChanges) {
  new GiasAttachUnload();
} else {
  locationAutoSuggest.setUp();

  new GiasAttachUnload({
    fields: $main.find(':input')
  });
}
