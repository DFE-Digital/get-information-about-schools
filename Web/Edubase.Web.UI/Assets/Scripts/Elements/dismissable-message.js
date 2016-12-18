(function () {

    var $messages = $('#content').find('.dismissable-message');

    $messages.each(function () {
        var msg = $(this);
        $(this).find('.cross').on('click', function(e) {
            e.preventDefault();
            msg.addClass('hidden');
        });
    });

}());

