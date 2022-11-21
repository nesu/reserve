<template>
    <v-container>
        <v-sheet
            class="base-banner"
            color="tertiary"
            height="400">
            <v-row
                align="center"
                justify="center"
                class="fill-height pa-3 pa-md-12">
                <v-col
                    class="hidden-sm-and-down"
                    cols="6">
                    <div class="inner-text">
                        Schedule appointments.
                    </div>
                    <div class="inner-text">
                        From anywhere.
                    </div>
                    <br>
                    <div class="inner-text">
                        Anytime.
                    </div>
                </v-col>
                <v-col
                    cols="12"
                    offset-md="2"
                    md="4">
                    <v-sheet
                        color="white">
                        <re-login-form/>
                    </v-sheet>
                </v-col>
            </v-row>
        </v-sheet>
    </v-container>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import ComponentBase from "~/commons/mixins/component-base";

    @Component({
        components: {
            "re-login-form": require("~/components/login-form.vue").default
        }
    })
    export default class HeroBannerComponent extends mixins(ComponentBase)
    {

    }

</script>
<style lang="scss">
    @import "../assets/style/components/herobanner";
</style>
