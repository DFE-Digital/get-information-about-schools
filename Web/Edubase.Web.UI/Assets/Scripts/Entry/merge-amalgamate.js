import Vue from 'vue';
import errorSummary from '../GiasVueComponents/ErrorSummary';
import GiasRadio from '../GiasVueComponents/GiasRadio';
import GiasWaitSpinner from '../GiasVueComponents/GiasWaitSpinner';
const mergersApp = new Vue({
  el: '#mergers-app',
  components: {
    errorSummary,
    GiasRadio,
    GiasWaitSpinner,
  },
  data: {
    localAuthorities: window.localAuthorities,
    types: window.types,
    phases: window.phases,
    errorTitle: 'Incorrect or missing details',
    errors: [],
    commitErrors: '',
    apiError: {},
    isProcessing: false,

    mergerType: '',
    mergerTypeConfirmed: false,
    mergerTypeError: false,
    mergerComplete: false,
    amalgamationComplete: false,

    estabLookup: '/api/establishment/{0}',
    commitApi: '/api/amalgamate-merge',

    amalgamatedEstab1: '',
    amalgamatedEstab2: '',
    amalgamatedEstab3: '',
    amalgamatedEstab4: '',

    amalgamatedUrn1Error: '',
    amalgamatedUrn2Error: '',
    amalgamatedUrn3Error: '',
    amalgamatedUrn4Error: '',


    mergerEstab0: '',
    mergerEstab1: '',
    mergerEstab2: '',
    mergerEstab3: '',

    mergerUrn0Error: '',
    mergerUrn1Error: '',
    mergerUrn2Error: '',
    mergerUrn3Error: '',

    amalgamationEstabs: [],
    mergerEstabs: [],
    validMergeUrns: false,

    newName: '',
    typeId: '',
    phaseId: '',
    laId: '',
    mergeDateDay: '',
    mergeDateMonth: '',
    mergeDateYear: '',

    nameError: false,
    typeError: false,
    laError: false,
    mergeDateError: false,
    phaseError: false,

    completeAmalgamation: false,
    amalgUrn: '',
    exitUrl: '',

  },
  created: function() {
    this.amalgamationFields = [
      this.amalgamatedEstab1,
      this.amalgamatedEstab2,
      this.amalgamatedEstab3,
      this.amalgamatedEstab4,
    ];

    this.mergerFields = [
      this.mergerEstab0,
      this.mergerEstab1,
      this.mergerEstab2,
      this.mergerEstab3
    ];

    this.populateSelect('new-establishment-type', this.types);
    this.populateSelect('LocalAuthorityId', this.localAuthorities);
  },
  methods: {
    populateSelect: function (control, data) {
      let frag = document.createDocumentFragment();

      document.getElementById(control).options.length = 0;

      $.each(data, function (n, item) {
        let option = document.createElement('option');
        option.value = item.id;
        option.innerHTML = item.name;

        frag.appendChild(option);
      });
      document.getElementById(control).appendChild(frag);
    },
    updatePhases: function () {
      const tp = type2PhaseMap;
      const self = this;
      const validOptions = [];

      let validPhaseIds = [];
      for (const k in tp) {
        if (k === self.typeId) {
          validPhaseIds = tp[k];
        }
      }

      for (let i = 0, len = validPhaseIds.length; i < len; i++) {
        const obj = phases.filter(function(phase) {
          return phase.id === validPhaseIds[i];
        })[0];
        validOptions.push(obj);
      }

      this.populateSelect('new-establishment-phase', validOptions);
    },
    checkMergeType: function () {
      this.mergerTypeError = this.mergerType === '';
      if (!this.mergerTypeError) {
        this.mergerTypeConfirmed = true;
        this.clearErrors();
      } else {
        this.errors.push ({
          message: "Please select 'Amalgamation' or 'Merger'",
          href: '#radio-amalgamation'
        });
        this.errorFocus();
      }
    },
    clearErrors: function () {
      window.document.title = "Amalgamations and mergers tool - GOV.UK";
      this.errors = [];
      this.amalgamatedUrn1Error = '';
      this.amalgamatedUrn2Error = '';
      this.amalgamatedUrn3Error = '';
      this.amalgamatedUrn4Error = '';
      this.mergerUrn1Error = '';
      this.mergerUrn2Error = '';
      this.mergerUrn3Error = '';
      this.mergerUrn4Error = '';
    },

    errorFocus: function () {
      window.document.title = "Error: Amalgamations and mergers tool - GOV.UK";
      $('.govuk-error-summary').focus()
    },
    checkIfInvalid: function(component){
      return this[component].length && this[component].length < 5 || isNaN(this[component])
    },
    validateField: function(component, componentErrorField){
      if (this.checkIfInvalid(component)) {
        const index = component.replace(/\D/g,'');
        this[componentErrorField] = `The establishment ${index} URN is invalid`;
      }
    },
    urnCheck: function(urn, component, errorField){
      const self = this;
      const index = component.replace(/\D/g,'')
      $.ajax({
        url: self.estabLookup.replace('{0}', urn),
        dataType: 'json',
        method: 'get',
        async: false,
        success: function (data) {
          self[component + 'Valid'] = !data.notFound;
          if (self[component + 'Valid']) {
            if (self.mergerType === 'merger') {
              self.mergerEstabs.push(data.returnValue);
            } else {
              self.amalgamationEstabs.push(data.returnValue);
            }
          }
          self[component + 'UrnChecked'] = true;
        },
        error: function (jqxhr) {
          if (jqxhr.hasOwnProperty('responseJSON')) {
            self.apiError = jqxhr.responseJSON;
          }
          self[errorField] = 'The establishment '+ index + ' URN was not found';
          self.errors.push({
            message: 'The establishment '+ index + ' URN was not found',
            href: '#' + self.mergerType + '-estab' + index
          });
          // self[component + 'Valid'] = false;
          // self[component + 'NoMatch'] = true;
          // self[component + 'UrnChecked'] = true;
        },
        complete: function () {}
      });
    },
    hasDuplicateUrn: function() {
      const arr = this.addedUrns.sort();
      let out = [];
      for (let i = 0, len = arr.length; i < len; i++) {
        if (arr[i] !== '' && arr[i + 1] === arr[i]) {
          out.push(arr[i]);
        }
      }
      return out.length ? out : false;
    },
    validateAmalgamationFields: function(){
      const self = this;
      this.clearErrors();

      this.isProcessing = true;
      console.log(this.isProcessing);
     // this.clearAmalgamationFields();
      this.amalgamationEstabs = [];


      if (this.amalgamatedEstab1 === '') {
        this.amalgamatedUrn1Error = 'Enter the establishment 1 URN';
        this.errors.push({
          message: 'Enter the establishment 1 URN',
          href: '#amalgamation-estab1'
        });
      }

      if (this.amalgamatedEstab2 === '') {
        this.amalgamatedUrn2Error = 'Enter the establishment 2 URN';
        this.errors.push({
          message: 'Enter the establishment 2 URN',
          href: '#amalgamation-estab2'
        })
      }

      const hasDupes = this.hasDuplicateUrn();
      console.log( 'hasDupes == ', hasDupes);
      if (hasDupes) {
        this.errors.push({
          message: 'One or more <abbr title="Unique Reference Number">URNs</abbr> are duplicated. Please enter different URN(s)',
          href: '#linked-establishments'
        });

        if (hasDupes.indexOf(this.amalgamatedEstab1) > -1){
          this.amalgamatedUrn1Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }

        if (hasDupes.indexOf(this.amalgamatedEstab2) > -1){
          this.amalgamatedUrn2Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }

        if (hasDupes.indexOf(this.amalgamatedEstab3) > -1) {
          this.amalgamatedUrn3Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }

        if (hasDupes.indexOf(this.amalgamatedEstab4) > -1) {
          this.amalgamatedUrn4Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }
      }


      this.validateField('amalgamatedEstab1', 'amalgamatedUrn1Error');
      this.validateField('amalgamatedEstab2','amalgamatedUrn2Error');
      this.validateField('amalgamatedEstab3', 'amalgamatedUrn3Error');
      this.validateField('amalgamatedEstab4', 'amalgamatedUrn4Error');

      let requests = [];
      if (!this.amalgamatedUrn1Error.length) {
        requests.push(self.urnCheck(self.amalgamatedEstab1, 'amalgamatedEstab1','amalgamatedUrn1Error'));
      }

      if (!this.amalgamatedUrn2Error.length) {
        requests.push(self.urnCheck(self.amalgamatedEstab2, 'amalgamatedEstab2','amalgamatedUrn21Error'));
      }

      if (!this.amalgamatedUrn3Error.length && this.amalgamatedEstab3.length) {
        requests.push(self.urnCheck(self.amalgamatedEstab3, 'amalgamatedEstab3','amalgamatedUrn3Error'));
      }

      if (!this.amalgamatedUrn4Error.length && this.amalgamatedEstab4.length) {
        requests.push(self.urnCheck(self.amalgamatedEstab4, 'amalgamatedEstab4','amalgamatedUrn4Error'));
      }

      $.when.apply($,requests).done(function() {
        self.isProcessing = false;
        if (!self.errors.length) {
          self.validMergeUrns = true;
        } else {
          self.errorFocus();
        }
      });
    },
    validateMergeFields: function(){
      const self = this;
      this.clearErrors();
      this.mergerEstabs = [];

      if (this.mergerEstab0 === '') {
        this.mergerUrn0Error = 'Enter the lead establishment URN';
        this.errors.push({
          message: 'Enter the lead establishment URN',
          href: '#merger-estab1'
        });
      }

      if (this.mergerEstab1 === '') {
        this.mergerUrn1Error = 'Enter the establishment 1 URN';
        this.errors.push({
          message: 'Enter the establishment 1 URN',
          href: '#merger-estab2'
        });
      }

      const hasDupes = this.hasDuplicateUrn();
      console.log( 'hasDupes == ', hasDupes);
      if (hasDupes) {
        this.errors.push({
          message: 'One or more <abbr title="Unique Reference Number">URNs</abbr> are duplicated. Please enter different URN(s)',
          href: '#linked-establishments'
        });

        if (hasDupes.indexOf(this.mergerEstab0) > -1){
          this.mergerUrn0Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }

        if (hasDupes.indexOf(this.mergerEstab1) > -1){
          this.mergerUrn1Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }

        if (hasDupes.indexOf(this.mergerEstab3) > -1) {
          this.mergerUrn2Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }

        if (hasDupes.indexOf(this.mergerEstab3) > -1) {
          this.mergerUrn3Error = 'Duplicate URN. Please correct the <abbr title="Unique Reference Number">URN</abbr>.';
        }
      }

      this.validateField('mergerEstab0', 'mergerUrn0Error');
      this.validateField('mergerEstab1','mergerUrn1Error');
      this.validateField('mergerEstab2', 'mergerUrn2Error');
      this.validateField('mergerEstab3', 'mergerUrn3Error');

      // var bothChecked;
      // bothChecked = window.setInterval(function () {
      //     if (self.leadEstabUrnChecked && self.linkedEstab1UrnChecked) {
      //       if (self.leadEstabValid && self.linkedEstab1Valid && !self.showGlobalError) {
      //         self.clearErrors();
      //         self.validMergeUrns = true;
      //       }
      //       window.clearInterval(bothChecked);
      //     }
      //   },
      //   100);

      let requests = [];
      if (!this.mergerUrn0Error.length) {
        requests.push(self.urnCheck(self.mergerEstab0, 'mergerEstab0','amalgamatedUrn1Error'));
      }

      if (!this.mergerUrn1Error.length) {
        requests.push(self.urnCheck(self.mergerEstab1, 'mergerEstab1','mergerUrn1Error'));
      }

      if (!this.mergerUrn2Error.length && this.amalgamatedEstab3.length) {
        requests.push(self.urnCheck(self.mergerEstab2, 'mergerEstab2','mergerUrn2Error'));
      }

      if (!this.mergerUrn3Error.length && this.amalgamatedEstab4.length) {
        requests.push(self.urnCheck(self.mergerEstab3, 'mergerEstab3','mergerUrn3Error'));
      }

      $.when.apply($,requests).done(function() {
        self.isProcessing = false;
        if (!self.errors.length) {
          self.validMergeUrns = true;
        } else {
          self.errorFocus();
        }
      });
    },
    processAmalgamation: function () {
      const self = this;
      let postData = {};

      this.nameError = (this.newName.length < 1);
      this.typeError = (this.typeId === '');
      this.laError = (this.laId === '');
      this.phaseError = (this.phaseId === '');
      this.mergeDateError = this.validateMergerDate();

      this.errorFocus();

      if (!this.nameError &&
        !this.typeError &&
        !this.laError &&
        !this.phaseError &&
        !this.mergeDateError &&
        !this.duplicateUrnsError) {

        this.isProcessing = true;
        postData.operationType = 'amalgamate';
        postData.MergeOrAmalgamationDate = [this.mergeDateYear, this.mergeDateMonth, this.mergeDateDay].join('-');
        postData.UrnsToMerge = this.amalgamationEstabs.map(function (estab) {
          return estab.urn;
        });
        postData.NewEstablishmentName = this.newName;
        postData.NewEstablishmentPhaseId = this.phaseId;
        postData.NewEstablishmentTypeId = this.typeId;
        postData.NewEstablishmentLocalAuthorityId = this.laId;


        $.ajax({
          url: self.commitApi,
          method: 'post',
          contentType: 'application/json; charset=utf-8',
          dataType: 'json',
          data: JSON.stringify(postData),
          success: function (data) {
            if (data.hasOwnProperty('successful') && data.successful) {
              self.completeAmalgamation = true;
              self.amalgUrn = data.response.amalgamateNewEstablishmentUrn;
              self.isProcessing = false;
              self.clearErrors();
            }
          },
          error: function (jqXHR) {
            var errObj = JSON.parse(jqXHR.responseText);
            var errMessage = '';

            if (jqXHR.hasOwnProperty('responseJSON')) {
              self.apiError = jqXHR.responseJSON;
            }

            for (var item in errObj) {
              if (item === 'validationEnvelope' && errObj.validationEnvelope) {
                var env = errObj[item][0].errors;
                env.forEach(function (er) {
                  //console.log(errObj[item][0].errors);
                  //console.log(er);
                  errMessage += er.message + '<br>';
                });
              } else if (!errObj.validationEnvelope && item === 'errors') {
                errMessage = errObj.errors[0].message;
                self.validMergeUrns = false;
              }
            }
            self.commitErrors = errMessage;
            self.isProcessing = false;
            self.errorFocus();
          }
        });
      }
    },
    selectedEstablishmentType: function () {
      const self = this;
      if (self.typeId !== '') {
        return types.filter(function(t) {
          return t.id == self.typeId;
        })[0].name;
      }
    },
    schoolDetailUrl: function () {
      return "/Establishments/Establishment/Details/" + this.leadEstab;
    },
    amalgUrl: function () {
      return '/Establishments/Establishment/Details/' + this.amalgUrn;
    },
    validateMergerDate: function () {
      const day = parseInt(this.mergeDateDay, 10);
        const month = parseInt(this.mergeDateMonth -1, 10);
        const year = parseInt(this.mergeDateYear, 10);

        let dateError = false;
        const months31 = [0, 2, 4, 6, 7, 9, 11];
        const currentYear = new Date().getFullYear();

      if (!day || !month || !year || isNaN(day) || isNaN(month) || isNaN(year)) {
        dateError = true;
      }

      const isLeap = new Date(year, 1, 29).getMonth() === 1; // will return march for non leap years

      if (isLeap && month === 1) {
        if (day > 29) {
          dateError = true;
        }
      } else if (month === 1) {
        if (day > 28) {
          dateError = true;
        }
      }

      if (months31.indexOf(month - 1)) {
        if (day < 1 || day > 31) {
          dateError = true;
        }
      } else {
        if (day < 1 || day > 30) {
          dateError = true;
        }
      }

      if (month < 1 || month > 12) {
        dateError = true;
      }

      return dateError;
    },
  },
  computed: {
    pageTitle: function () {
      if (this.mergerType === 'merger' && this.mergerTypeConfirmed){
        return 'Merger';

      } else if (this.mergerType === 'merger' && this.mergerComplete){
        return 'Merger complete';

      } else if(this.mergerType === 'amalgamation' && !this.completeAmalgamation && this.mergerTypeConfirmed) {
        return 'Amalgamation';

      } else if (this.amalgamationComplete) {
        return 'Amalgamation complete';
      } else {
        return 'Amalgamations and mergers tool';
      }
    },
    addedUrns: function() {
      if (this.mergerType === 'merger') {
        return [this.mergerEstab0, this.mergerEstab1, this.mergerEstab2, this.mergerEstab3];
      }
      return [this.amalgamatedEstab1, this.amalgamatedEstab2, this.amalgamatedEstab3, this.amalgamatedEstab4];
    },

    displayDate: function () {
      return this.mergeDateDay + '/' + this.mergeDateMonth + '/' + this.mergeDateYear;
    },

  }

})
