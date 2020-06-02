import QueryString from "../GiasHelpers/QueryString";
class GiasAdditionalFilters {
  constructor() {
    this.buildAdditionalFilters();
  }

  buildAdditionalFilters() {
    let frag = '';
    const additionalFilters = $('#EditSearchCollapse').find('.additional-search-critera');
    function checkBoxTemplate(props) {
      return `<div class="govuk-checkboxes__item">
              <input value="#${props.elemId}" data-alias="${props.dataAlias}" id="ctrl-${props.elemId}" class="js-filter-input additional-search-critera govuk-checkboxes__input" type="checkbox">
                <label for="ctrl-${props.elemId}" class="js-filter-label govuk-label govuk-checkboxes__label">
                    ${props.text}
                </label>
            </div>`;
    }

    $.each(additionalFilters, function (n, elem) {
      let props = {};
      props.elemId = elem.id;
      props.text = $(elem).find('.option-select-label').text();
      props.dataAlias = $(elem).data().bindAlias;
      frag += checkBoxTemplate(props);
    });

    $('#filter-type-target').append(`<div class="govuk-checkboxes gias-filter-checkboxes">${frag}</div>`);

    this.bindAdditionalFilterEvents();
  }

  bindAdditionalFilterEvents() {
    const maybeValidatedClientSide = [
      'ctrl-open-date-filter',
      'ctrl-close-date-filter',
      'ctrl-option-select-AgeRangeLow',
      'ctrl-option-select-AgeRangeHigh'
    ];

    function resetFieldErrors(filterId){
      const errorSummary = $('#js-filter-error-summary');
        $(filterId).find('.govuk-form-group').removeClass('govuk-form-group--error');
        $(filterId).find('.govuk-error-message').addClass('hidden');
        $(filterId).find('input[type="text"]').val('');
        errorSummary.find('.' + filterId + '-error').addClass('hidden');
    }

    $("#additional-filter-wrap input").on("change", function (e) {
      const chxId = this.id;
      const allAdditionalFilters = $('#EditSearchCollapse').find('.additional-search-critera');
      const $selectedFilters = $('#additional-filter-wrap').find('input:checked');

      const toShow = $.map($selectedFilters, (filter)=>{
        return '#' + filter.id.replace('ctrl-','');
      });

      const aliases = $.makeArray($selectedFilters.map((n, filter) => {
        return $(filter).data().alias;
      })).join('');

      $('#selected-search-filters').val(aliases);

      allAdditionalFilters.addClass('hidden');
      $(toShow.join(',')).removeClass('hidden');

      if (maybeValidatedClientSide.indexOf(chxId) > -1) {
        resetFieldErrors(chxId);
      }
    });
  }

}

export default GiasAdditionalFilters;
