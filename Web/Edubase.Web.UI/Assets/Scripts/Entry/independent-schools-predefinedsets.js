import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';

const formElements = $('form');

// For each form element, add the confirmation behaviour to prompt if the user is sure about deleting an LA set.
// Note that when JavaScript is disabled, currently the form will submit without confirmation.
for (let i = 0; i < formElements.length; i++) {
  const formElement = $(formElements[i]);
  formElement.okCancel({
    ok: function () {
      // Disable the form submit handler, to allow the form to submit
      formElement.off('submit');
      formElement.submit();
    },
    immediate: false,
    triggerEvent: 'submit',
    title: 'Are you sure you want to remove this set?',
    content: 'You will no longer be able to filter by this set.'
  });
}

// When editing the LA set, options are displayed as pills and can individually be removed.
// Note that this functionality requires JavaScript to function correctly.
$('#la-id-target').on('click', '.remove-suggest-la', function (e) {
  e.preventDefault();
  $('#' + $(this).text().toLowerCase().replace(/\s/g, '-')).remove();
  $(this).remove();
});
