// get the table data from the page and sort
// when the table header is clicked upon
// table headers require `data-sort-key`
// assume all fields are sortable

(function ($) {
    "use strict";
    window.DfE = window.DfE || {};

    var defaults = {
        triggerClassName: '.js-sort-header',
        sortDirection: 'asc',
        initialSortKey: 'name'
    };


    function ClientSort(el, opts) {
        this.el = el;
        this.opts = $.extend({}, defaults, opts);

        this.init();

    }


    ClientSort.prototype = {
        init: function() {
            var $el = $(this.el);
            var opts = this.opts;
            var asc = opts.sortDirection === 'asc';
            var self = this;
            var sortKey;

            var $triggers = $el.find(opts.triggerClassName);
            var $tbody = $el.find('tbody');

            var sortData = [];

            if ($tbody.find('tr').length < 2) {
                $el.addClass('unsortable');
                $el.removeClass('sortable-table');
                $triggers.removeAttr('href');
                return;
            }
            var selectedTrigger = $triggers.filter(function(n, ele) {
                return $(ele).data().sortKey === opts.initialSortKey;
            });



            $tbody.find('tr').each(function () {
                var $row = $(this);
                var temp = {};
                $row.find('td').each(function (n) {
                    var key = $triggers.eq(n).data().sortKey;
                    var value = $(this).text();
                    temp[key] = value;
                });
                sortData.push(temp);
            });

            this.sortData = sortData;


            function dateFromString(dateString) {
                var dateParts = dateString.split('/');
                if (dateParts.length < 3) { // probably says not recorded or something similar
                    return new Date(1900, 0, 1);
                }
                return new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
            }


            function sortDate (a, b) {
                if (asc) {
                    return dateFromString(a[sortKey]) - dateFromString(b[sortKey]);
                } else {
                    return dateFromString(b[sortKey]) - dateFromString(a[sortKey]);
                }

            }

            function sortNumeric(a, b) {
                if (asc) {
                    return Number(a[sortKey]) - Number(b[sortKey]);
                } else {
                    return Number(b[sortKey]) - Number(a[sortKey]);
                }

            }

            function sortText (a, b) {
                var textA = (a[sortKey] !== null) ? a[sortKey].toLowerCase() : '';
                var textB = (b[sortKey] !== null) ? b[sortKey].toLowerCase() : '';
                if (asc) {
                    if (textA < textB) {
                        return -1;
                    }
                    if (textA > textB) {
                        return 1;
                    }
                    return 0;
                } else {
                    if (textA > textB) {
                        return -1;
                    }
                    if (textA < textB) {
                        return 1;
                    }
                    return 0;
                }
            }

            function rebuildTableBody() {
                var row = document.createElement('tr');
                var cell = document.createElement('td');
                var frag = document.createDocumentFragment();

                $(self.sortData).each(function(n, rowData) {
                    var tr = row.cloneNode();
                    for (var val in rowData) {
                        if (rowData.hasOwnProperty(val)) {
                            var td = cell.cloneNode();
                            var text = document.createTextNode(rowData[val]);
                            td.appendChild(text);
                            tr.appendChild(td);
                        }

                    }
                    frag.appendChild(tr);
                });
                $tbody[0].innerHTML = '';
                $tbody[0].appendChild(frag);
            }

            // Add default aria-label to each table header link
            $triggers.each(function(){
                var headerName = $(this).text().trim();
                $(this).attr('aria-label', headerName + '. Click to sort data by this column.');
            });

            $triggers.on('click', function (e) {
                e.preventDefault();
                var sort = $.data(this, 'sortType');
                var sorted;
                var sortClass;

                if ($(this).hasClass('selected-sort')) {
                    asc = !asc;
                }

                asc ? sortClass = 'sorted-asc' : sortClass = 'sorted-desc';
                $triggers.removeClass('selected-sort sorted-asc sorted-desc');

                $(this).addClass('selected-sort');

                $(this).addClass(sortClass);

                // Adding a custom aria-label to the selected table header link
                var sortDirText;
                var sortTextFull = $(this).text().trim();
                asc ? sortDirText = 'an ascending order' : sortDirText = 'a descending order';
                $(this).attr('aria-label', sortTextFull + ' is sorted in ' + sortDirText + '. Click to change order. All other columns are sortable.');

                sortKey = $.data(this, 'sortKey');

                if (sort === 'sortText') {
                   sorted = self.sortData.sort(sortText);
                } else if (sort === 'sortDate') {
                    sorted = self.sortData.sort(sortDate);
                } else {
                    sorted = self.sortData.sort(sortNumeric);
                }

                self.sortData = sorted;
                rebuildTableBody();
            });

            selectedTrigger.click();
        }
    }

    $.fn.clientSort = function(opts) {
        return this.each(function() {
            if (!$.data(this, 'clientSort')) {
                $.data(this, 'clientSort', new ClientSort(this, opts));
            }
        });
    }
}($));
