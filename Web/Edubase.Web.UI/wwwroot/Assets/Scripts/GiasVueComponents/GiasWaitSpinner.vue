<template>
    <div aria-live="polite" class="gias-wait-mask gias-wait-mask--inline" v-show="isProcessing">
    <p class="govuk-visually-hidden">Please wait</p>
      <div class="lds-spinner"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div>
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
