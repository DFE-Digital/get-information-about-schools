
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
                    :checked="selectedFields.includes(chx.id)"
                    @update:checked="updateSelectedFields(chx.id, $event)"
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
    selectedFields: Array,
    hasError: Boolean,
  },
  components: {
    checkbox,
  },
  methods: {
    updateSelectedFields(id, isChecked) {
     if (isChecked) {
       this.selectedFields.push(id);
    } else {
      const index = this.selectedFields.indexOf(id);
      if (index > -1) {
        this.selectedFields.splice(index, 1);
	}
      }
	this.$emit('update:selectedFields', this.selectedFields);
    },
  },
};
</script>
