import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
const locationAutoSuggest = require('../GiasEdit/locationAutoSuggests');

if (window.isConfimingChanges) {
  new GiasAttachUnload();
} else {
  locationAutoSuggest.setUp();

  new GiasAttachUnload({
    fields: $('#main-content').find(':input')
  });
}
