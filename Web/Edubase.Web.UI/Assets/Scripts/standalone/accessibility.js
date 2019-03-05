(function (){
    $(document).ready(function(){
        // Re-inserting text into notification/banner to trigger aria-live
        var $bannerContent = $('.js-trigger-aria-live > .message-text');
        $bannerContent.html($bannerContent.text());
    });
}());

