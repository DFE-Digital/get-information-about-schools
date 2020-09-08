DfE.Views.editIebt = {
  radios: $('#proprietor-radios').find('input'),

  okClick: function () {
    this.closeModal();
    DfE.Views.editIebt.refreshDisplay();
    return true;
  },
  cancelClick: function () {
    this.closeModal();
    var selectedVal = $('#proprietor-radios').find('input:checked').val();

    if (selectedVal === 'IndividualProprietor') {
      $('#proprietor-type-ProprietorBody').prop('checked', true).change();

    } else if (selectedVal === 'ProprietorBody') {
      $('#proprietor-type-IndividualProprietor').prop('checked', true).change();
    }
  },

  refreshIndividualProprietors: function (addedBool) {
    // we only want to show the 'remove' link if there are multiple individual proprietors
    var trueLength = $('.proprietorRow').length + (addedBool === true ? 1 : 0);
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

  refreshDisplay: function () {
    var self = this;
    var radioValue = typeof self.radios.filter(':checked').val() !== 'undefined' ? self.radios.filter(':checked').val() : 'unselected';

    if (radioValue === '1') {
      // individual proprietor
      $('#proprietor-type-IndividualProprietor').prop('checked', true);
      $('.proprietorRowTitle').removeClass('hidden');
      $('#proprietorAdd').removeClass('hidden');
      $('#ProprietorBody-chair').addClass('hidden');

      self.refreshIndividualProprietors();
    }
    if (radioValue === '2') {
      // proprietor body
      $('#proprietor-type-ProprietorBody').prop('checked', true);
      $('.proprietorRowTitle').addClass('hidden');
      $('#proprietorAdd').addClass('hidden');
      $('#ProprietorBody-chair').removeClass('hidden');

      // remove all additional Individual Proprietors
      $(".removeProprietor").slice(1).parent().parent().remove();
      self.refreshIndividualProprietors();
    }

    if (radioValue !== 'unselected') {
      $("#proprietor-entry").removeClass('hidden');
    }
  },
  init: function () {
    var self = this;
    if (self.radios.length === 0) {
      return;
    }

    $("#addProprietor").click(function (e) {
      e.preventDefault();
      $.ajax({
        url: "/Establishments/Establishment/Proprietor/Add/" + ($('.proprietorRow').length + 1),
        cache: false,
        success: function (html) { $("#proprietorList").append(html); }
      });
      self.refreshIndividualProprietors(true);

      $(".removeProprietor").on('click', function (e) {
        e.preventDefault();
        $(this).parent().parent().remove();
        self.refreshIndividualProprietors();
        return false;
      });

      return false;
    });

    self.radios.on('change', function (e) {
      var radioId = $(this).prop('id');

      if (radioId === 'proprietor-type-ProprietorBody') {
        self.radios.data().okCancel
          .updateModalContent('Are you sure you want to change to a proprietary body?',
            'All single proprietor details will be lost');
      } else {
        self.radios.data().okCancel
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
};

if (DfE.Views.editIebt.radios.length) {
  DfE.Views.editIebt.init();
}
