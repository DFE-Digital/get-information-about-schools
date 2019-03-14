(function () {

    $(document).ready(function() {
        // Focusing on error summary
        if ($('.error-summary').length) {
            $('.error-summary').focus();
            $('.error-summary a').click(function (e) {
                var href = $(this).attr('href');
                var targetEl = $(href);
                if (targetEl.is('input') || targetEl.is('textarea') || targetEl.is('select')) {
                    e.preventDefault()
                    targetEl.focus();
                }
            })
        }
    });

}());
