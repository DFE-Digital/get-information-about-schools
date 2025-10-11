const typeToPhase = {
  filterEstablishmentPhases: function() {
    const $typeSelector = $("#TypeId");
    const $phaseSelector = $("select#EducationPhaseId");

    let optionList = '<option value="">Please select</option>';
    const typeId = $typeSelector.val();
    const  previousSelValue = $phaseSelector.val();
    if(!isNaN(typeId)) {
      var applicablePhases = window.type2PhaseMap[typeId];
      if(applicablePhases && applicablePhases.length > 0) {
        for (let i = 0; i < applicablePhases.length; i++) {
          const filtered = $.grep(phases, function(ele, y) {
            return parseInt(ele.value, 10) == applicablePhases[i];
          });
          if(filtered !== null && filtered.length > 0) {
            optionList += '<option value="' + filtered[0].value + '" '+ (previousSelValue == filtered[0].value ? "selected" : "") +'>' + filtered[0].text + '</option>';
          }
        }
      }
    }
    $phaseSelector.html(optionList);
  },
  bindEvents: function() {
    if (window.type2PhaseMap) {
      const $typeSelector = $("#TypeId");
      const self = this;
      this.filterEstablishmentPhases();
      $typeSelector.on('change', function() {
        self.filterEstablishmentPhases();
      });
    }
  }
}

export default typeToPhase;
