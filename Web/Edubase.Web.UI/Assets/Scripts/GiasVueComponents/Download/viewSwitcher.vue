<template>
    <div class="gias-align-right__from-tablet govuk-!-padding-bottom-1">
        <a role="button" class="govuk-!-font-size-19" href="#" v-show="viewAsList" v-on:click.prevent="changeViewMode">View in sections</a>
        <a role="button" class="govuk-!-font-size-19" href="#" v-show="!viewAsList" v-on:click.prevent="changeViewMode">View in alphabetical order</a>
    </div>
</template>

<script>
    export default {
        name: "viewSwitcher",
        props: {
            viewAsList: Boolean
        },
        methods: {
            changeViewMode() {
                this.$root.changeViewMode();
            }
        }
    }

</script>

