const $ = require('jquery');

class GiasAccordionExtensions {
  constructor(accordion) {
    this.accordion = accordion;
    this.init();
  }

  getAccordionState() {
    const regex = new RegExp('[\\?&]accordionstate=([^&#]*)', 'i');
    let results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
  }

  setAccordionState() {
    let accordionState = [];
    let newUrl = '';
    const uri =  window.location.toString();
    const regex = new RegExp("([?&])accordionstate=.*?(&|$)", "i");

    $(this.accordion).find('.govuk-accordion__section').each(function(n, el) {
      if ($(el).hasClass('govuk-accordion__section--expanded')) {
        accordionState.push(n);
      }
    });

    const separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (accordionState.length > 0) {
      if (uri.match(regex)) {
        newUrl = uri.replace(regex, '$1' + "accordionstate=" + accordionState.join('|') + '$2');
      } else {
        newUrl = uri + separator + "accordionstate=" + accordionState.join('|');
      }
      history.replaceState({}, document.title, newUrl);
    } else {
      history.replaceState({}, document.title, uri.replace(regex, ''));
    }
  }

  constructSptAccordionControls() {
    const accordionControls = $(this.accordion).find('.govuk-accordion__controls');
    const footerControls = $(accordionControls).clone();

    footerControls.prepend('<a href="#main-content" class="spt-back-to-top-linkage">Back to top</a>').insertAfter($(this.accordion));

    // keep the cloned accordion open all in sync with the official govuk-control
    $(footerControls).find('.govuk-accordion__open-all').on('click', function(e) {
      e.preventDefault();
      accordionControls.eq(0).find('.govuk-accordion__open-all').trigger('click');
      $(this).html(accordionControls.eq(0).find('.govuk-accordion__open-all').html());
    });

    $(accordionControls).find('.govuk-accordion__open-all').on('click', function() {
      $(footerControls).find('.govuk-accordion__open-all').html($(this).html());
    });

    $(this.accordion).find('.govuk-accordion__icon').on('click', function() {
      window.setTimeout(function() {
        $(footerControls).find('.govuk-accordion__open-all').html(accordionControls.eq(0).find('.govuk-accordion__open-all').html());
      }, 0);
    });

  }

  bindAccordionEvents() {
    const self = this;
    $(this.accordion).find('.govuk-accordion__section-button, .govuk-accordion__icon').on('click', function(){
      window.setTimeout(function () {
        self.setAccordionState();
      },100);
    });

    $(this.accordion).find('.govuk-accordion__open-all').on('click', function(){
      window.setTimeout(function () {
        self.setAccordionState();
      },100);
    });
  }

  init() {
    const initialState = this.getAccordionState();
    const self = this;

    if (initialState && $(this.accordion).is(':visible')) {
      const openPanels = initialState.split('|');
      $.each(openPanels, function (n, panelIndex) {
        $(self.accordion).find('.govuk-accordion__section').eq(panelIndex).addClass('govuk-accordion__section--expanded');
      });

      const allOpen = openPanels.length === $(this.accordion).find('.govuk-accordion__section').length;

      if (allOpen) {
        $(this.accordion).find('.govuk-accordion__open-all').text('Close all');
      }
    }

    this.constructSptAccordionControls();
    this.bindAccordionEvents();
  }
}


$.fn.giasAccordionExtensions = function() {
  return this.each(function () {
    if (!$.data(this, 'giasAccordionExtensions')) {
      $.data(this, 'giasAccordionExtensions', new GiasAccordionExtensions(this));
    }
  });
};


export default GiasAccordionExtensions;
