(function (){
    $(document).ready(function () {
        // Re-order the items in the validation summary, to be the same as the fields on the screen
        $('div[class="govuk-error-summary__body"] ul li').sort(sort_validation_summary).appendTo('div[class="govuk-error-summary__body"] ul');

        function sort_validation_summary(a, b) {
          var $ahref = $(a).find('a').attr('href');
          var $bhref = $(b).find('a').attr('href');

          if (!$ahref.startsWith("#")) {
            return -1;
          }
          if (!$bhref.startsWith("#")) {
            return 1;
          }
          return ($('#' + $bhref.substring(1)).offset().top < $('#' + $ahref.substring(1)).offset().top ? 1 : -1);
        }
    });
}());