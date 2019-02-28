(function (){
    $(document).ready(function(){
        // Re-inserting text into warning banner to trigger aria-live
        var $bannerContent = $('.js-trigger-aria-live > .warning-message-text');
        $bannerContent.html($bannerContent.text());
    });
}());

