const autocompletes = require('../GiasSearchForm/autocomplete-setup');
import GiasRadioToggles from "../GiasSearchForm/GiasRadioToggles";
import GiasSearchErrors from "../GiasSearchForm/GiasSearchErrors";

autocompletes.setUp();

$('#main-content').find('.search-toggle-panel').giasRadioToggles({
  untoggle: true
});

new GiasSearchErrors();
