(function () {

    var $messages = $('#content').find('.js-dismiss-message');

    $messages.each(function () {
        var msg = $(this);
        $(this).find('.cross').on('click', function(e) {
            e.preventDefault();
            msg.addClass('hidden');
        });
    });

}());

