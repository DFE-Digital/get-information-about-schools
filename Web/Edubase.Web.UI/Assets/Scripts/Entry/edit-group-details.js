import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';

new GiasAttachUnload({
  fields: $('#main-content').find(':input')
})

if (document.getElementById('close-created-in-error')) {
  let modalMessage = ' ';
  const form = $('#edit-group-details');
  const $groupTypeName = $("#GroupTypeName").val();
  let $groupTypeShortName = $groupTypeName.split(" ").pop().toLowerCase();
  let $groupTypeLinks = "academies and shared governors";

  if ($groupTypeShortName !== "trust") {
	  $groupTypeShortName = $groupTypeName.toLowerCase();
	  $groupTypeLinks = "academies and/or free schools";
  }

  if ($groupTypeName == 'Federation') {
    $groupTypeLinks = "establishments";
  }

  if ($groupTypeName == 'Secure single-academy trust') {
    $groupTypeLinks = "academies";
  }

  if ($groupTypeName == 'Single-academy trust') {
    $groupTypeLinks = "academies";
  }

  if($groupTypeName !== 'School sponsor') {
    modalMessage = 'All ' + $groupTypeLinks + ' will be removed on closure.'
  }

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
        title: 'Are you sure you want to close this ' + $groupTypeShortName + ' record ?',
        content: modalMessage
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
