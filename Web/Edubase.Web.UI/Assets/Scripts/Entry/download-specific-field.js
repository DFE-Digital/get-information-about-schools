document.addEventListener('DOMContentLoaded', function() {
  const selectAllLink = document.getElementById('select-all');
  const clearAllLink = document.getElementById('clear-all');
  const switchViewLink = document.getElementById('switch-view');
  const searchInput   = document.getElementById('field-search');
  const searchButton  = document.getElementById('search-btn');
  const clearSearchLink = document.getElementById('clear-search');
  const searchError   = document.getElementById('search-error');

  const checkboxContainer = document.getElementById('checkbox-container');
  const originalHTML = checkboxContainer.innerHTML;

  let isAlphabetical = false;

  function getAllCheckboxes() {
  return document.querySelectorAll('input[name="selectedCustomFields"]');
}
    function getLabelText(checkboxItem) {
    return checkboxItem.querySelector('label')?.textContent.trim().toLowerCase() || '';
}
    function getSelections() {
    return Array.from(getAllCheckboxes())
      .filter(checkbox => checkbox.checked)
      .map(checkbox => checkbox.value);
}

  function restoreSelections(selectedValues) {
  getAllCheckboxes().forEach(cb => {
   if (selectedValues.includes(cb.value)) {
    cb.checked = true;
   }
  });
}

  selectAllLink?.addEventListener('click', function(e) {
  e.preventDefault();
  getAllCheckboxes().forEach(cb => cb.checked = true);
});

  clearAllLink?.addEventListener('click', function(e) {
  e.preventDefault();
  getAllCheckboxes().forEach(cb => cb.checked = false);
});

  searchButton?.addEventListener('click', function(e) {
  e.preventDefault();

  const query = searchInput.value.trim().toLowerCase();
  if (!query) return;

  const selectedBefore = getSelections();
  searchError.style.display = 'none';
  searchError.textContent = '';

  const legends = checkboxContainer.querySelectorAll('.govuk-fieldset__legend');
  legends.forEach(legend => legend.style.display = 'none');

  const allItems = checkboxContainer.querySelectorAll('.govuk-checkboxes__item');
  allItems.forEach(it => {
    it.style.display = 'block';
  });

  let foundAny = false;
  allItems.forEach(it => {
    const label = getLabelText(it);
    if (!label.includes(query)) {
      it.style.display = 'none';
    } else {
      foundAny = true;
    }
  });
  restoreSelections(selectedBefore);

  if (foundAny) {
    searchError.style.display = 'block';
    searchError.textContent = 'There are no results for your search. Please clear your search and try again.';
  }

  clearSearchLink?.addEventListener('click', function(e) {
    e.preventDefault();

  checkboxContainer.querySelectorAll('.govuk-fieldset__legend').forEach(lg => lg.style.display = '');
  checkboxContainer.querySelectorAll('.govuk-checkboxes__item').forEach(it => it.style.display = 'block');
});

  searchError.style.display = 'none';
  searchError.textContent = '';
  searchError.value = '';
  this.style.display = 'none';
});

  switchViewLink?.addEventListener('click', function(e) {
    e.preventDefault();

    const selectedBefore = getSelections();
    isAlphabetical = !isAlphabetical;

    if (isAlphabetical) {
      checkboxContainer.innerHTML = '';

      let temp = document.createElement('div')
      temp.innerHTML = originalHTML;
      let allItems = Array.from(temp.querySelectorAll('.govuk-checkboxes__item'));

      allItems.sort((a, b) => getLabelText(a).localeCompare(getLabelText(b)));

      const singleFs = document.createElement('fieldset');
      singleFs.className = 'govuk-fieldset';
      const checkboxesDiv = document.createElement('div');
      checkboxesDiv.className = 'govuk-checkboxes';

      allItems.forEach(it => checkboxesDiv.appendChild(it));
      singleFs.appendChild(checkboxesDiv);

      checkboxContainer.appendChild(singleFs);

      switchViewLink.textContent = 'View in sections';
    } else {
      checkboxContainer.innerHTML = originalHTML;
      switchViewLink.textContent = 'View in alphabetical order';
    }
    restoreSelections(selectedBefore);
  });
});
