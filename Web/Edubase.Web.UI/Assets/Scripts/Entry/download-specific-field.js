document.addEventListener('DOMContentLoaded', function () {
  const selectAllLink = document.getElementById('select-all');
  const clearAllLink = document.getElementById('clear-all');
  const switchViewLink = document.getElementById('switch-view');
  const searchInput = document.getElementById('field-search');
  const searchButton = document.getElementById('search-btn');
  const clearSearchLink = document.getElementById('clear-search');
  const searchError = document.getElementById('search-error');
  const responseError = document.getElementById('response-error');
  const searchInlineError = document.getElementById('search-inline-error');
  const checkboxContainer = document.getElementById('checkbox-container');
  const searchFormGroup = document.getElementById('search-form-group');
  const originalHTML = checkboxContainer.innerHTML;
  const nextButtons = document.querySelectorAll('#next-button');

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

  function buildAlphabeticalView() {
    checkboxContainer.innerHTML = '';
    let temp = document.createElement('div');
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
  }

  function buildGroupedView() {
    checkboxContainer.innerHTML = originalHTML;
  }

  selectAllLink?.addEventListener('click', function (e) {
    e.preventDefault();
    getAllCheckboxes().forEach(cb => cb.checked = true);
  });

  clearAllLink?.addEventListener('click', function (e) {
    e.preventDefault();
    getAllCheckboxes().forEach(cb => cb.checked = false);
  });

  searchButton?.addEventListener('click', function (e) {
    e.preventDefault();

    const query = searchInput.value.trim().toLowerCase();
    const errorSummary = document.getElementById('search-error-summary');
    errorSummary.style.display = 'none';

    if (!query) {
      errorSummary.style.display = 'block';
      errorSummary.scrollIntoView({ behavior: 'smooth', block: 'start' });
      searchInlineError.style.display = 'block';
      searchFormGroup?.classList.add('govuk-form-group--error');
      return;
    }

    searchFormGroup?.classList.remove('govuk-form-group--error');

    searchInlineError.style.display = 'none';
    const selectedBefore = getSelections();
    searchError.style.display = 'none';
    searchError.textContent = '';

    checkboxContainer.querySelectorAll('.govuk-fieldset__legend')
      .forEach(lg => lg.style.display = 'none');

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

    const allGroups = checkboxContainer.querySelectorAll('.govuk-form-group');
    allGroups.forEach(group => {
      const itemsInGroup = group.querySelectorAll('.govuk-checkboxes__item');
      const allHidden = Array.from(itemsInGroup).every(item => item.style.display === 'none');
      if (allHidden) {
        group.style.display = 'none';
      }
      else {
        group.style.display = 'block';
      }
    });

    restoreSelections(selectedBefore);

    if (!foundAny) {
      searchError.style.display = 'block';
      clearSearchLink.style.display = 'inline-block';
    }

    clearSearchLink.style.display = 'inline-block';

    document.getElementById('inline-clear').addEventListener('click', function (e) {
      e.preventDefault();
      clearSearchLink.click();
    })

    responseError.style.display = 'inline-block';
    searchButton.style.display = 'inline-block';
    selectAllLink.style.display = 'none';
    clearAllLink.style.display = 'none';

    nextButtons.forEach(btn => btn.style.display = 'none');
    if (foundAny) {
      nextButtons.forEach(btn => btn.style.display = 'inline-block');
      responseError.style.display = 'none';
    }
  });

  clearSearchLink?.addEventListener('click', function (e) {
    e.preventDefault();

    const selectedBefore = getSelections();

    searchError.style.display = 'none';
    searchError.textContent = '';
    searchError.value = '';
    this.style.display = 'none';

    checkboxContainer.innerHTML = '';

    if (isAlphabetical) {
      buildAlphabeticalView();
      switchViewLink.textContent = 'View in sections';
    } else {
      buildGroupedView();
      switchViewLink.textContent = 'View in alphabetical order';
    }

    restoreSelections(selectedBefore);

    selectAllLink.style.display = 'inline-block';
    clearAllLink.style.display = 'inline-block';
    nextButtons.forEach(btn => btn.style.display = 'inline-block');
    responseError.style.display = 'none';
  });

  switchViewLink?.addEventListener('click', function (e) {
    e.preventDefault();

    const selectedBefore = getSelections();
    isAlphabetical = !isAlphabetical;
    checkboxContainer.innerHTML = '';

    if (isAlphabetical) {
      buildAlphabeticalView();
      this.textContent = 'View in sections';
    } else {
      buildGroupedView();
      this.textContent = 'View in alphabetical order';
    }
    restoreSelections(selectedBefore);
  });
});
