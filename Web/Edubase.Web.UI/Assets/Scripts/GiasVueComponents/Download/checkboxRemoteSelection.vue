<template>
    <div class="govuk-grid-column-one-half govuk-!-padding-bottom-8 gias-align-right__from-tablet govuk-!-padding-top-2">
        <a class="govuk-!-font-size-19" href="#" v-on:click.prevent="selectAll">Select all</a>
        &nbsp; &nbsp; &nbsp;
        <a class="govuk-!-font-size-19" href="#" v-on:click.prevent="deselectAll">Clear all</a>
    </div>

</template>

<script>
    export default {
        name: "checkboxRemoteSelection",
        props: {
            selectedFields: Array
        },

        methods: {
            selectAll: function() {
                this.$root.selectAll();

            },
            deselectAll: function(){
                this.$root.deselectAll();
            }
        }
    }
</script>

