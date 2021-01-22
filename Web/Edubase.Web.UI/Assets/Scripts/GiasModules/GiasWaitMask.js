class GiasWaitMask {
  constructor(opts) {
    this.opts = opts;
    this.init();
  }

  init() {
    $('#main-content').find('.gias-wait-mask-trigger').one('click', function(e){
      $(this).addClass('js-allow-exit');
      e.preventDefault();
      $('html').addClass('no-scroll');
      $('#gias-content-mask').removeClass('hidden');
      const $btn = $(this);

      window.setTimeout(function(){
        $btn.click();
      },10);
    });

  }
}


export default GiasWaitMask;
