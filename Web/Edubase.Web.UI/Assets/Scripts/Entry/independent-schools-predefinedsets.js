import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';
  let dismissed = false;
  const form = $('form');
  form.on('submit', function (e) {
    if (!dismissed) {
      e.preventDefault();
      $(this).okCancel({
        ok: function () {
          dismissed = true;
          $(e.target).find("#delete").click();
        },
        immediate: true,
        idPrefix: 'close-continue',
        title: 'Are you sure you want to remove this set?',
        content: 'You will no longer be able to filter by this set.'
      });
      $(this).removeData('okCancel');
    }
  });
