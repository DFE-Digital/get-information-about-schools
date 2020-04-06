const autocompletes = require('../GiasSearchForm/autocomplete-setup');
import GiasRadioToggles from "../GiasSearchForm/GiasRadioToggles";
import GiasSearchWarnings from "../GiasSearchForm/GiasSearchWarnings";

autocompletes.setUp();

$('#main-content').find('.search-toggle-panel').giasRadioToggles({
  untoggle: true
});

new GiasSearchWarnings();
