<template>
  <div v-bind:class="['govuk-form-group', hasError? 'govuk-form-group--error' : '']">
    <label :for="textBoxId" class="govuk-label">{{labelText}}</label>
    <span class="govuk-error-message" v-show="hasError">{{errorMessage}}</span>
    <input type="text"
           :value="value"
           v-bind:class="['govuk-input ', textBoxClasses, hasError? ' govuk-input--error': '']"
           :id="textBoxId"
           v-on:input="emitValue($event.target.value)" />
  </div>
</template>

<script>
  export default {
    name: "textFieldFormGroup",
    props: {
      labelText: String,
      textBoxName: String,
      textBoxId: String,
      textBoxClasses: String,
      hasError: Boolean,
      errorMessage: String,
      value: String,
    },
    methods: {
      emitValue: function(e) {
        this.$emit('input', value);
      }
    }
  }
</script>


