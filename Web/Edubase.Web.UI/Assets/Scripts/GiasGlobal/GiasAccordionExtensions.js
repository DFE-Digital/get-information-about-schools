
const $content = $('#main-content');
const giasAccordionExtensions = function(){
  if (!$content.find('.govuk-accordion').length) {
    return;
  }

  const accordions = $content.find('.govuk-accordion');

  accordions.each(function(n, accordion) {
    const accordionControls = $(accordion).find('.govuk-accordion__controls');
    const footerControls = $(accordionControls).clone()
      .insertAfter(accordion);


    const $scrollBtn = $(footerControls).prepend('<button class="gias-scroll-to-top">Back to top</button>');

    $(footerControls).find('.govuk-accordion__open-all').on('click', function(e) {
      e.preventDefault();
      accordionControls.eq(0).find('.govuk-accordion__open-all').trigger('click');
      $(this).html(accordionControls.eq(0).find('.govuk-accordion__open-all').html());
    });

    $(accordionControls).find('.govuk-accordion__open-all').on('click', function() {
      $(footerControls).find('.govuk-accordion__open-all').html($(this).html());
    });

    $(accordion).find('.govuk-accordion__icon').on('click', function() {
      window.setTimeout(function() {
        $(footerControls).find('.govuk-accordion__open-all').html(accordionControls.eq(0).find('.govuk-accordion__open-all').html());
      }, 0);
    });

    $scrollBtn.on('click', function(e) {
      e.preventDefault();
      window.scrollTo(0,0);
    });
  });
};

module.exports = giasAccordionExtensions;
