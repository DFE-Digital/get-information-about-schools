/*import Vue from 'vue';
const ceApp = new Vue({
  el: '#create-establishment'
});*/
import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
(function () {
  const $radios = $('#number-gen-options').find(':radio');
  const $numberPanel = $('#enter-number');
  const $typeSelector = $('#EstablishmentTypeId');
  const $phaseSelector = $('#EducationPhaseId');
  const $ccFields = $('.childrens-centre-fields');

  function estabNumberToggle() {
    $radios.filter(':checked').val() === 'False' ?
      $numberPanel.removeClass('hidden') : $numberPanel.addClass('hidden');
  }

  function createOther() {
    $('#establishment-phase-form-group').removeClass('hidden');
    $('#number-gen-options').removeClass('hidden');
    $ccFields.addClass('hidden');
  }

  function createChildrensCentre() {
    $('#establishment-phase-form-group').addClass('hidden');
    $('#number-gen-options').addClass('hidden');
    $numberPanel.addClass('hidden');
    $ccFields.removeClass('hidden');
  }

  function filterEstablishmentPhaseList() {
    let optionList = `<option value=''>Please select</option>`;
    const typeId = $typeSelector.val();
    const previousSelValue = (typeId === '41' ? '1' : $phaseSelector.val());
    if (typeId !== '') {
      const applicablePhases = type2PhaseMap[typeId];
      if (applicablePhases && applicablePhases.length > 0) {
        for (let i = 0; i < applicablePhases.length; i++) {
          const filtered = $.grep(phases, function(ele, y) {
            return parseInt(ele.value, 10) === parseInt(applicablePhases[i], 10);
          });
          if (filtered !== null && filtered.length > 0) {
            const sel = previousSelValue === filtered[0].value ? 'selected' : '';
            optionList += `<option value='${filtered[0].value }' ${sel} > ${filtered[0].text}</option>`;
          }
        }
      }

      if (typeId === '41') {
        createChildrensCentre();
      } else {
        createOther();
        estabNumberToggle();
      }
    } else {
      $("#establishment-phase-form-group").addClass('hidden');
      $("#number-gen-options").addClass('hidden');
      $ccFields.addClass('hidden');
    }
    $phaseSelector.html(optionList);
  }

  // attach events
  if ($typeSelector.val() !== '') {
    if ($typeSelector.val() === '41') {
      createChildrensCentre();
    } else {
      createOther();
      estabNumberToggle();
    }
  }

  $radios.on('change',estabNumberToggle);
  $typeSelector.on('change keyup', filterEstablishmentPhaseList);
  new GiasAttachUnload();

if (document.getElementById('warning-modal-content-0')) {
  // When the page reloads with a warning modal not opened via JS
  $('.modal-exit, .cancel').on('click', function(e){
    e.preventDefault();
    $('.modal-content , .modal-overlay').addClass('hidden');
  });

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


}());
