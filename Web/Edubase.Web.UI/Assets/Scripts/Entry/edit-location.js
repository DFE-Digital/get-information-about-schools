import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
import locationAutoSuggest from '../GiasEdit/locationAutoSuggests';
const  $main = $('#main-content');

if (window.isConfimingChanges) {
  new GiasAttachUnload();
} else {
  locationAutoSuggest.setUp();

  new GiasAttachUnload({
    fields: $main.find(':input')
  });
}
