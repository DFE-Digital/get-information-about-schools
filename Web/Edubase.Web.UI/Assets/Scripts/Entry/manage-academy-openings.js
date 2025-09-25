$(function () {
  var $content = $('#academy-openings-content');
  if (!$content.length) {
    return;
  }

  var dataElement = document.getElementById('academy-openings-data');
  var rawData = dataElement ? (dataElement.textContent || dataElement.innerText || '') : '';
  var allItems;

  try {
    allItems = rawData ? JSON.parse(rawData) : [];
  } catch (error) {
    allItems = [];
  }

  if (!Array.isArray(allItems) || !allItems.length) {
    return;
  }

  var $table = $('#academy-openings-table');
  var $tbody = $table.find('tbody');
  var $monthSelect = $('#opening-date-filter');
  var $form = $monthSelect.closest('form');
  var $paginationContainers = $content.find('.upper-pagination, .lower-pagination');

  var state = {
    allItems: allItems,
    month: ($content.data('month') || '').toString(),
    sortField: ($content.data('sortField') || 'OpenDate').toString(),
    sortDir: ($content.data('sortDir') || 'desc').toString().toLowerCase() === 'asc' ? 'asc' : 'desc',
    take: parseInt($content.data('take'), 10) || 50,
    skip: parseInt($content.data('skip'), 10) || 0,
    baseUrl: ($content.data('baseUrl') || window.location.pathname).toString(),
    establishmentTypeId: ($content.data('establishmentTypeId') || '').toString(),
    detailUrlTemplate: ($content.data('detailUrlTemplate') || '').toString(),
    editUrlTemplate: ($content.data('editUrlTemplate') || '').toString(),
    total: allItems.length
  };

  state.currentPage = state.take > 0 ? Math.floor(state.skip / state.take) : 0;

  var UNSORTED_DESCRIPTION = '. Click to sort data by this column.';

  function parseDate(value) {
    if (!value) {
      return 0;
    }

    var date = new Date(value);
    var timestamp = date.getTime();
    return isNaN(timestamp) ? 0 : timestamp;
  }

  function compareStrings(left, right) {
    var a = left || '';
    var b = right || '';
    if (a === b) {
      return 0;
    }

    return a.localeCompare(b);
  }

  function comparePredecessorNameAsc(a, b) {
    var left = a.predecessorName;
    var right = b.predecessorName;

    if (!left && !right) {
      return 0;
    }

    if (!left) {
      return 1;
    }

    if (!right) {
      return -1;
    }

    return left.localeCompare(right);
  }

  function comparePredecessorNameDesc(a, b) {
    var result = comparePredecessorNameAsc(a, b);
    return result === 0 ? 0 : result > 0 ? -1 : 1;
  }

  function sortItems(items) {
    var sorted = items.slice();
    var sortKey = state.sortField + '-' + state.sortDir;

    switch (sortKey) {
      case 'OpenDate-asc':
        sorted.sort(function (a, b) {
          return parseDate(b.openDate) - parseDate(a.openDate);
        });
        break;
      case 'OpenDate-desc':
        sorted.sort(function (a, b) {
          return parseDate(a.openDate) - parseDate(b.openDate);
        });
        break;
      case 'Urn-asc':
        sorted.sort(function (a, b) {
          return (a.urn || 0) - (b.urn || 0);
        });
        break;
      case 'Urn-desc':
        sorted.sort(function (a, b) {
          return (b.urn || 0) - (a.urn || 0);
        });
        break;
      case 'EstablishmentName-asc':
        sorted.sort(function (a, b) {
          return compareStrings(a.establishmentName, b.establishmentName);
        });
        break;
      case 'EstablishmentName-desc':
        sorted.sort(function (a, b) {
          return compareStrings(b.establishmentName, a.establishmentName);
        });
        break;
      case 'EstablishmentType-asc':
        sorted.sort(function (a, b) {
          return compareStrings(a.establishmentType, b.establishmentType);
        });
        break;
      case 'EstablishmentType-desc':
        sorted.sort(function (a, b) {
          return compareStrings(b.establishmentType, a.establishmentType);
        });
        break;
      case 'PredecessorUrn-asc':
        sorted.sort(function (a, b) {
          return compareStrings(a.predecessorUrn, b.predecessorUrn);
        });
        break;
      case 'PredecessorUrn-desc':
        sorted.sort(function (a, b) {
          return compareStrings(b.predecessorUrn, a.predecessorUrn);
        });
        break;
      case 'PredecessorName-asc':
        sorted.sort(comparePredecessorNameAsc);
        break;
      case 'PredecessorName-desc':
        sorted.sort(comparePredecessorNameDesc);
        break;
      default:
        break;
    }

    return sorted;
  }

  function filterItemsByMonth() {
    if (!state.month) {
      return state.allItems.slice();
    }

    var parts = state.month.split('.');
    if (parts.length !== 2) {
      return state.allItems.slice();
    }

    var selectedMonth = parseInt(parts[0], 10);
    var selectedYear = parseInt(parts[1], 10);

    if (isNaN(selectedMonth) || isNaN(selectedYear)) {
      return state.allItems.slice();
    }

    return state.allItems.filter(function (item) {
      var timestamp = parseDate(item.openDate);
      if (!timestamp) {
        return false;
      }

      var date = new Date(timestamp);
      return date.getMonth() + 1 === selectedMonth && date.getFullYear() === selectedYear;
    });
  }

  function escapeHtml(value) {
    if (value === null || value === undefined) {
      return '';
    }

    return String(value)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }

  function buildUrl(overrides) {
    var params = {
      sortBy: state.sortField + '-' + state.sortDir,
      skip: state.skip,
      establishmentTypeId: state.establishmentTypeId,
      month: state.month
    };

    overrides = overrides || {};
    $.extend(params, overrides);

    var query = Object.keys(params)
      .filter(function (key) {
        if (params[key] === undefined || params[key] === null || params[key] === '') {
          return false;
        }

        if (key === 'skip' && parseInt(params[key], 10) === 0) {
          return false;
        }

        return true;
      })
      .map(function (key) {
        return encodeURIComponent(key) + '=' + encodeURIComponent(params[key]);
      })
      .join('&');

    return state.baseUrl + (query ? '?' + query : '');
  }

  function formatPaginationInfo(startIndex, endIndex, total) {
    if (!total) {
      return 'Showing 0 to 0 of 0';
    }

    return 'Showing ' + (startIndex + 1) + ' to ' + endIndex + ' of ' + total;
  }

  function renderTable(items) {
    if (!items.length) {
      $tbody.html('<tr class="govuk-table__row"><td class="govuk-table__cell" colspan="7">No openings found.</td></tr>');
      return;
    }

    var detailTemplate = state.detailUrlTemplate;
    var editTemplate = state.editUrlTemplate;

    var rows = items
      .map(function (item) {
        var urnLink = detailTemplate ? detailTemplate.replace('__urn__', item.urn) : '#';
        var predecessorLink = item.predecessorUrn
          ? '<a class="govuk-link" href="' + detailTemplate.replace('__urn__', item.predecessorUrn) + '">' + escapeHtml(item.predecessorUrn) + '</a>'
          : '';
        var editLink = editTemplate ? editTemplate.replace('__urn__', item.urn) : '#';

        return '<tr class="govuk-table__row">' +
          '<td class="govuk-table__cell cell-openingdate">' + escapeHtml(item.openDateDisplay) + '</td>' +
          '<td class="govuk-table__cell cell-urn"><a class="govuk-link" href="' + urnLink + '">' + escapeHtml(item.urn) + '</a></td>' +
          '<td class="govuk-table__cell cell-establishmentname">' + escapeHtml(item.establishmentName) + '</td>' +
          '<td class="govuk-table__cell cell-establishmenttype">' + escapeHtml(item.establishmentType) + '</td>' +
          '<td class="govuk-table__cell cell-predecessorurn">' + predecessorLink + '</td>' +
          '<td class="govuk-table__cell cell-predecessorname">' + escapeHtml(item.predecessorName) + '</td>' +
          '<td class="govuk-table__cell cell-edit"><a class="govuk-link" href="' + editLink + '">Edit</a></td>' +
          '</tr>';
      })
      .join('');

    $tbody.html(rows);
  }

  function renderPagination(total, startIndex, endIndex) {
    var paginationText = formatPaginationInfo(startIndex, endIndex, total);
    var hasPrevious = state.currentPage > 0;
    var hasNext = endIndex < total;
    var paginationHtml = '<nav role="navigation" aria-label="Pagination" class="pagination">' +
      '<p class="pagination-info">' + paginationText + '</p>' +
      '<ul class="pagination-links">';

    if (hasPrevious) {
      paginationHtml += '<li><a class="pagination-prev" data-page="prev" href="' + buildUrl({ skip: state.skip - state.take }) + '">&lt;&lt; Previous</a></li>';
    }

    if (hasNext) {
      paginationHtml += '<li><a class="pagination-next" data-page="next" href="' + buildUrl({ skip: state.skip + state.take }) + '">Next &gt;&gt;</a></li>';
    }

    paginationHtml += '</ul></nav>';

    $paginationContainers.each(function () {
      $(this).html(paginationHtml);
    });
  }

  function getSortedDescription(direction) {
    return ' is sorted in ' + (direction === 'asc' ? 'an ascending order' : 'a descending order') + '. Click to change order. All other columns are sortable.';
  }

  function updateSortLinks() {
    var sortDescription = getSortedDescription(state.sortDir);

    $table.find('thead th').each(function () {
      var $th = $(this);
      var $link = $th.find('a[data-sort-column]');
      if (!$link.length) {
        $th.removeAttr('aria-sort');
        return;
      }

      var column = $link.data('sortColumn');
      var isSortedColumn = column === state.sortField;
      var nextDir = isSortedColumn && state.sortDir === 'asc' ? 'desc' : 'asc';
      var linkText = $.trim($link.text());

      $link
        .attr('href', buildUrl({ sortBy: column + '-' + nextDir, skip: 0 }))
        .attr('aria-label', linkText + (isSortedColumn ? sortDescription : UNSORTED_DESCRIPTION))
        .toggleClass('selected-sort', isSortedColumn)
        .toggleClass('sorted-asc', isSortedColumn && state.sortDir === 'asc')
        .toggleClass('sorted-desc', isSortedColumn && state.sortDir === 'desc');

      if (isSortedColumn) {
        $th.attr('aria-sort', state.sortDir === 'asc' ? 'ascending' : 'descending');
      } else {
        $th.removeAttr('aria-sort');
      }
    });
  }

  function updateHiddenFields() {
    $form.find('input[name="sortBy"]').val(state.sortField + '-' + state.sortDir);
    $form.find('input[name="establishmentTypeId"]').val(state.establishmentTypeId);
    $monthSelect.val(state.month || '');
  }

  function updateHistory() {
    if (!window.history || !window.history.replaceState) {
      return;
    }

    var url = buildUrl({ skip: state.skip });
    window.history.replaceState(null, document.title, url);
  }

  function bindSortEvents() {
    $table.find('a[data-sort-column]').off('click').on('click', function (event) {
      event.preventDefault();
      var column = $(this).data('sortColumn');
      if (!column) {
        return;
      }

      if (state.sortField === column) {
        state.sortDir = state.sortDir === 'asc' ? 'desc' : 'asc';
      } else {
        state.sortField = column;
        state.sortDir = 'asc';
      }

      state.currentPage = 0;
      render();
    });
  }

  function bindPaginationEvents() {
    $content.find('.pagination-links a').off('click').on('click', function (event) {
      event.preventDefault();
      var action = $(this).data('page');

      if (action === 'prev' && state.currentPage > 0) {
        state.currentPage -= 1;
      } else if (action === 'next' && (state.skip + state.take) < state.total) {
        state.currentPage += 1;
      } else {
        return;
      }

      render();
    });
  }

  function bindMonthEvents() {
    $form.on('submit', function (event) {
      event.preventDefault();
    });

    $monthSelect.on('change', function (event) {
      event.preventDefault();
      state.month = $(this).val();
      state.currentPage = 0;
      render();
    });
  }

  function render() {
    var filtered = filterItemsByMonth();
    var sorted = sortItems(filtered);

    state.total = sorted.length;

    if (state.take <= 0) {
      state.take = 50;
    }

    var pageCount = state.take > 0 ? Math.ceil(sorted.length / state.take) : 1;
    if (pageCount < 1) {
      pageCount = 1;
    }

    if (state.currentPage >= pageCount) {
      state.currentPage = pageCount - 1;
    }

    if (state.currentPage < 0) {
      state.currentPage = 0;
    }

    state.skip = state.currentPage * state.take;

    var endIndex = Math.min(state.skip + state.take, sorted.length);
    var pageItems = sorted.slice(state.skip, endIndex);

    renderTable(pageItems);
    renderPagination(sorted.length, state.skip, endIndex);
    updateSortLinks();
    updateHiddenFields();
    updateHistory();
    bindPaginationEvents();
    bindSortEvents();
  }

  bindMonthEvents();
  render();
});
