import Vue from 'vue';
import errorSummary from '../GiasVueComponents/errorSummary';
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
      commitErrors: false,
      apiError: {},
      isProcessing: false,
      csrfToken: '',
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
      presentExitWarning: false,

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
    this.blockExits();
  },
  mounted: function () {
    this.csrfToken = this.getAntiForgeryToken();
  },
  watch: {
    mergerTypeConfirmed: function() {
      // breadcrumbs out side of vue app, but need to be kept in sync
      $('#tertiary-breadcrumb').toggleClass('hidden');
    }
  },
  methods: {
    getAntiForgeryToken() {
      return document.querySelector('meta[name="csrf-token"]').getAttribute('content');
    },
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
      this.phaseId = '';

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
      window.document.title = "Amalgamate or merge establishments - GOV.UK";
      this.errors = [];
      this.amalgamatedUrn1Error = '';
      this.amalgamatedUrn2Error = '';
      this.amalgamatedUrn3Error = '';
      this.amalgamatedUrn4Error = '';
      this.mergerUrn0Error = '';
      this.mergerUrn1Error = '';
      this.mergerUrn2Error = '';
      this.mergerUrn3Error = '';
    },

    errorFocus: function () {
      window.document.title = "Error: Amalgamate or merge establishments - GOV.UK";
      $('.govuk-error-summary').focus();
    },
    checkIfInvalid: function(component){
      return this[component].length && this[component].length < 5 || isNaN(this[component])
    },
    validateField: function(component, componentErrorField, fieldId){
      if (this.checkIfInvalid(component)) {
        const index = component.replace(/\D/g,'');
        this[componentErrorField] = `The establishment ${index} URN is invalid`;
        this.errors.push({
          href: '#' + fieldId,
          message:  `The establishment ${index} URN is invalid`,
        });
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
      this.amalgamationEstabs = [];


      if (this.amalgamatedEstab1 === '') {
        this.amalgamatedUrn1Error = 'Enter the establishment 1 URN';
        this.errors.push({
          message: 'Enter the establishment 1 URN',
          href: '#amalgamation-estab1'
        });
      }
      this.validateField('amalgamatedEstab1', 'amalgamatedUrn1Error', 'amalgamation-estab1');

      if (this.amalgamatedEstab2 === '') {
        this.amalgamatedUrn2Error = 'Enter the establishment 2 URN';
        this.errors.push({
          message: 'Enter the establishment 2 URN',
          href: '#amalgamation-estab2'
        })
      }

      this.validateField('amalgamatedEstab2','amalgamatedUrn2Error', 'amalgamation-estab2');

      const hasDupes = this.hasDuplicateUrn();
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



      this.validateField('amalgamatedEstab3', 'amalgamatedUrn3Error', 'amalgamation-estab3');
      this.validateField('amalgamatedEstab4', 'amalgamatedUrn4Error', 'amalgamation-estab4');

      let requests = [];
      if (!this.amalgamatedUrn1Error.length) {
        requests.push(self.urnCheck(self.amalgamatedEstab1, 'amalgamatedEstab1','amalgamatedUrn1Error'));
      }

      if (!this.amalgamatedUrn2Error.length) {
        requests.push(self.urnCheck(self.amalgamatedEstab2, 'amalgamatedEstab2','amalgamatedUrn2Error'));
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
      this.validateField('mergerEstab0', 'mergerUrn0Error', 'merger-estab0');

      if (this.mergerEstab1 === '') {
        this.mergerUrn1Error = 'Enter the establishment 1 URN';
        this.errors.push({
          message: 'Enter the establishment 1 URN',
          href: '#merger-estab2'
        });
      }
      this.validateField('mergerEstab1','mergerUrn1Error', 'merger-estab1');

      const hasDupes = this.hasDuplicateUrn();
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

      this.validateField('mergerEstab2', 'mergerUrn2Error', 'merger-estab2');
      this.validateField('mergerEstab3', 'mergerUrn3Error', 'merger-estab3');

      let requests = [];
      if (!this.mergerUrn0Error.length) {
        requests.push(self.urnCheck(self.mergerEstab0, 'mergerEstab0','mergerUrn0Error'));
      }

      if (!this.mergerUrn1Error.length) {
        requests.push(self.urnCheck(self.mergerEstab1, 'mergerEstab1','mergerUrn1Error'));
      }

      if (!this.mergerUrn2Error.length && this.mergerEstab2.length) {
        requests.push(self.urnCheck(self.mergerEstab2, 'mergerEstab2','mergerUrn2Error'));
      }

      if (!this.mergerUrn3Error.length && this.mergerEstab3.length) {
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
    exitConfirmed: function() {
      window.location = this.exitUrl;
    },
    processAmalgamation: function () {
      const self = this;
      let postData = {};
      this.errors = [];

      this.nameError = (this.newName.length < 1);
      this.typeError = (this.typeId === '' || typeof this.typeId === 'undefined');
      this.laError = (this.laId === '' || typeof this.laId === 'undefined');
      this.phaseError = (this.phaseId === '' || typeof this.phaseId === 'undefined');
      this.mergeDateError = this.validateMergerDate();


      if (this.nameError) {
        this.errors.push({
          href: '#new-establishment-name',
          message: 'New school details must be completed before amalgamation. Please enter a new school name.',
        });
      }

      if (this.typeError) {
        this.errors.push({
          href: '#new-establishment-type',
          message: 'You haven\'t selected an establishment type. Please select one to continue.',
        });
      }

      if (this.phaseError) {
        this.errors.push({
          href: '#new-establishment-phase',
          message: 'You haven\'t selected an establishment phase. Please select one to continue.',
        });
      }

      if (this.laError) {
        this.errors.push({
          href: '#LocalAuthorityId',
          message: 'You haven\'t selected a local authority. Please select one to continue.',
        });
      }

      if (this.mergeDateError) {
        this.errors.push({
          href: '#amalgamation-date-day',
          message: 'You have entered an invalid date. Please correct the date to continue',
        });
      }
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

        const my = this;

        $.ajax({
          url: my.commitApi,
          method: 'post',
          headers: {
            'RequestVerificationToken': my.csrfToken
          },
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
            const errObj = JSON.parse(jqXHR.responseText);
            let errMessage = '';

            if (jqXHR.hasOwnProperty('responseJSON')) {
              self.apiError = jqXHR.responseJSON;
            }

            for (const item in errObj) {
              if (item === 'validationEnvelope' && errObj.validationEnvelope) {
                var env = errObj[item][0].errors;
                env.forEach(function (er) {
                  self.errors.push({
                    href: '#',
                    message: er.message
                  });
                });
              } else if (!errObj.validationEnvelope && item === 'errors') {
                self.errors.push({
                  href: '#',
                  message: errObj.errors[0].message
                });
                self.validMergeUrns = false;
              }
            }
            self.commitErrors = self.errors.length > 0;
            self.isProcessing = false;
            self.errorFocus();
          }
        });
      }
    },
    processMerger: function () {
      const self = this;
      let postData = {};
      this.errors = [];

      postData.operationType = 'merge';
      postData.MergeOrAmalgamationDate = [this.mergeDateYear, this.mergeDateMonth, this.mergeDateDay].join('-');
      postData.LeadEstablishmentUrn = this.mergerEstab0;
      postData.UrnsToMerge = this.mergerEstabs.filter(function(item) {
        return item.urn !== Number(self.mergerEstab0);
      }).map(function (estab) {
        return estab.urn;
      });

      this.mergeDateError = this.validateMergerDate();
      this.errorFocus();

      const my = this;

      if (!this.mergeDateError) {
        this.validLinkDate = true;
        this.isProcessing = true;
        $.ajax({
          url: my.commitApi,
          method: 'post',
          headers: {
            'RequestVerificationToken': my.csrfToken
          },
          contentType: 'application/json; charset=utf-8',
          dataType: 'json',
          data: JSON.stringify(postData),
          success: function (data) {
            if (data.hasOwnProperty('successful') && data.successful) {
              self.mergerComplete = true;
              self.isProcessing = false;
              self.clearErrors();
            }
          },
          error: function (jqXHR) {
            const errObj = JSON.parse(jqXHR.responseText);
            let errMessage = '';

            if (jqXHR.hasOwnProperty('responseJSON')) {
              self.apiError = jqXHR.responseJSON;
            }

            for (const item in errObj) {
              if (item === 'validationEnvelope') {
                const env = errObj[item][0].errors;
                env.forEach(function (er) {
                 // errMessage += er.message + '<br>';
                  self.errors.push({
                    href: '#',
                    message: er.message
                  });
                });
              } else if (!errObj.validationEnvelope && item === 'errors') {
               //errMessage = errObj.errors[0].message;
                self.errors.push({
                  href: '#',
                  message: errObj.errors[0].message
                });
                self.validMergeUrns = false;
              }
            }
            self.commitErrors = self.errors.length > 0;
            self.isProcessing = false;
            self.errorFocus();
          }
        });
      } else {
       self.errors.push({
         href: '#mergedate-day',
         message: 'Merger date is invalid',
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

    validateMergerDate: function () {
      const day = parseInt(this.mergeDateDay, 10);
      const month = parseInt(this.mergeDateMonth - 1, 10);
      const year = parseInt(this.mergeDateYear, 10);

      let dateError = false;
      const months31 = [0, 2, 4, 6, 7, 9, 11];
      const currentYear = new Date().getFullYear();

      if (!day || !this.mergeDateMonth || !year || isNaN(day) || isNaN(month) || isNaN(year)) {
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

      if (this.mergeDateMonth < 1 || this.mergeDateMonth > 12) {
        dateError = true;
      }

      return dateError;
    },
    restart: function () {
      if (!this.mergerTypeConfirmed) {
        window.location = '/Tools';
        return;
      }
      window.location = '/Tools/MergersTool';
    },
    blockExits: function () {
      const self = this;
      $('a, [value="cancel"]').on('click', function (e) {
        self.exitUrl = $(this).attr('href');
        if ((self.mergerType === 'amalgamation' && !self.completeAmalgamation) ||
          (self.mergerType == 'merger' && !self.mergerComplete)) {
          e.preventDefault();
          self.presentExitWarning = true;

          window.setTimeout(function(){
            $('#button-ok').focus();
          },0)
        }
      });
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

      } else if (this.completeAmalgamation) {
        return 'Amalgamation complete';
      } else {
        return 'Amalgamate or merge establishments';
      }
    },
    addedUrns: function() {
      if (this.mergerType === 'merger') {
        return [this.mergerEstab0, this.mergerEstab1, this.mergerEstab2, this.mergerEstab3];
      }
      return [this.amalgamatedEstab1, this.amalgamatedEstab2, this.amalgamatedEstab3, this.amalgamatedEstab4];
    },

    displayDate: function () {
      const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
      const datecur = parseInt(this.mergeDateDay, 10);

      return `${datecur}   ${ monthNames[this.mergeDateMonth -1]} ${this.mergeDateYear}`;
    },
    leadEstablishmentName: function () {
      const self = this;
      if (self.validMergeUrns && self.mergerType === 'merger') {
        const leadName = self.mergerEstabs.filter(function (estab) {
          return estab.urn === Number(self.mergerEstab0);
        })[0].name;

        return leadName;
      }
      return '';
    },
    leadEstablishmentType: function() {
      const self = this;
      if (self.validMergeUrns && self.mergerType === 'merger') {
        const leadType = self.mergerEstabs.filter(function (estab) {
          return estab.urn === Number(self.mergerEstab0);
        })[0].typeName;

        return leadType;
      }
      return '';
    },
    schoolDetailUrl: function () {
      return "/Establishments/Establishment/Details/" + this.mergerEstab0;
    },
    amalgUrl: function () {
      return '/Establishments/Establishment/Details/' + this.amalgUrn;
    },
  }
})
