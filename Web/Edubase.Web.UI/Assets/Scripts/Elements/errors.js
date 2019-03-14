(function () {

    $(document).ready(function() {
        // Focusing on error summary
        if ($('.error-summary').length) {
            $('.error-summary').focus();
            $('.error-summary a').click(function (e) {
                e.preventDefault()
                var href = $(this).attr('href');
                $(href).focus();
            })
        }
    });

}());
