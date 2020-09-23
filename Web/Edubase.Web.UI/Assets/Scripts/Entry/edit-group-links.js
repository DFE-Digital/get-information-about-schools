import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
const isFederation = document.getElementById('GroupTypeName') && document.getElementById('GroupTypeName').value === 'Federation';

const modalTitle = isFederation ? 'Are you sure you want to remove this school?': 'Are you sure you want to remove this academy?';
const modalContent = isFederation ? 'All school information will be removed.' : 'All governor information associated with this academy will be removed too.';

$('#main-content').find('.button-remove').okCancel({
  triggerEvent: 'click',
  title: modalTitle,
  content: modalContent,
  idPrefix: 'remove-',
  ok: function () {
    $(this.el).data().okCancel.unbind();
    $(this.el).click();
  }
});

new GiasAttachUnload();
