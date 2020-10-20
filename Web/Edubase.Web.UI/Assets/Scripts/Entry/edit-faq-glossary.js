import GiasTextCounter from '../GiasModules/GiasTextCounter';
import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';

new GiasTextCounter(document.querySelector('.entry-content'), { maxLength: 4000 });
const $delete = $('#delete-button');
let warningTitle = 'Are you sure you want to delete this FAQ entry?';
let warningContent = 'The entry will be removed from the FAQ immediately';

if (window.isGlossaryEdit) {
  warningTitle = warningTitle.replace('FAQ', 'glossary');
  warningContent = warningContent.replace('FAQ', 'glossary');
}


$delete.okCancel({
  ok: function () {
    $delete.data().okCancel.unbind();
    $delete.click();
  },
  cancel: function() {
    this.closeModal();
  },
  title: warningTitle,
  content: warningContent,
  triggerEvent: 'click'
});
