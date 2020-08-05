<template>
  <div v-show="hasErrors" class="govuk-error-summary" aria-labelledby="error-summary-title" role="alert" tabindex="-1">
    <h2 class="govuk-error-summary__title" id="error-summary-title">
      {{errorTitle}}
    </h2>
    <div class="govuk-error-summary__body">
      <ul class="govuk-list govuk-error-summary__list" v-for="error in errors">
        <li>
          <a :href="error.href" v-html="error.message"></a>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
  export default {
    name: "errorSummary",
    props: {
      errorTitle: String,
      errors: Array
    },
    computed: {
      hasErrors: function () {
        return this.errors.length > 0;
      }
    }
  }
</script>
