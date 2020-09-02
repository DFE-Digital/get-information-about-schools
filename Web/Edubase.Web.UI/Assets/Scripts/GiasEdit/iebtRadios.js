import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';
const iebtRadios = {
  radios: $('#proprietor-radios').find('input'),

  cloneFields: $('#cloneable-fields-container').find('.cloneable').detach(),

  okClick: function () {
    this.closeModal();
    $('#SingleProprietor, #ProprietorBody').find('.govuk-input').val('');

    $('#proprietor-radios').find('input').each(function () {
      $(this).data().okCancel.pause(true);
    });


    $('#ProprietorBody, #SingleProprietor').on('change, keydown', '.govuk-input', function () {

      $('#proprietor-radios').find('input').each(function () {
        $(this).data().okCancel.pause();
      });

      $('#ProprietorBody, #SingleProprietor').off('change, keydown', '.govuk-input');
    });

    return true;
  },
  cancelClick: function () {
    this.closeModal();
    var selectedVal = $('#proprietor-radios').find('input:checked').val();

    if (selectedVal === 'SingleProprietor') {
      $('#proprietor-type-ProprietorBody').prop('checked', true).change();

    } else if (selectedVal === 'ProprietorBody') {
      $('#proprietor-type-SingleProprietor').prop('checked', true).change();
    }

  },
  init: function () {
    const self = this;
    let overlayAttached = false;
    if (self.radios.length === 0) {
      return;
    }

    const radioValue = typeof self.radios.filter(':checked').val() !== 'undefined' ? self.radios.filter(':checked').val() : 'SingleProprietor';

    if (radioValue === 'ProprietorBody') {
      $('#field-clone-target').append(self.cloneFields);
    } else {
      $('#cloneable-fields-container').append(self.cloneFields);
      $('#proprietor-type-SingleProprietor').prop('checked', true);
      $('#SingleProprietor').removeClass('hidden');
    }

    self.radios.on('change', function (e) {
      const radioId = $(this).prop('id');

      if (radioId === 'proprietor-type-ProprietorBody') {
        $('#field-clone-target').append(self.cloneFields);
        $('#proprietor-radios').find('input').data().okCancel
          .updateModalContent('Are you sure you want to change to a proprietary body?',
            'All single proprietor details will be lost');
      } else {
        $('#cloneable-fields-container').append(self.cloneFields);
        $('#proprietor-radios').find('input').data().okCancel
          .updateModalContent('Are you sure you want to change to a single proprietor?',
            'All proprietary body details will be lost');
      }
    });

    self.radios.okCancel({
      ok: self.okClick,
      cancel: self.cancelClick,
      idPrefix: 'iebt-'
    });
  }
}

export default iebtRadios;
