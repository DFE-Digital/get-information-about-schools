/**
 *  additionalClasses {string}
 *      classnames to add to the modal
 *
 *  headingSize {string}
 *      any govuk heading size- s, m, l, xl
 *
 *  immediate {bool}
 *      setting this to true disables the open click attachment
 *      you need to call 'open' yourself
 *
 *  remoteContent {bool}
 *      ajax call to obtain the modal content
 *
 *  remoteUrl {string}
 *      URL for the ajax call
 *
 *  contentSelector {string}
 *      a valid css selector for the parent element of the modal content in the ajax response
 *
 *  closeButtonClass {string}
 *      a valid css selector for elements you want to make additional close buttons
 *
 * usage:
 * DEFAULTS - expects the modal content to be hidden in the page
 * identified by a local page anchor in the href of the link
 *
   $('#main-content').find('.modal-link').each(function(n, el) {
      new GiasModal(el);
    });

 * OPEN THE MODAL ONLOAD WITH CONTENT RETRIEVED VIA AJAX

 const modal = new GiasModal(document.body, {
      remoteContent: true,
      remoteUrl: '/path/to/your/response,
      immediate: true,
      contentSelector: '#data-status-summary',
    });


 */
const defaults = {
  additionalClasses: '',
  headingSize: 'm',
  immediate: false,
  remoteContent: false,
  remoteUrl: null,
  contentSelector: null,
  closeButtonClass: null,
  triggerEvent: 'click',
};

class GiasModal {
  constructor(el, opts) {
    this.el = el;
    const metaData = $(this.el).data();
    this.opts = $.extend({}, defaults, metaData, opts);

    this.init();
  }

  init() {
    if (!document.getElementById('app')) {
      const $body = $('body');
      $body.wrapInner('<div id="app"></div>');
    }

    if (!this.opts.immediate) {
      $(this.el).on(this.opts.triggerEvent, (e) => {
        e.preventDefault();
        this.openModal();
      });
    } else {
      this.openModal();
    }
  }

  openModal() {
    const self = this;
    const opts = this.opts;
    const contentTarget = $(this.el).attr('href');
    const $appContainer = $('#app');
    let modalContent;

    function prepareContent() {
      const contentTitle = opts.title ? opts.title :
          modalContent.find('.make-modal-header').html();

      let bodyContent = opts.content ? $('<div><p>'+ opts.content+ '</p></div>') :
          $('<div/>').append(modalContent.clone(true).children().not('.make-modal-header'));

      bodyContent = bodyContent.html();

      if (typeof opts.ok === 'function' && typeof opts.cancel === 'function') {
        bodyContent += `<div class="button-row">
                            <a id="button-ok" class="govuk-button js-allow-exit" href="#">${opts.okLabel}</a>
                            <a href="#" class="govuk-button govuk-button--secondary js-allow-exit" id="button-cancel">Cancel</a>
                        </div>`;
      } else if (typeof opts.ok === 'function') {
        bodyContent += `<div class="button-row">
                            <a id="button-ok" class="govuk-button js-allow-exit" href="#">${opts.okLabel}</a>
                        </div>`;
      }

      const modal = `
        <div id="gias-modal" open class="gias-modal ${opts.additionalClasses}" aria-labelledby="gias-modal-title" role="dialog">
            <div role="document">
                <div class="gias-modal__close-container">
                    <a href="#" role="button" id="gias-modal-close" class="gias-modal__close-button">Close</a>
                </div>
                <div class="gias-modal__content-wrapper" id="gias-modal-content-wrapper">
                <h1 id="gias-modal-title" class="govuk-heading-${opts.headingSize} gias-modal__title">${contentTitle}</h1>
                    <div class="gias-modal__content" id="gias-modal-content">
                      ${bodyContent.toString()}
                    </div>
                </div>
            </div>
        </div>`;

      const modalOverlay = `
      <div class="gias-modal__overlay" id="gias-modal-overlay" title="Close modal">
        <div class="govuk-visually-hidden">Dismiss modal and return to page</div>
      </div>`;

      insertModal(modal, modalOverlay);
    }

    function insertModal(modal, modalOverlay) {
      $('html').addClass('no-scroll');
      $appContainer.attr('aria-hidden', true);

      $(modalOverlay).insertAfter($appContainer);
      $(modal).insertAfter($appContainer);

      $('#gias-modal-close').focus();

      $('body').on('keydown', '#gias-modal', (e) => {
        self.manageModalKeyPress(e);
      });

      $('#gias-modal-close, #gias-modal-overlay').on('click', (e) => {
        e.preventDefault();
        self.closeModal();
      });

      if (opts.closeButtonClass) {
        $('#gias-modal').find(opts.closeButtonClass).on('click', (e) => {
          e.preventDefault();
          self.closeModal();
        });
      }

      $(document).on('click', '#button-ok', function (e) {
          e.preventDefault();
          opts.ok.call(self);
          self.closeModal();
        });

      $(document).on('click', '#button-cancel', function (e) {
        e.preventDefault();
        opts.cancel.call(self);
        self.closeModal();
      });

      document.getElementById('gias-modal-content-wrapper').addEventListener('touchstart', function () {
        const $appModal = $('#gias-modal');
        const top = $appModal.scrollTop;
        const totalScroll = $appModal.scrollHeight;
        const currentScroll = top + $appModal.offsetHeight;
        if (top === 0) {
          $('#gias-modal').scrollTop = 1;
        } else if (currentScroll === totalScroll) {
          $('#gias-modal').scrollTop = top - 1;
        }
      });

      $(window).trigger({
        type: 'modal:opened',
        element: self.el
      });
    }

    function setModalContent() {
        return $.ajax({
          url: opts.remoteUrl,
          dataType: 'html',
          success: function (data) {
            modalContent = $(data).filter(opts.contentSelector);
          },
          error: function () {
            console.error('Error retrieving data status panel');
          }
        });
    }

    if (opts.remoteContent) {
      $.when(setModalContent()).then(function() {
        prepareContent();
      });
    } else if (typeof opts.content !== 'undefined') { // okCancel...
      prepareContent();
    } else {
      modalContent = $(contentTarget);
      prepareContent();
    }
  }

  closeModal() {
    const $body = $('body');
    const $appContainer = $('#app');
    $('html').removeClass('no-scroll');
    $appContainer.removeAttr('aria-hidden');

    $body.off('keydown', '#gias-modal');

    $('#gias-modal-overlay').off('click').remove();
    $('#gias-modal').remove();

    this.el.focus();
    $(window).trigger('modal:closed');
  }

  manageModalKeyPress(e) {
    if (e.keyCode === 27) { // esc
      this.closeModal();
      return false;
    }
    if (e.keyCode === 9) { // tab or maj+tab
      const focusableItems = $('#gias-modal').find('a[href], area[href], input:not([disabled]), button:not([disabled])').filter(':visible');
      const focusableItemsCount = focusableItems.length;

      const focusedItem = $(document.activeElement);

      const focusedItemIndex = focusableItems.index(focusedItem);

      if (!e.shiftKey && (focusedItemIndex === focusableItemsCount - 1)) {
        focusableItems.get(0).focus();
        e.preventDefault();
      }
      if (e.shiftKey && focusedItemIndex === 0) {
        focusableItems.get(focusableItemsCount - 1).focus();
        e.preventDefault();
      }
    }
  }
}


export default GiasModal;

