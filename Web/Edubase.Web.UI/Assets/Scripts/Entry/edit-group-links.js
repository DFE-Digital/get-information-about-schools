import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';

$('#main-content').find('.button-remove').okCancel({
  triggerEvent: 'click',
  title: 'Are you sure you want to remove this academy?',
  content: 'All governor information associated with this academy will be removed too.',
  idPrefix: 'remove-',
  ok: function () {
    $(this.el).data().okCancel.unbind();
    $(this.el).click();
  }
});

new GiasAttachUnload();
