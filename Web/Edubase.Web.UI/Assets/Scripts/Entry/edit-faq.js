import GiasTextCounter from '../GiasModules/GiasTextCounter';
import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';

new GiasTextCounter(document.querySelector('.entry-content'));
const $delete = $('#delete-button');

$delete.okCancel({
  ok: function () {
    $delete.data().okCancel.unbind();
    $delete.click();
  },
  cancel: function() {
    this.closeModal();
  },
  title: 'Are you sure you want to delete this FAQ entry?',
  content: 'The entry will be removed from the FAQ immediately',
  triggerEvent: 'click'
});
