(function() {
    var defaults = {
        pageSize: 100,
        apiUrl: '/api/approvals/change-requests',
        confirmUrl: '/api/approvals/change-request'

    };

    var approvalApp = new Vue({
            el: '#change-approvals',
            data: {
                changes: [],
                reason: '',
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
                approvalMessage: '',
                rejectionMessage: '',
                isProcessing: true,
                apiError: '',
                apiBork: {},
                sortAscending: true,
                sortType: 'effectiveDateUtc',
                cannedRejections: window.GIASRejections || [],
                showRejections: false,
                noReasonSelectedError: false,
                reasonIds: [],
                noItemsSelected: true,
                tableColumns: {
                  establishmentUrn: 'URN',
                  establishmentLAESTAB: 'DfE Number',
                  establishmentName: 'Establishment',
                  fieldName: 'Updated field',
                  oldValue: 'Old value',
                  newValue: 'New value',
                  effectiveDateUtc: 'Effective date',
                  requestedDateUtc: 'Date requested',
                  originatorFullName: 'Suggested by',
                }
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
                    return Math.ceil(this.currentCount / this.pageSize);
                },
                isZeroItemsSelected: function () {
                    console.log("isZeroItemsSelected");
                    return $('#changes-table').find('.boldened-checkbox').filter(':checked').length == 0;
                }
            },
            methods: {
                showRejectionsModal: function () {
                    this.showRejections = true;
                    $(document).ready(function() {
                        if ($('#modal-label').length > 0) {
                            $('#modal-label').focus();
                        }
                    });
                },
                selectItem: function () {
                    this.noItemsSelected = ($('#changes-table').find('.boldened-checkbox').filter(':checked').length == 0);
                },
                selectReason: function () {
                    var reasonText = this.reason;
                    var self = this;
                    this.noReasonSelectedError = false;

                    if (this.reasonIds.length === 0) {
                        return this.noReasonSelectedError = true;
                    }
                    if (reasonText.length > 0) {
                        reasonText = reasonText + '\n\n';
                    }

                    for (var i = 0, len = this.reasonIds.length; i < len; i++) {
                        var reason = this.cannedRejections.filter(function(r) {
                            if (r.id === Number(self.reasonIds[i])) {
                                return r;
                            }
                        })[0];

                        reasonText += reason.title + '\n' + reason.content + '\n\n';

                    }
                    this.reason = reasonText;
                    this.reasonIds = [];
                    this.showRejections = false;
                    $('#reason').focus().trigger('drop');
                },
                setSort: function(sort) {
                    if (sort === this.sortType) {
                        this.sortAscending = !this.sortAscending;
                    } else {
                        this.sortType = sort;
                    }

                    this.getChangesData();
                },
                detailUrl: function (urn) {
                    return '/Establishments/Establishment/Details/' + urn;
                },
                formatDate: function (utcDate) {
                    if (utcDate === null) {
                        return 'unknown';
                    }
                    var d = new Date(utcDate);
                    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
                },
                getChangesData: function (skip, callback) {
                    var self = this;
                    this.isProcessing = true;
                    var sortDir = this.sortAscending ? '-asc' : '-desc';
                    $('#changes-table').find(':checkbox').prop('checked', false);

                    $.ajax({
                        url: defaults.apiUrl,
                        data: {
                            take: this.pageSize,
                            skip: skip || 0,
                            sortBy: this.sortType + sortDir // Jon: put `th.data-sortkey` value into here and concat '-asc' or '-desc'
                        },
                        success: function (data) {
                            self.currentCount = data.count;
                            self.changes =  data.items;
                            self.isProcessing = false;
                            if (callback) {
                                callback.call(self);
                            }
                        },
                        error: function (jqxhr) {
                            if (jqxhr.hasOwnProperty('responseJSON')) {
                                self.apiBork = jqxhr.responseJSON;
                            }
                        }
                    });
                },
                approveSuccessCallback: function () {
                    this.itemsConfirmedRemoved = true;
                    this.approvalMessage = 'Items approved. The editor has been notified by email.'
                },
                rejectSuccessCallBack: function() {
                    this.pendingRejection = false;
                    this.itemsConfirmedRejected = true;
                    this.rejectionMessage = 'Items rejected. The editor has been notified by email.';
                    this.reason = '';
                    window.setTimeout(function() {
                        $("#reason").data().textCount.setCount();
                    },0);

                },
                updateCount: function (removedCount) {
                    this.currentCount = this.currentCount - removedCount;
                },
                setCurrentPage: function (pageIndex) {
                    this.currentPage = pageIndex;
                    this.getChangesData(pageIndex * this.pageSize);
                },
                showAll: function () {
                    this.currentPage = 0;
                    this.pageSize = 100000;
                    this.getChangesData();
                },
                confirmRejection: function () {
                    var self = this;
                    this.apiError = '';
                    this.invalidReason = $('#reason').val().length < 1;
                    this.reasonLength = $('#reason').val().length > 1024;

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
                                var responses = JSON.parse(jqXHR.responseText);
                                var messages = [];
                                for (var i = 0, len = responses.length; i < len; i++) {
                                    messages.push(responses[i].message);
                                }
                                self.apiError = messages.join('<br>');
                                self.isProcessing = false;

                                if (jqXHR.hasOwnProperty('responseJSON')) {
                                    self.apiBork = jqXHR.responseJSON;
                                }
                            }
                        });
                    }
                },
                approveSelection: function () {
                    var self = this;
                    this.apiError = '';
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
                                var responses = JSON.parse(jqXHR.responseText);
                                var messages =[];
                                for (var i = 0, len = responses.length; i < len; i++) {
                                    messages.push(responses[i].message);
                                }
                                self.apiError = messages.join('<br>');
                                self.isProcessing = false;

                                if (jqXHR.hasOwnProperty('responseJSON')) {
                                    self.apiBork = jqXHR.responseJSON;
                                }
                            }
                        });
                    }
                }
            }
        });


    $('#reason').textCount({ maxLength: 1024 });
}());
