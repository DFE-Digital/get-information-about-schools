DfE.Views.editIebt = {
    radios: $('#proprietor-radios').find('input'),

    cloneFields: $('#cloneable-fields-container').find('.cloneable').detach(),

    okClick: function () {
        this.closeModal();
        $('#proprietor-single, #proprietor-body').find('.form-control').val('');

        $('#proprietor-radios').find('input').each(function() {
            $(this).data().okCancel.pause(true);
        });

       
        $('#proprietor-body, #proprietor-single').on('change, keydown', '.form-control', function () {

            $('#proprietor-radios').find('input').each(function () {
                $(this).data().okCancel.pause();
            });

            $('#proprietor-body, #proprietor-single').off('change, keydown', '.form-control');
        });

        return true;
    },
    cancelClick: function() {
        this.closeModal();
        var selectedVal = $('#proprietor-radios').find('input:checked').val();

        if (selectedVal === 'single') {
            $('#proprietor-type-body').prop('checked', true).change();

        } else if (selectedVal === 'body') {
            $('#proprietor-type-single').prop('checked', true).change();
        }

    },
    init: function () {
        var self = this;
        var overlayAttached = false;
        if (self.radios.length === 0) {
            return;
        }
    
        var radioValue = typeof self.radios.filter(':checked').val() !== 'undefined' ? self.radios.filter(':checked').val() : 'single';

        if (radioValue === 'body') {
            $('#field-clone-target').append(self.cloneFields);
        } else {
            $('#cloneable-fields-container').append(self.cloneFields);
            $('#proprietor-type-single').prop('checked', true);
            $('#proprietor-single').removeClass('hidden');
        }

        self.radios.on('change', function (e) {
            var radioId = $(this).prop('id');
            
            if (radioId === 'proprietor-type-body') {
                $('#field-clone-target').append(self.cloneFields);

                if (overlayAttached) {
                     $('#proprietor-radios').find('input').data().okCancel
                    .updateModalContent('Are you sure you want to change to a proprietary body?',
                        'All single proprietor changes will be lost');
                }               
               
            } else {
                $('#cloneable-fields-container').append(self.cloneFields);

                if (overlayAttached) {
                    $('#proprietor-radios').find('input').data().okCancel
                        .updateModalContent('Are you sure you want to change to a single proprietor?',
                            'All proprietary body changes will be lost');
                }
            }

        });


        $('#proprietor-body, #proprietor-single').on('change, keydown', '.form-control', function() {
            if (!overlayAttached) {
                overlayAttached = true;                
                self.radios.okCancel({
                    ok: self.okClick,
                    cancel: self.cancelClick,
                    idPrefix: 'iebt-'
                });

            }
            $('#proprietor-body, #proprietor-single').off('change, keydown', '.form-control');
            
        });
    }    
};

if (DfE.Views.editIebt.radios.length) {
    DfE.Views.editIebt.init();
}
