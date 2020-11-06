class GiasFilterToggle {
  constructor() {
    this.init();
  }

  init() {
    const $app = $('#app');
    const filterToggle = $('#gias-filter-toggle');
    const startingCount = $('#list-count').text();


    let mobileFilters = $('#filter-form');

    const mobileFiltersContainer = $(
      `<div class="gias-mobile-filters hidden" aria-hidden="true">
            <div class="gias-mobile-filters__header">
                <a href="#" role="button" class="gias-mobile-filters__close">Back</a>
                <h2 class="govuk-heading-s">Filters</h2>
                <a href="#" role="button" class="gias-mobile-filters__clear">Clear all filters</a>
            </div>
            <div class="gias-mobile-filters__panel">

            </div>
            <div class="gias-mobile-filters__footer">
                <button class="govuk-button gias-mobile-filters__close" id="gias-mobile-filter-submit">
                    View results
                    <span class="mobile-count">(${startingCount})</span>
                </button>
            </div>
        </div>`);

    mobileFilters.detach().appendTo(mobileFiltersContainer.find('.gias-mobile-filters__panel'));

    mobileFiltersContainer.appendTo('body');

    filterToggle.on('click', function(e) {
      e.preventDefault();
      mobileFiltersContainer.removeClass('hidden').removeAttr('aria-hidden');
      $app.addClass('hidden').attr('aria-hidden', true);
    });

    $(document).on('click', '.gias-mobile-filters__clear',function(e) {
      $('#clear-filters').click();
    });

    $(document).on('click', '.gias-mobile-filters__close',function (e){
      e.preventDefault();
      mobileFiltersContainer.addClass('hidden').attr('aria-hidden', true);
      $app.removeClass('hidden').removeAttr('aria-hidden');
    });

    $('.gias-filter-toggle--container').css({ display: 'block' });
  }

  destroy() {
    const filters = $('.gias-mobile-filters__panel').detach();
    $('#gias-filter-container').append(filters);
  }

}

export default GiasFilterToggle;
