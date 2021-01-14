import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
import GiasWaitMask from '../GiasModules/GiasWaitMask';

const $mc = $('#main-content');
const $fields = $mc.find(':input');


new GiasAttachUnload({
  fields: $fields
});

$mc.find('.remove-urn').okCancel({
  triggerEvent: 'click',
  title: 'Are you sure you want to remove this establishment?',
  content: 'This will permanently remove it from the list.',
  ok: function () {
    $(this.el).data().okCancel.unbind();
    $(this.el).click();
  }
});

new GiasWaitMask();
