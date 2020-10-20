<template>
  <div aria-live="polite" class="wait-spinner" v-show="isProcessing">
    <p class="govuk-visually-hidden">Please wait</p>
  </div>
</template>

<script>
  export default {
    name: "GiasWaitSpinner",
    props: {
      isProcessing: Boolean
    }
  }
</script>
