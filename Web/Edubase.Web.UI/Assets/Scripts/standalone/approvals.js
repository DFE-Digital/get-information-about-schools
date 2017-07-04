(function() {
    var defaults = {
        pageSize: 100,
        apiUrl: '/api/approvals/change-requests',
        confirmUrl: '/api/approvals/change-request'
        
    };
    Vue.component('changes-table',
        {
            template: '#table-template',
            props: {
                currentPage: {
                    type: Number,
                    default: 1
                },
                maxPageSize: {
                    type: Number,
                    default: 10
                },
                pages: Array
            },

            computed: {
                page: function () {
                    return this.pages;
                }
            },
            methods: {
                detailUrl: function (urn) {
                    return '/Establishment/Details/' + urn;
                },
                formatDate: function (utcDate) {
                    if (utcDate === null) {
                        return 'unknown';
                    }
                    var d = new Date(utcDate);
                    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
                }
            }
        });

    var approvalApp = new Vue({
            el: '#change-approvals',
            data: {
                changes: [],
                currentPage: 0,
                pages: [],
                initialCount: 0,
                currentCount: 0,
                startIndex: 0,
                pageSize: defaults.pageSize,
                invalidReason: false,
                approvalError: false,
                reasonLength: false,
                noneSelectedError: false,
                pendingRejection: false,
                itemsConfirmedRemoved: false,
                itemsConfirmedRejected: false,
                isProcessing: true

            },
            created: function() {
                this.getChangesData();
            },
            computed: {
                pageCount: function () {
                    return this.currentPage * this.pageSize;
                },
                paginationDescription: function () {
                    var starting = this.currentPage * this.pageSize + 1,
                        ending = this.currentPage * this.pageSize + this.pageSize;

                    ending = ending > this.currentCount ? this.currentCount : ending;

                    return starting + " - " + ending;
                },
                totalPages: function() {
                    return Math.ceil(this.currentCount / defaults.pageSize);
                }

            },
            methods: {
                getChangesData: function (skip, callback) {
                    var self = this;
                    this.isProcessing = true;
                    $('#changes-table').find(':checkbox').prop('checked', false);
                    $.ajax({
                        url: defaults.apiUrl,
                        data: {
                            take: defaults.pageSize,
                            skip: skip || 0
                        },
                        success: function (data) {
                            self.currentCount = data.count;
                            self.changes =  data.items;
                            self.isProcessing = false;
                            if (callback) {
                                callback.call(self);                                
                            }
                        },
                        error: function () {
                            console.log("Error loading changes data");
                        }
                    });
                },
                approveSuccessCallback: function () {
                    this.itemsConfirmedRemoved = true;
                },
                rejectSuccessCallBack: function() {
                    this.pendingRejection = false;
                    this.itemsConfirmedRejected = true;
                    document.getElementById('reason').value = '';
                },
                updateCount: function (removedCount) {
                    this.currentCount = this.currentCount - removedCount;
                },
                setCurrentPage: function (pageIndex) {
                    this.currentPage = pageIndex;
                    this.getChangesData(pageIndex * defaults.pageSize);

                },
                confirmRejection: function () {
                    var self = this;
                    this.invalidReason = $('#reason').val().length < 1;
                    this.reasonLength = $('#reason').val().length > 1000;

                    var selectedItems = $('#changes-table').find('.boldened-checkbox')
                        .filter(':checked');

                    this.noneSelectedError = selectedItems.length === 0;

                    if (this.invalidReason || this.noneSelectedError || this.reasonLength) {
                        return;
                    } else {
                        this.isProcessing = true;
                        var removedIds = selectedItems.map(function (n, item) {
                            return Number(item.value);
                        });
                        removedIds = $.makeArray(removedIds);
                       
                        $.ajax({
                            url: defaults.confirmUrl,
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            method: 'post',
                            data: JSON.stringify({
                                action: 'reject',
                                'ids': removedIds,
                                rejectionReason: $('#reason').val()
                            }),
                            success: function(data) {
                                self.getChangesData(0, self.rejectSuccessCallBack);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                alert('There was an error rejecting the changes - see the console for details');                              
                                console.log('textStatus== ' + textStatus + '\n\n');
                                console.log('errorThrown==' + errorThrown + '\n\n');
                                console.log(jqXHR.responseText + '\n\n');
                            }
                        });                       
                    }
                },
                approveSelection: function () {
                    var self = this;
                    var selectedItems = $('#changes-table').find('.boldened-checkbox')
                        .filter(':checked');

                    this.approvalError = (selectedItems.length === 0);
                    this.itemsConfirmedRejected = false;
                    this.itemsConfirmedRemoved = false;

                    if (this.approvalError) {
                        return;
                    } else {
                        this.isProcessing = true;
                        var removedIds = selectedItems.map(function (n, item) {
                            return Number(item.value);
                        });

                        removedIds = $.makeArray(removedIds);

                        $.ajax({
                            url: defaults.confirmUrl,
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            method: 'post',
                            data: JSON.stringify({
                                action: 'approve',
                                'ids': removedIds                                
                            }),
                            success: function (data) {
                                self.getChangesData(0, self.approveSuccessCallback);
                                
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                alert('There was an error approving the changes - see the console for details');
                                console.log('textStatus== '+textStatus);
                                console.log('errorThrown=='+ errorThrown);
                                console.log(jqXHR);

                            }
                        });                      
                    }
                }
            }
        });

}());