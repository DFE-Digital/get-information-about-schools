import Vue from 'vue';
import errorSummary from "../GiasVueComponents/ErrorSummary";
import checkbox from "../GiasVueComponents/checkbox";
import checkboxRemoteSelection from "../GiasVueComponents/Download/checkboxRemoteSelection";
import vueaccordion from "../GiasVueComponents/GovukAccordion";
import viewSwitcher from "../GiasVueComponents/Download/viewSwitcher";
import searchByFieldName from "../GiasVueComponents/Download/searchByFieldName";

const app = new Vue({
  el: '#field-select-app',
  components: {
    errorSummary,
    checkbox,
    checkboxRemoteSelection,
    vueaccordion,
    viewSwitcher,
    searchByFieldName
  },

  mounted: function(){
    let noJsElem = document.getElementById('no-js-container');
    noJsElem.parentNode.removeChild(noJsElem);

  },

  data: {
    categoryList: window.fieldListByCat,
    fields: window.fieldList,
    viewAsList: false,
    selectedFields: [],
    searchMatches: [],
    errorMessages: []
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
        }

    }
  },
  computed: {
    formIsValid: function() {
      return this.selectedFields.length > 0;
    },
    hasErrors: function() {
      return this.errorMessages.length > 0;
    }
  }

});


export default app;
