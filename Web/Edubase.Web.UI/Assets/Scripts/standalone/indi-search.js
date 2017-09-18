(function() {

    var filterForm = $('#indi-filter-form');
    var resultsContainer = $('#results-container');
    var filterIntent;
    var downloadLink = $('.download-link');
    var downloadBaseUrl = '/independent-schools/download?';

    var getResults = function() {
        $.ajax({
            url: '/independent-schools/results-js',
            data: filterForm.serialize(),
            success: function (data) {
                resultsContainer.html(data);
                downloadLink.attr('href', downloadBaseUrl + filterForm.serialize());
            }
        });
    }

    filterForm.find('.trigger-result-update').on('change', function() {
        window.clearTimeout(filterIntent);
        filterIntent = window.setTimeout(function() {
            getResults();
        }, 1200);
        
    });

    filterForm.find('.form-control').on('focus', function() {
        window.clearTimeout(filterIntent);
    });

    filterForm.find('.filter-button').on('click', function(e) {
        e.preventDefault();
        window.clearTimeout(filterIntent);

        getResults();
    });
    

}());
