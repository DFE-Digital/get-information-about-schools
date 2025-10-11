
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


    const $scrollBtn = $(footerControls).prepend('<a href="#top" class="gias-scroll-to-top back-to-top-link"><svg role="presentation" focusable="false" class="back-to-top-icon" xmlns="http://www.w3.org/2000/svg" width="13" height="17" viewBox="0 0 13 17">\n' +
      '            <path fill="currentColor" d="M6.5 0L0 6.5 1.4 8l4-4v12.7h2V4l4.3 4L13 6.4z"></path>\n' +
      '        </svg> Back to top</a>');

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

    // Removed to keep consistent with other Back to top links which don't rely on JS
    // changed to anchor of the #top element
    //$scrollBtn.on('click', function(e) {
    //  e.preventDefault();
    //  window.scrollTo(0,0);
    //});
  });
};

export default giasAccordionExtensions;
