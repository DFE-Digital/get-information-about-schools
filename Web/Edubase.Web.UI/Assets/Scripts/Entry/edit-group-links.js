import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';

let modalTitle;
let modalContent;
let groupType;
if (document.getElementById('GroupTypeName')) {
  groupType = document.getElementById('GroupTypeName').value;
}

if (groupType === 'Federation') {
  modalTitle = 'Are you sure you want to remove this school?';
  modalContent = 'All school information will be removed.';
} else if (groupType.indexOf('Children\'s') > -1) {
  modalTitle = 'Are you sure you want to remove this children\'s centre?';
  modalContent = 'All children\'s centre information will be removed from this children\'s centre group record.';
} else {
  modalTitle = 'Are you sure you want to remove this academy?';
  modalContent = 'All governor information associated with this academy will be removed too.';

}


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
