import { createApp } from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
import checkbox from '../GiasVueComponents/checkbox';
import checkboxRemoteSelection from '../GiasVueComponents/Download/checkboxRemoteSelection';
import downloadCategorisedFields from '../GiasVueComponents/DownloadCategorisedFields'
import viewSwitcher from "../GiasVueComponents/Download/viewSwitcher";
import searchByFieldName from '../GiasVueComponents/Download/searchByFieldName';

const app = createApp({
  components: {
    errorSummary,
    checkbox,
    checkboxRemoteSelection,
    downloadCategorisedFields,
    viewSwitcher,
    searchByFieldName
  },
  watch: {
    selectedFields(newVal, oldVal) {
      console.log('changed ', oldVal, '-', newVal);
      console.trace('selectedFields reset source');
    }},

  mounted: function () {
    console.log('[parent] sintial selected: ', this.selectedFields)
    this.$watch(
      () => this.selectedFields,
      (newVal) => {
        console.log('[parent] selectedFields watcher triggered: ', newVal)
      },
      {deep: true}
    );

    console.log('vue app mounted. Initial selectedFields', this.selectedFields);

    this.$watch('selectedFields', (newVal) => {
      console.log('selectedFields changed:', newVal)
    })

    let noJsElem = document.getElementById('no-js-container');
    noJsElem.parentNode.removeChild(noJsElem);
  },

  data() {
    return {
      categoryList: window.fieldListByCat,
      fields: window.fieldList,
      viewAsList: false,
      selectedFields: [],
      searchMatches: [],
      errorMessages: [],
      radioError: false,
      isFilteringFields: false,
    };
  },

  methods: {
    selectAll: function () {
      this.selectedFields = [];
      this.selectedFields.push(...this.fields.map((fld) => {
        return fld.id
      }));
    },
    deselectAll: function () {
      this.selectedFields = [];
    },
    handleSelectedFieldsUpdate(updatedFields) {
      console.log('[parent] selected fields updated', updatedFields);
      this.selectedFields = updatedFields;
    },
    changeViewMode: function () {
      this.viewAsList = !this.viewAsList;
    },
    filterFilters: function (searchText) {
      this.searchMatches = this.fields.filter((fld) => {
        return fld.name.toLowerCase().indexOf(searchText.toLowerCase()) > -1;
      });
    },
    handleSubmit(event) {
      event.preventDefault();
      console.log('form submitted');
      console.log('selectedFields', this.selectedFields);

      if (this.selectedFields.length > 0) {
        this.radioError = false;
        console.log('form is valid. proceeding');
      } else {
        this.radioError = true;
        console.log('no fields selected');
        this.errorMessages = [{href: '#js-field-selection', message: 'Select at least one field'}];
        window.scrollTo(0, 0);
      }
    },
    handleCheckboxToggle(payload) {
      console.log('handling checkbox toggle', payload);
      console.log('before update, selectedFields', this.selectedFields);

      const {value, checked} = payload;

      let updatedFields = [...this.selectedFields];

      if (checked && !updatedFields.includes(value)) {
        updatedFields.push(value);
      } else if (!checked && updatedFields.includes(value)) {
        updatedFields = updatedFields.filter(item => item !== value);
      }

      this.selectedFields = JSON.parse(JSON.stringify(updatedFields));

      console.log('after update, selectFields: ', this.selectedFields);
    }
  },
    computed: {
      formIsValid: function () {
        return this.selectedFields.length > 0;
      },
      hasErrors: function () {
        return this.errorMessages.length > 0;
      },
      shouldShowContinue: function () {
        return !(this.isFilteringFields && this.searchMatches.length === 0);
      }
    }
});

app.mount('#field-select-app');
