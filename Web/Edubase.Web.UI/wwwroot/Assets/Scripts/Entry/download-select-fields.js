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

  mounted: function(){
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
    changeViewMode: function () {
      this.viewAsList = !this.viewAsList;
    },
    filterFilters: function (searchText) {
      this.searchMatches = this.fields.filter((fld) => {
        return fld.name.toLowerCase().indexOf(searchText.toLowerCase()) > -1;
      });
    },
    handleSubmit: function (e) {
      if (this.formIsValid) {
        this.radioError = false;

        let frag = document.createDocumentFragment();
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'zm';

        for (let i = 0, len = this.selectedFields.length; i < len; i++) {
          const clone = input.cloneNode();
          clone.value = this.selectedFields[i];
          frag.appendChild(clone);
        }
        document.getElementById('field-selection').appendChild(frag);

        } else {
          e.preventDefault();
          this.errorMessages = [{href: '#js-field-selection', message: 'Select at least one field'}];
          window.scrollTo(0,0);
          this.radioError = true;
        }

    }
  },
  computed: {
    formIsValid: function() {
      return this.selectedFields.length > 0;
    },
    hasErrors: function() {
      return this.errorMessages.length > 0;
    },
    shouldShowContinue: function() {
      return !(this.isFilteringFields && this.searchMatches.length === 0);
    }
  }

});

app.mount('#field-select-app');
