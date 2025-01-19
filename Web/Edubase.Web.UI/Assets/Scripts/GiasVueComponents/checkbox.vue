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
        modelValue: {
          type: [Boolean, Array],
          default: () => []
        },
        value: {
          type: [String, Boolean, Number, Object],
          required: true
        },
        name: String
      },
      computed: {
        checkboxState() {
          if (Array.isArray(this.modelValue)) {
            return this.modelValue.includes(this.value);
          }
          return Boolean(this.modelValue);
        }
      },
      methods: {
        toggle(event) {
          event.preventDefault();
          event.stopPropagation();

          console.log('checkbox toggled:', this.value);

          this.$emit('checkbox-toggled', {
            value: this.value,
            checked: event.target.checked
          });

          console.log('emitted checkbox-toggled with:', {
            value: this.value,
            checked: event.target.checked
          })
        }
      }
    };
</script>
