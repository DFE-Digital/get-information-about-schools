import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
//import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';

new GiasAttachUnload({
  fields: $('#main-content').find(':input')
})

if (document.getElementById('close-created-in-error')) {
  const form = $('#edit-group-details');
  let canCloseImmediately = false;
  const closeCheckbox = document.getElementById('close-created-in-error');

  form.on('submit', function(e) {
    if (closeCheckbox.checked && !canCloseImmediately) {
      e.preventDefault();
      $(this).okCancel({
        ok: function () {
          canCloseImmediately = true;
          $('#edit-submit').click();
        },
        immediate: true,
        idPrefix: 'close-continue',
        title: 'Are you sure you want to close this trust?',
        content:
          'All academies and shared governors will be removed on closure.'
      });
      $(this).removeData('okCancel');
    }
  });
}

const $closedDateInputs = $(".closed-date input[type='text']");
const $statusSelector = $("select#StatusId");

if ($closedDateInputs.length && $statusSelector.length) {
  const toggleClosedFields = function () {
    $closedDateInputs.prop("disabled", $statusSelector.val() != "2");
  };
  toggleClosedFields();
  $statusSelector.on("change", toggleClosedFields);
}

if ($("[id|='warning-modal-content']").length > 0) {
  var idInt = $("[id|='warning-modal-content']").attr('id').split('-').slice(-1);
  $('#overlay-proceed-'+idInt).focus();
}
