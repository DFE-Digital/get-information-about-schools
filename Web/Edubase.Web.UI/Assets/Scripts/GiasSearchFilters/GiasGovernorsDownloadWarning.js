import GiasOkCancel from "../GiasModules/GiasModals/GiasOkCancel";

class GiasGovernorsDownloadWarning{
  constructor() {
    this.$trigger = $('#main-content').find('.search-results-download-link')
    this.init();
  }

  attachOkCancel() {
    this.$trigger.okCancel({
      ok: function(){
        this.closeModal();
      },
      cancel: null,
      title: 'Too many records',
      content: 'Please filter your search to fewer than 5000 governors. If you require a list of all governors you can download a full list from the <a href="/Downloads">downloads</a> page.',
      triggerEvent: 'click'
    });
  }

  init(){
    const self = this;
    if (this.$trigger.hasClass('prevent-download')) {
      this.attachOkCancel();
    }

    $(window).on('ajaxResultLoad', function (e) {
      if (e.count <= 5000 && typeof self.$trigger.data().okCancel !== 'undefined') {
        self.$trigger.data().okCancel.unbind();
      }
      else if (e.count > 5000 && !self.$trigger.data().hasOwnProperty('okCancel')) {
        self.attachOkCancel();
      }
    });
  }
}

export default GiasGovernorsDownloadWarning;
