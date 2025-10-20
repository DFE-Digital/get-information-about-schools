<template>
  <div class="govuk-radios__item">
    <input class="govuk-radios__input vue"
           :id="inputId" type="radio"
           :value="value"
           :name="name"
           v-model="value"
           v-on:change="emitValue($event.target.value)" />
    <label class="govuk-label govuk-radios__label vue" :for="inputId">
      {{labeltext}}
    </label>
  </div>
</template>

<script>
    export default {
      name: "giasRadio",
      model: {
        prop: 'checked',
        event: 'change'
      },
      props: {
        value: String,
        labeltext: String,
        binding: String,
        name: String,
        checkedValue: String,
      },
      methods: {
        emitValue: function(value) {
          this.$emit('input', value);
        }
      },
      computed: {
          inputId: function(){
            return this.binding + this.value;
          }
      }

    }
</script>


