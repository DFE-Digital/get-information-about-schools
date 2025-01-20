<template>
    <div class="govuk-checkboxes__item">
        <input type="checkbox"
               class="govuk-checkboxes__input"
               :id="idPrefix+'-'+inputId"
               :value="value"
               :checked="checkboxState"
               @change="toggle" />
        <label class="govuk-label govuk-checkboxes__label" :for="idPrefix+'-'+inputId">
            {{label}}
        </label>
    </div>
</template>

<script>
    export default {
      name: 'checkbox',
      props: {
        inputId: {
          type: String,
          default: undefined
        },
        label: {
          type: String,
          default: undefined
        },
        idPrefix: {
          type: String,
          default: ''
        },
        value: {
          type: [String, Boolean, Number, Object],
          required: true
        },
        modelValue: {
          type: [Boolean, Array],
          default: () => []
        },
        emits: ['update:modelValue'],
        computed: {
          checkboxState() {
            if (Array.isArray(this.modelValue)) {
              return this.modelValue.includes(this.value);
            }
            return false;
          }
        },
        methods: {
          toggle(event) {

            const isChecked = event.target.checked;
            const updatedModelValue = [...this.modelValue];

            if (isChecked && !updatedModelValue.includes(this.value)) {
              updatedModelValue.push(this.value);
            } else if (!isChecked && updatedModelValue.includes(this.value)) {
              const index = updatedModelValue.indexOf(this.value);
              updatedModelValue.splice(index, 1);
            }
            console.log('[downloadCatFields] emitted selectedFields', updatedFields);
            this.$emit('update:modelValue', updatedModelValue)
          }
        }
      }
    };
</script>
