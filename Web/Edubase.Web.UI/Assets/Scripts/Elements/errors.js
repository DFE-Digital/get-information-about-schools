(function () {

    var $errorSummary = $('.error-summary');

    $(document).ready(function() {
        // Focusing on error summary
        if ($errorSummary.length > 0) {
            $errorSummary.focus();
        }
    });

}());
