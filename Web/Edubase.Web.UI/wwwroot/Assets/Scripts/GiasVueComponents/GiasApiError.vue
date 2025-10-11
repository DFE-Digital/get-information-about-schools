<template>
  <div v-show="Object.keys(apiError).length> 0" class="govuk-grid-row">
    <div class="govuk-grid-column-full">
      <h1 class="govuk-heading-xl">Sorry, there is a problem with the service</h1>
      <p>Try again later.</p>

      <p class="api-error-code" v-show="apiError.hasOwnProperty('errorCode')">Error code: {{apiError.errorCode}}</p>
      <div v-show="apiError.hasOwnProperty('technicalDetails')">
        <code>
          {{apiError.technicalDetails}}
        </code>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: "giasApiError",
    props: {
      apiError: Object
    }
  }
</script>

