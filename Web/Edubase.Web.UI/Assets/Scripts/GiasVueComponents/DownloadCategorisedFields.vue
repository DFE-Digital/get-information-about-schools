
<template>
  <div v-bind:class="['gias-download-fields', hasError? 'govuk-form-group--error': '' ]" :id="accordionId">
    <span v-if="hasError" class="govuk-error-message">Select at least one field</span>
    <div class="gias-download-fields--section" v-for="(panel, i) in panelData">
        <h2 class="govuk-heading-m">{{panel.category}} </h2>
      <div class="gias-download-fields--section__content">
        <div class="govuk-checkboxes">
          <checkbox :input-id="chx.id"
                    :label="chx.name"
                    :value="chx.id"
                    v-model:modelValue="selectedFields"
                    v-for="(chx, j) in panel.customFields"
                    :key="i + '_' + j"
                    ></checkbox>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import checkbox from "./checkbox";
export default {
  name: "downloadCategorisedFields",
  props: {
    accordionId: String,
    panelData: Array,
    modelValue: {
      type: Array,
      required: true
    },
    hasError: Boolean
  },
  emits: ['update:modelValue'],
  components: {
    checkbox,
  },
  methods: {
    updateSelectedFields(id, isChecked) {
      console.log('[downloadCatFields] updateselectedFields called with ${id}, ischecked: ${isChecked}');
      let updatedFields = [...this.modelValue];

     if (isChecked && !updatedFields.includes(id)) {
       updatedFields.push(id);
      }
     else if (!isChecked && updatedFields.includes(id))
      {
        updatedFields = updatedFields.filter(fileId => fileId !== id);
      }
      console.log('[downloadCatFields] emitted selectedFields', updatedFields);
	    this.$emit ('update:modelValue', updatedFields)

    },
  },
};
</script>
