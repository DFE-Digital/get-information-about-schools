const $filterForm = $('#filter-form');

import OptionSelect from "../GiasSearchFilters/OptionSelect";
import GiasAdditionalFilters from "../GiasSearchFilters/GiasAdditionalFilters";
import GiasSearchWithin from "../GiasSearchFilters/GiasSearchWithin";
import GiasNestedFilters from "../GiasSearchFilters/GiasNestedFilters";
import GiasSelectAll from "../GiasSearchFilters/GiasSelectAll";
import GiasFiltering from "../GiasSearchFilters/GiasFiltering";
import GiasSearchMap from "../GiasModules/GiasSearchMap";

new GiasAdditionalFilters();
$filterForm.find('.govuk-option-select').optionSelect();
$filterForm.find('.nested-items').giasNestedFilters();
$filterForm.find('.searchable-filters').giasSearchWithin();
$filterForm.find('.select-all').giasSelectAll();

new GiasFiltering();
const gMap = new GiasSearchMap();
gMap.bindActions();
