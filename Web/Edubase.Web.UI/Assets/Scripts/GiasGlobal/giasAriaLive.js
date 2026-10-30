const giasAriaLive = function(){
  // Re-inserting text into notification/banner to trigger aria-live
  let $bannerContent = $('.js-trigger-aria-live > .message-text');
  $bannerContent.text($bannerContent.text());
};

export default giasAriaLive;
