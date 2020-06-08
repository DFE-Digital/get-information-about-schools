<template>
    <div class="govuk-checkboxes__item">
        <input type="checkbox"
               class="govuk-checkboxes__input"
               :id="idPrefix+'-'+inputId"
               :value="label"
               :checked="checkboxState"
               v-on:click="toggle" />
        <label class="govuk-label govuk-checkboxes__label" :for="idPrefix+'-'+inputId">
            {{label}}
        </label>
    </div>
</template>

<script>
    export default {
        name: 'checkbox',
        model: {
            prop: 'model',
            event: 'change'
        },
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
            model: {
                type: [Boolean, Array],
                default: undefined
            },
            checked: Boolean,
            value: {
                type: [String, Boolean, Number, Object, Array, Function],
                default: undefined
            },
            name: String,

        },
        data() {
            return {
                lv: this.model
            }
        },
        computed: {
            checkboxState() {
                if (Array.isArray(this.model)) return this.model.indexOf(this.value) !== -1
                return this.model || Boolean(this.lv)
            },
        },
        methods: {
            toggle() {
                let v = this.model || this.lv
                if (Array.isArray(v)) {
                    const i = v.indexOf(this.value)
                    if (i === -1) v.push(this.value)
                    else v.splice(i, 1)
                }
                else {
                    v = !v
                }
                this.lv = v
                this.$emit('change', v, this.value)
            },

        },
        watch: {
            checked(v) {
                if (v !== this.checkboxState) this.toggle()
            },
            model(v) {
                this.lv = v
            }
        },
        mounted() {
            if (this.checked && !this.checkboxState) {
                this.toggle()
            }
        }
    }
</script>

