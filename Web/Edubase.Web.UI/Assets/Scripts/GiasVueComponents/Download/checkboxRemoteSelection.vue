<template>
    <div class="govuk-grid-column-one-half govuk-!-padding-bottom-8">
        <a href="#" v-on:click.prevent="selectAll">Select all</a>
        <a href="#" v-on:click.prevent="deselectAll">Clear all</a>
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

