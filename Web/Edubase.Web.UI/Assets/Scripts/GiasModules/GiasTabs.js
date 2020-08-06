class GiasTabs {
  constructor(el, opts) {
    const defaults = {
      tabClass: 'gias-tabs__list-item',
      selectedTabClass: 'gias-tabs__list-item--selected',
      tabContentClass: 'tab-content',
      tabContentWrapper: 'tab-content-wrapper'
    };

    this.el = el;
    this.opts = $.extend({}, defaults, opts);

    this.init();
  }

  init() {
    const $el = $(this.el);
    const opts = this.opts;
    const $tabs = $el.find('.' + opts.tabClass);
    const $tabbedContent = $el.find('.' + opts.tabContentClass);

    this.validHashes = $.map($tabs, function (elem) {
      if ($(elem).find('a').attr('href').indexOf('#') === 0) {
        return $(elem).find('a').attr('href').split('#')[1];
      }
    });

    $tabbedContent.attr('tabindex', 0);
    $tabbedContent.addClass('hidden-tab-content').attr('aria-hidden', true);

    $tabbedContent.each(function (n, el) {
      const $tabContent = $(el);
      const hasPagination = $tabContent.find('.pagination').length > 0;

      if (hasPagination) {
        $tabContent.find('.pagination-links a').each(function () {
          const linkHref = $(this).prop('href');
          $(this).prop('href', linkHref + '#' + $tabContent.attr('id'));
        });
      }
    });

    this.attachEvents();
  }

  attachEvents() {
    const opts = this.opts;
    const $tabs = $(this.el).find('.' + opts.tabClass);
    const $tabbedContent = $(this.el).find('.' + opts.tabContentClass);
    const initialTabSelection = $.inArray(window.location.hash.replace('#', ''), this.validHashes);

    $('.tab-manipulator').on('click', (e)=> {
      e.preventDefault();
      const hash = $(e.target).attr('href').replace('#', '');
      const hasTab = $.inArray(hash, this.validHashes);
      if (hasTab > -1) {
        $tabs.eq(hasTab).find('a').click();
        $(window).scrollTop($tabs.offset().top);
      }
    });

    $tabs.find('a').on('click', function (e) {
      const targetContent = $(this).attr('href');
      if (targetContent.indexOf('#') !== 0) {
        return true;
      }
      e.preventDefault();
      const yScroll = window.scrollY;
      window.location.replace(targetContent);

      $tabs.removeClass(opts.selectedTabClass);

      $tabbedContent.addClass('hidden-tab-content')
        .attr('aria-hidden', true);

      $(this).parent().addClass(opts.selectedTabClass);

      $(targetContent).removeClass('hidden-tab-content')
        .attr('aria-hidden', false);
      $(targetContent)[0].focus({preventScroll:true});
      window.scroll(0, yScroll);

      $(window).trigger({ type: 'tabChange', tab: targetContent });
    });

    // only process the hash if it's for a valid
    if (window.location.hash !== '' && initialTabSelection > -1) {
      $tabs.eq(initialTabSelection).find('a').click();
    } else {
      if ($('.' + opts.selectedTabClass).length === 0) {
        $tabs.slice(0, 1).addClass(opts.selectedTabClass);
        $tabbedContent.slice(0, 1).removeClass('hidden-tab-content').attr('aria-hidden', false);
      }

    }

    setTimeout(function() {
      window.scrollTo(0, 0);
    },0);
  }
}


$.fn.giasTabs = function (opts) {
  return this.each(function () {
    if (!$.data(this, 'giasTabs')) {
      $.data(this, 'giasTabs', new GiasTabs(this, opts));
    }
  });
}
export default GiasTabs;
