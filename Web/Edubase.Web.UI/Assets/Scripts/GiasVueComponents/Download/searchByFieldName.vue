<template>
  <div>
  <div v-bind:class="['govuk-form-group', filterSearchError ? 'govuk-form-group--error': '']">
        <span class="govuk-error-message" v-show="filterSearchError">
            Enter a field name to search
        </span>
    <label class="govuk-label" for="field-text-search">
      Search by field name
    </label>
    <input v-bind:class="['govuk-input gias-inline-search__field', filterSearchError? 'govuk-input--error' : '']"
           id="field-text-search" v-model="filterSearchText" type="text">
    <button class="govuk-button gias-inline-search__button search-results-download-search-button" v-on:click.prevent="filterFilters">Search</button>
    <a href="#" class="search-results-download-clear-search" v-on:click.prevent="resetTextFilter" v-if="showClearButton">Clear search</a>
  </div>
    <div v-if="noMatchingFields">
      <p>There are no results for your search.
        Please <a href="#" role="button" v-on:click.prevent="resetTextFilter">clear your search</a> and try again.</p>
    </div>
  </div>
</template>

<script>
export default {
  name: "searchByFieldName",
  props: {
    selectedFields: Array,
    fields: Array,
  },
  data: function () {
    return {
      filterSearchText: '',
      showClearButton: false,
      filterSearchError: false,
      isResetting: false,
    }
  },
  methods: {
    filterFilters: function () {
      this.filterSearchError = false;
      this.$root.isFilteringFields = true
      this.$root.errorMessages = [];
      if (!this.isResetting && this.filterSearchText.length === 0) {
        console.log('[searchByFieldName] selectedfields: ', selectedFields)
        this.filterSearchError = true;
        this.$root.errorMessages = [{href: '#field-text-search', message: 'Enter a field name to search'}];

      } else {
        this.$root.viewAsList = true;
        this.$root.filterFilters(this.filterSearchText);
      }
      this.isResetting = false;
      this.showClearButton = this.filterSearchText.length > 0;
    },
    resetTextFilter: function () {
      this.filterSearchError = true;
      this.filterSearchText = '';
      this.isResetting = true;
      this.filterFilters();

      document.getElementById('field-text-search').focus();
    }
  },
  computed: {
    noMatchingFields: function () {
      return this.$root.isFilteringFields && this.$root.searchMatches.length === 0;
    }
  }
}
</script>
