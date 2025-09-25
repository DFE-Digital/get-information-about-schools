$(function() {
  var contentSelector = "#academy-openings-content";
  function load(url) {
    $.get(url, function(html) {
      var $html = $(html);
      var $newContent = $html.find(contentSelector);
      var $existingContent = $(contentSelector);

      if ($newContent.length && $existingContent.length) {
        $existingContent.replaceWith($newContent);
        bind();

        if (window.history && window.history.replaceState) {
          window.history.replaceState(null, document.title, url);
        }
      } else {
        window.location.href = url;
      }
    });
  }
  function bind() {
    $(contentSelector)
      .find('.govuk-table th a, .pagination a')
      .off('click')
      .on('click', function(e) {
        e.preventDefault();
        load(this.href);
    });
  }
  bind();
});
