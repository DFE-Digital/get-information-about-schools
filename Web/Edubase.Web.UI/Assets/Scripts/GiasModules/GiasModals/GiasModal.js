const defaults = {
  additionalClasses: '',
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

    $(this.el).on('click', (e) => {
      e.preventDefault();
      this.openModal();
    });
  }

  openModal() {
    const opts = this.opts;
    const contentTarget = $(this.el).attr('href');
    const modalContent = $(contentTarget);
    const $appContainer = $('#app');
    const contentTitle = modalContent.find('.make-modal-header').html();

    let bodyContent = $('<div/>').append(modalContent.clone().children().not('.make-modal-header'));

    bodyContent = bodyContent.html();

    const modal = `
        <dialog id="gias-modal" open class="gias-modal ${opts.additionalClasses}" aria-labelledby="gias-modal-title" role="dialog">
            <div role="document">
                <div class="gias-modal__close-container">
                    <a href="#" role="button" id="gias-modal-close" class="gias-modal__close-button">Close</a>
                </div>
                <div class="gias-modal__content-wrapper" id="gias-modal-content-wrapper">
                <h1 id="gias-modal-title" class="govuk-heading-m gias-modal__title">${contentTitle}</h1>
                    <div class="gias-modal__content" id="gias-modal-content">
                        ${bodyContent.toString()}
                      </div>
                </div>
            </div>
        </dialog>`;

    const modalOverlay = `
      <div class="gias-modal__overlay" id="gias-modal-overlay" title="Close modal">
        <div class="govuk-visually-hidden">Dismiss modal and return to page</div>
      </div>`;

    $('html').addClass('no-scroll');
    $appContainer.attr('aria-hidden', true);

    $(modalOverlay).insertAfter($appContainer);
    $(modal).insertAfter($appContainer);

    $('#gias-modal-close').focus();

    $('body').on('keydown', '#gias-modal', (e)=> {
      this.manageModalKeyPress(e);
    });

    $('#gias-modal-close, #gias-modal-overlay').on('click', (e)=> {
      e.preventDefault();
      this.closeModal();
    });

    document.getElementById('gias-modal-content-wrapper').addEventListener('touchstart', function(){
      const $appModal = $('#gias-modal');
      const top = $appModal.scrollTop;
      const totalScroll = $appModal.scrollHeight;
      const currentScroll = top + $appModal.offsetHeight ;
      if(top === 0) {
        $('#js-modal').scrollTop = 1;
      } else if(currentScroll === totalScroll) {
        $('#js-modal').scrollTop = top - 1;
      }
    });

    $(window).trigger({
      type: 'modal:opened',
      element: this.el
    });
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

