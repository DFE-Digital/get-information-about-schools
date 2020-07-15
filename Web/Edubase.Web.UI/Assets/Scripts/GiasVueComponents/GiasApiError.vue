<template>
  <div v-show="Object.keys(apiError).length> 0" class="govuk-grid-row">
    <div class="govuk-grid-column-full">
      <h1 class="govuk-heading-xl">Sorry, something went wrong</h1>
      <p>Please try again later.</p>
      <p>A report of the error has been sent to our technical team.</p>

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

