DfE.Views.editIebt = {
    radios: $('#proprietor-radios').find('input'),
    cloneFields: $('#cloneable-fields-container').find('.cloneable').detach(),
    okClick: function() {
        this.closeModal();
        $('#proprietor-single, #proprietor-body').find('.form-control').val('');
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
        var radioValue;
       // var popOverInitialised = false;

        if (self.radios.length === 0) {
            return;
        }

        console.log('init');
        
        radioValue = self.radios.filter(':checked').val();

        if (typeof radioValue === 'undefined') {
            $('#proprietor-type-single').prop('checked', true);
            radioValue = self.radios.filter(':checked').val();
        }


        if (radioValue === 'body') {
            $('#field-clone-target').append(self.cloneFields);
        } else {
            $('#cloneable-fields-container').append(self.cloneFields);
        }

        $(window).on('radioChange', function (e) {
            var radioId = $(e.selectedRadio).prop('id');

            if (radioId === 'proprietor-type-body') {
                $('#field-clone-target').append(self.cloneFields);
                $('#proprietor-radios').find('input').data().okCancel
                    .updateModalContent('Are you sure you want to change to a proprietary body?',
                        'All single proprietor changes will be lost');

               
            } else {
                $('#cloneable-fields-container').append(self.cloneFields);
                 $('#proprietor-radios').find('input').data().okCancel
                    .updateModalContent('Are you sure you want to change to a single proprietor?',
                        'All proprietary body changes will be lost');
            }

        });

        $('#proprietor-body, #proprietor-single').on('change', '.form-control', function() {
            console.log('CCCCCHHHHanges');
                self.radios.okCancel({
                    ok: self.okClick,
                    cancel: self.cancelClick
                });

                $('#proprietor-body, #proprietor-single').off('change', '.form-control');
            
        });

        

    }
    
};

DfE.Views.editIebt.init();