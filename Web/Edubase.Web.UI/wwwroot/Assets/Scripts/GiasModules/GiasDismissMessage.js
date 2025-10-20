function giasDismissMessage() {
  const $messages = $('#main-content').find('.js-dismiss-message');

  $messages.each(function() {
    const msg = $(this);
    $(this).find('.cross').on('click', function(e) {
      e.preventDefault();
      msg.addClass('hidden');
    });
  });
}

export default giasDismissMessage;
