import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';

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
  $('#app').attr('aria-hidden', true);
  $('html').addClass('no-scroll');
  $('.modal-exit').focus();

  $('body').on('keydown', '.modal-content', (e) => {
    if (e.keyCode === 27) { // esc
      $('.modal-exit').click();
      return;
    }
    if (e.keyCode === 9) { // tab or maj+tab
      const focusableItems = $('.modal-content').find('a[href], area[href], input:not([disabled]), button:not([disabled])').filter(':visible');
      const focusableItemsCount = focusableItems.length;

      const focusedItem = $(document.activeElement);

      const focusedItemIndex = focusableItems.index(focusedItem);

      if (!e.shiftKey && (focusedItemIndex === focusableItemsCount - 1)) {
        focusableItems.get(0).focus();
        e.preventDefault();
      }
      if (e.shiftKey && focusedItemIndex === 0) {
        focusableItems.get(focusableItemsCount - 1).focus();
        e.preventDefault();
      }
    }
  });
}
