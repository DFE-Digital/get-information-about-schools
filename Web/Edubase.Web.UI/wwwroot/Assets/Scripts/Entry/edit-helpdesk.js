import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
import GiasTextCounter from '../GiasModules/GiasTextCounter';
if (window.isConfimingChanges) {
  new GiasAttachUnload();
} else {
  new GiasAttachUnload({
    fields: $('#main-content').find(':input')
  });

  new GiasTextCounter(document.getElementById('HelpdeskNotes'), { maxLength: 255 });
}
