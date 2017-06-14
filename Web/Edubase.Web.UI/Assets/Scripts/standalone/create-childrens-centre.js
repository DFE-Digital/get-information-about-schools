(function () {


    var ccGroup = new Vue({
        el: '#create-childrens-centre',
        data: {
            groupType: '9',
            groupName: '',
            groupStatus: 'Open',
            openDateDay: '',
            openDateMonth: '',
            openDateYear: '',
            la: '',

            step0Complete: false,
            step1Complete: false,

            openDateValidateUrl: '/Groups/Group/CreateChildrensCentre/Validate/JoinedDate',
            openDateError: false,
            openDateErrorMessage: '',
            urnLookupUrl: ''


        },
        computed: {
            openDate: function () {
                if (this.openDateDay !== '' && this.openDateMonth !== '' && this.openDateYear !== '') {
                    return [this.openDateDay, this.openDateMonth, this.openDateYear].join('/');
                }
                return '';

            },
            laName: function () {
                return $('#LocalAuthorityId').find('option[value="' + this.la + '"]').text();
            }
        },
        methods: {
            validateJoinDate: function () {
                var self = this;
                self.openDateError = false;

                $.ajax({
                    url: self.openDateValidateUrl,
                    method: 'post',
                    dataType: 'json',
                    data: {
                        Day: self.openDateDay,
                        Month: self.openDateMonth,
                        Year: self.openDateYear,
                        Label: ""
                    },
                    success: function (data) {
                        self.openDateError = data.length > 0;
                        var errors = [];
                        $.each(data, function (n, err) {
                            errors.push(err.Value.Errors[0].ErrorMessage);
                        });

                        self.openDateErrorMessage = errors.join('<br>');

                    }
                });
            },
            step0Continue: function () {
                this.step0Complete = true;
            },
            step1Continue: function () {
                this.validateJoinDate();
            }

        }
    });

}());