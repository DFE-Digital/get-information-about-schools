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

    if (selectedVal === '1') {
      $('#proprietor-type-ProprietorBody').prop('checked', true).change();

    } else if (selectedVal === '2') {
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
    var self = this;
    var radioValue = typeof self.radios.filter(':checked').val() !== 'undefined' ? self.radios.filter(':checked').val() : 'unselected';

    if (radioValue === '1') {
      // individual proprietor
      $('#proprietor-type-IndividualProprietor').prop('checked', true);
      $('.proprietorRowTitle').removeClass('hidden');
      $('#proprietorAdd').removeClass('hidden');
      $('#ProprietorBody-chair').addClass('hidden');

      self.refreshIndividualProprietors();
      self.clearChairProprietor();
    }
    if (radioValue === '2') {
      // proprietor body
      $('#proprietor-type-ProprietorBody').prop('checked', true);
      $('.proprietorRowTitle').addClass('hidden');
      $('#proprietorAdd').addClass('hidden');
      $('#ProprietorBody-chair').removeClass('hidden');

      // remove all additional Individual Proprietors
      $(".removeProprietor").slice(1).closest(".proprietorRow").remove();
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
        success: function (html) {
          var currentPosition = $(window).scrollTop();
          $("#proprietorList").append(html);
          $(window).scrollTop(currentPosition);
        }
      });
      self.refreshIndividualProprietors(true);
      return false;
    });

    $(document).on('click', ".removeProprietor", function (e) {
      e.preventDefault();

      if ($(this).closest(".proprietorRow").prev().length) {
        $(window).scrollTop($(this).closest(".proprietorRow").prev().offset().top);
      }
      $(this).closest(".proprietorRow").remove();
      self.refreshIndividualProprietors();
      return false;
    });

    self.radios.on('change', function (e) {
      var radioId = $(this).prop('id');

      if (radioId === 'proprietor-type-ProprietorBody') {
        self.radios.data().okCancel
          .updateModalContent('Are you sure you want to change to a proprietary body?',
            'All individual proprietor details will be lost');
      } else {
        self.radios.data().okCancel
          .updateModalContent('Are you sure you want to change to an individual proprietor?',
            'All proprietary body details will be lost');
      }
    });

    self.radios.okCancel({
      ok: self.okClick,
      cancel: self.cancelClick,
      idPrefix: 'iebt-'
    });

    DfE.Views.editIebt.refreshDisplay();
  }
};

if (DfE.Views.editIebt.radios.length) {
  DfE.Views.editIebt.init();
}
