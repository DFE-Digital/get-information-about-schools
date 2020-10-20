class GiasTableSort {
  constructor(el, opts) {
    const defaults = {
      triggerClassName: '.js-sort-header',
      sortDirection: 'asc',
      initialSortKey: 'name'
    };

    this.el = el;
    this.opts = $.extend({}, defaults, opts);

    this.init();
  }

  init() {
    const $el = $(this.el);
    const opts = this.opts;
    let asc = opts.sortDirection === 'asc';
    const self = this;
    let sortKey;

    const $triggers = $el.find(opts.triggerClassName);
    const $tbody = $el.find('tbody');

    const sortData = [];

    if ($tbody.find('tr').length < 2) {
      $el.addClass('unsortable');
      $el.removeClass('sortable-table');
      $triggers.removeAttr('href');
      return;
    }
    const selectedTrigger = $triggers.filter(function(n, ele) {
      return $(ele).data().sortKey === opts.initialSortKey;
    });

    $tbody.find('tr').each(function () {
      const $row = $(this);
      let temp = {};
      $row.find('td').each(function (n) {
        const key = $triggers.eq(n).data().sortKey;
        temp[key] = $(this).text();
      });
      sortData.push(temp);
    });

    this.sortData = sortData;

    function dateFromString(dateString) {
      const dateParts = dateString.split('/');
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
      const row = document.createElement('tr');
      const cell = document.createElement('td');
      const frag = document.createDocumentFragment();

      row.classList.add('govuk-table__row');
      cell.classList.add('govuk-table__cell')

      $(self.sortData).each(function(n, rowData) {
        let tr = row.cloneNode();
        for (const val in rowData) {
          if (rowData.hasOwnProperty(val)) {
            let td = cell.cloneNode();
            let text = document.createTextNode(rowData[val]);
            td.appendChild(text);
            tr.appendChild(td);
          }

        }
        frag.appendChild(tr);
      });
      $tbody[0].innerHTML = '';
      $tbody[0].appendChild(frag);
    }

    // Adding data-labels back to the <td>'s to display the label in mobile view
    function addDataLabels() {
      $('.sortable-table tbody tr').each(function(n, el){
        const $el = $(el);
        $el.children('td').each(function(index){
          const headerLabel = $el.parent().siblings('thead').find('tr th:eq('+index+') a').text().trim();
          $(this).attr('data-label', headerLabel);
        });
      });
    }

    // Add default aria-label to each table header link
    $triggers.each(function(){
      const headerName = $(this).text().trim();
      $(this).attr('aria-label', headerName + '. Click to sort data by this column.');
    });

    $triggers.on('click', function (e) {
      e.preventDefault();
      const sort = $.data(this, 'sortType');
      let sorted;
      let sortClass;

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
      addDataLabels();
    });

    selectedTrigger.click();
  }
}
$.fn.giasTableSort = function(opts) {
  return this.each(function() {
    if (!$.data(this, 'giasTableSort')) {
      $.data(this, 'giasTableSort', new GiasTableSort(this, opts));
    }
  });
}

export default GiasTableSort;
