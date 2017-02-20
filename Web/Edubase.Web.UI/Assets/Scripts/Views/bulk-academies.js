var stub = stub || [];
(function ($) {
    'use strict';
    var defaults = {
        data: stub,
        listParent: '#academies-bulk-list',
        searchTrigger: '#find-urn',
        addTrigger: '#add-item-to-list',
        submitChanges: '#go-create'

    },
    addedEstablishments = [],
    selectedCount = 0,
    selectedEstablishmentTemplate = '<ul class="selected-establishment"><li class="estab-detail" data-urn="{0}">{0} - {1}</li><li>Establishment type: {2}</li><li>Opening date: {3}</li></ul>',
    buttonTemplate = '<div class="button-row"><button class="button button-grey remove-item">Remove</button><button class="button button-grey edit-item">Edit</button></div>';


    function BulkAcademies(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();
    }


    BulkAcademies.prototype = {
        init: function () {
            var opts = this.opts;
            var self = this;

            // expose stuib URNs to UI
            var urns = $.map(opts.data, function (item) {
                return item.URN;
            });
            
            $('.help-with-this-tool').html(urns.join(',<br>'));

            $('.heading-intro').click(function() {
                $('.help-with-this-tool').toggleClass('hidden');
            })

            // check a URN
            $(opts.searchTrigger).on('click', function (e) {
                e.preventDefault();
                var userURN = $('#establishment-urn').val();

                var validURN = $(opts.data).filter(function(n, estab) {
                    if (estab.URN === userURN) {
                        return estab;
                    } else {
                        return false;
                    }                    
                });

                if (!validURN.length) {
                    self.showErrors('.invalid-urn');
                } else {
                    self.hideErrors();
                    self.showEstablishmentDetails(validURN);
                    self.selectedEstablishment = validURN[0];
                }
            });

            // add Selected URN to list
            $(opts.addTrigger).on('click', function(e) {
                e.preventDefault();
                if ($('#opendate-day').val() === '' ||
                    $('#opendate-month').val() === '' ||
                    $('#opendate-year').val() === '') {
                    self.showErrors('.missing-date');

                } else {
                    self.hideErrors();
                    self.addResult();
                }
            });

            // remove a previously added item
            $(self.el).on('click', '.remove-item', function (e) {
                e.preventDefault();
                $(this).parents('li').remove();

                selectedCount--;
                $('.academy-count').text(selectedCount);
            });

            // edit a previously added item
            $(self.el).on('click', '.edit-item', function (e) {
                e.preventDefault();
                var urn = $(this).parents('li').find('.estab-detail').data().urn;

                $(this).parents('li').remove();

                selectedCount--;
                $('.academy-count').text(selectedCount);

                $('#establishment-urn').val(urn);
                $('#find-urn').click();
            });

            // commit changes
            $(opts.submitChanges).on('click', function(e) {
                e.preventDefault();

                if (selectedCount === 0) {
                    self.showErrors('.invalid-urn');
                    return;
                }
                $('#bulk-acamedmy-create, .progress-indicator').toggleClass('hidden');

                $('.result-list').find('.estab-detail').each(function() {
                    var text = $(this).text();
                    $(this).html('<a href="#">' + text + '</a>');
                });

                window.setTimeout(function() {
                    $('.bulk-create-result, .progress-indicator').toggleClass('hidden');
                }, 2500);
            });
        },
        showEstablishmentDetails: function (estab) {
            $('.establishment-name').text(estab[0].establishmentName);
            $('.establishment-found').removeClass('hidden');
        },

        hideErrors: function () {
            $('.error-summary, .error-sub-heading, .error-message, .error-summary-heading').addClass('hidden');
            $('.form-group').removeClass('error');

            return true;
        },
        showErrors: function (errorClass) {
            var self = this;

            $.when(self.hideErrors()).then(function() {
            
                $(errorClass).removeClass('hidden');
                $('.error-summary').removeClass('hidden');

                if (errorClass === '.invalid-urn') {
                    $('.error-on-urn').addClass('error');

                } else if (errorClass === '.missing-date') {
                    $('.error-on-open-date').addClass('error');

                } 
           });
        },
        addResult: function () {
            var estab = this.selectedEstablishment;

            $('.establishment-found').addClass('hidden');

            selectedCount++;
            $('.academy-count').text(selectedCount);

            function makeDateString() {
                return [
                    $('#opendate-day').val() , 
                    $('#opendate-month').val() ,
                    $('#opendate-year').val() 
                 ].join('/');
            }

            var selectedEstablishment = selectedEstablishmentTemplate
                .replace(/\{0\}/g, estab.URN)
                .replace('{1}', estab.establishmentName)
                .replace('{2}', $('#academy-type').val())
                .replace('{3}', makeDateString());

            $('.academies-bulk-list').append('<li>' + selectedEstablishment + buttonTemplate + '</li>');
        }
    }


    $.fn.bulkAcademies = function (opts) {
        return this.each(function () {
            if (!$.data(this, 'bulkAcademies')) {
                $.data(this, 'bulkAcademies', new BulkAcademies(this, opts));
            }
        });
    }
    
}($));


$('#bulk-acamedmy-create').bulkAcademies();