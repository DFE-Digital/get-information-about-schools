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
		  
		  // we would normally juts base this on offset top, but for the date inputs which are on the same row, we also need to check offset left
		  if ($('#' + $bhref.substring(1)).offset().top < $('#' + $ahref.substring(1)).offset().top) {
			  return 1;
		  }
		  if ($('#' + $bhref.substring(1)).offset().top > $('#' + $ahref.substring(1)).offset().top) {
			  return -1;
		  }

          return ($('#' + $bhref.substring(1)).offset().left < $('#' + $ahref.substring(1)).offset().left ? 1 : -1);
        }
    });
}());