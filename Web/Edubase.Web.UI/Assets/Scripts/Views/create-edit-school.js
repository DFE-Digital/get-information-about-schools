DfE = window.DfE || {};

DfE.Views.createEdit = (function() {

    $('.help-icon').on('click', function(e) {
        e.preventDefault();
        $(this).next('.help-text').toggle();
    });

}());