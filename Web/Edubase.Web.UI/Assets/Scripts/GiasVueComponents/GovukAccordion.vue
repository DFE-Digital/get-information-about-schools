
<template>
    <div class="govuk-accordion" data-module="govuk-accordion" :id="accordionId">
        <div class="govuk-accordion__section" v-for="(panel, i) in panelData">
            <div class="govuk-accordion__section-header">
                <h2 class="govuk-accordion__section-heading">
                    <span class="govuk-accordion__section-button"
                          :id="panel.category.toLowerCase().replace(/\s/g, '-')+'-'+i">
                        {{panel.category}}
                    </span>
                </h2>
            </div>
            <div class="govuk-accordion__section-content"
                 :aria-labelledby="panel.category.toLowerCase().replace(/\s/g, '-')+'-'+i">
                    <div class="govuk-checkboxes">
                        <checkbox :input-id="chx.id"
                                  :label="chx.name"
                                  :value="chx.id"
                                  :checked="modelValue.includes(chx.id)"
                                  @update:checked="updateSelectedFields(chx.id, $event)"
                                  v-for="(chx, j) in panel.customFields" :key="i+'_'+j"></checkbox>
                    </div>
            </div>
        </div>
    </div>
</template>

<script>
    import checkbox from "./checkbox";
    import  {initAll} from 'govuk-frontend';
    export default {
        name: "vueaccordion",
        props: {
            accordionId: String,
            panelData: Array,
            selectedFields: Array
        },
        components: {
            checkbox
        },
        mounted: function() {
            const elem = document.getElementById('js-field-selection');
          initAll({ scope: elem });
            console.log('[vueaccordian] mounted with selectedFields: ', this.selectedFields)
        }
    }
</script>
