import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';
const iebtRadios = {
  radios: $('#proprietor-radios').find('.govuk-radios__input'),

  cloneFields: $('#cloneable-fields-container').find('.cloneable').detach(),

  okClick: function () {
    this.closeModal();
    iebtRadios.refreshDisplay();
    return true;
  },
  cancelClick: function () {
    this.closeModal();
    const selectedVal = $('#proprietor-radios').find('.govuk-radios__input:checked').val();

    if (selectedVal === '1') {
      $('#proprietor-type-ProprietorBody').prop('checked', true).change();

    } else if (selectedVal === '2') {
      $('#proprietor-type-IndividualProprietor').prop('checked', true).change();
    }

  },

  refreshIndividualProprietors: function(addedBool) {
    const trueLength = $('.proprietorRow').length + (addedBool === true ? 1 : 0);
    if (trueLength < 2) {
      $('.removeProprietor').addClass('hidden');
    } else {
      $('.removeProprietor').removeClass('hidden');
    }

    // re-sequence the numbers
    $(".proprietorRowCounter").each(function (index) {
      $(this).text(index + 1);
    });
  },

  clearChairProprietor: function() {
    $('#ProprietorBody-chair').find(':input').each(function () {
      switch (this.type) {
        case 'password':
        case 'text':
        case 'textarea':
        case 'file':
        case 'select-one':
        case 'select-multiple':
        case 'date':
        case 'number':
        case 'tel':
        case 'email':
          $(this).val('');
          break;
        case 'checkbox':
        case 'radio':
          this.checked = false;
          break;
      }
    });
  },

  refreshDisplay: function () {
    const self = this;
    const radioValue = typeof self.radios.filter(':checked').val() !== 'undefined' ? self.radios.filter(':checked').val() : 'unselected';

    if (radioValue === '1') {
      // individual proprietor
      $('#proprietor-type-IndividualProprietor').prop('checked', true);
      $('.proprietorRowTitle').removeClass('hidden');
      $('#proprietorAdd').removeClass('hidden');
      $('#ProprietorBody-chair').addClass('hidden');
      $('#ProprietorBody').addClass('hidden');
      $('#IndividualProprietor').removeClass('hidden');

      self.refreshIndividualProprietors();
      self.clearChairProprietor();
    }
    if (radioValue === '2') {
      // proprietor body
      $('#proprietor-type-ProprietorBody').prop('checked', true);
      $('.proprietorRowTitle').addClass('hidden');
      $('#proprietorAdd').addClass('hidden');
      $('#ProprietorBody-chair').removeClass('hidden');
      $('#ProprietorBody').removeClass('hidden');
      $('#IndividualProprietor').addClass('hidden');

      // remove all additional Individual Proprietors
      $(".removeProprietor").slice(1).closest(".proprietorRow").remove();
      self.refreshIndividualProprietors();
    }

    if (radioValue !== 'unselected') {
      $("#proprietor-entry").removeClass('hidden');
    }
  },

  init: function () {
    const self = this;
    let overlayAttached = false;
    if (self.radios.length === 0) {
      return;
    }

    $("#addProprietor").click(function (e) {
      e.preventDefault();
      $.ajax({
        url: "/Establishments/Establishment/Proprietor/Add/" + ($('#Urn').val()) + "/" + ($('.proprietorRow').length + 1),
        cache: false,
        success: function (html) {
          const currentPosition = $(window).scrollTop();
          $("#proprietorList").append(html);
          $(window).scrollTop(currentPosition);
        }
      });
      self.refreshIndividualProprietors(true);
      return false;
    });

    $(document).on('click', ".removeProprietor", function (e) {
      e.preventDefault();
      const proprietorHasValues = $(e.target).closest(".proprietorRow").find(':input').filter(function(n, input) {
        return $(input).attr('type') === 'text' && $.trim($(input).val()) !== '';
      }).length > 0;

      if (proprietorHasValues) { // the proprietor about to be discarded has some values
        $(this).okCancel({
          ok: function () {
            if ($(e.target).closest(".proprietorRow").prev().length) {
              $(window).scrollTop($(e.target).closest(".proprietorRow").prev().offset().top);
            }
            $(e.target).closest(".proprietorRow").remove();
            self.refreshIndividualProprietors();
          },
          okLabel: "Yes",
          immediate: true,
          idPrefix: 'close-continue',
          title: 'Remove individual proprietor',
          content: 'Agreeing to remove this individual proprietor record will mean that this is removed from the system and cannot be re-instated.<br /><br />Are you sure you want to remove the record?'
        });

        $(this).removeData('okCancel');
        return false;
      } else {
        $(e.target).closest(".proprietorRow").remove();
        self.refreshIndividualProprietors();
      }
    });


    self.radios.on('change', function (e) {
      const radioId = $(this).prop('id');

      if (radioId === 'proprietor-type-ProprietorBody') {
        $('#proprietor-radios').find('input').data().okCancel
          .updateModalContent('Are you sure you want to change to a proprietary body?',
            'All single proprietor details will be lost');
      } else {
        $('#proprietor-radios').find('input').data().okCancel
          .updateModalContent('Are you sure you want to change to a single proprietor?',
            'All proprietary body details will be lost');
      }
    });


    const initialProprietorValue = $('input[name=\'ProprietorTypeId\']').val();


    self.radios.okCancel({
      ok: self.okClick,
      cancel: self.cancelClick,
      title: initialProprietorValue === '1' ?
        'Are you sure you want to change to a proprietary body?' :
        'Are you sure you want to change to a single proprietor?',
      content: initialProprietorValue === '1' ?
        'All single proprietor details will be lost' :
        'All proprietary body details will be lost',
      triggerEvent: 'change',
    });

   self.refreshDisplay();
  }
}

export default iebtRadios;
