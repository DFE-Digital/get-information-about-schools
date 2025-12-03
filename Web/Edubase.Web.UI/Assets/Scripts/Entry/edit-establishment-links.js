import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';

new GiasAttachUnload({
  fields: $('#main-content').find(':input')
});

(function() {
  const $linkedDateFields = $('#main-content').find('.linked-estab-date .govuk-input');

  $('#ActiveRecord_CreateReverseLink').change(function () {
    $("#create-reserve-link").toggleClass('hidden');
  });
  $('#ActiveRecord_ReverseLinkSameDate').change(function () {
    if (this.checked) {
      $linkedDateFields.prop('disabled', 'disabled').val('');
    } else {
      $linkedDateFields.prop("disabled", false);
    }
  });
}());

