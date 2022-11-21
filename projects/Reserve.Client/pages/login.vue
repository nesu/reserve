<template>
    <v-container fluid>
        <v-row
            class="mt-md-12"
            justify="center"
            align="center">
            <v-col
                cols="12"
                class="d-flex justify-center">
                <nuxt-link to="/">
                    <div class="logo-styled">
                        Reserve.
                    </div>
                </nuxt-link>
            </v-col>
            <v-col
                md="6"
                cols="12"
                class="d-flex justify-center">
                <v-sheet
                    width="100%"
                    max-width="400"
                    color="transparent">
                    <re-login-form/>
                    <v-row>
                        <v-col
                            cols="12">
                            <v-row no-gutters>
                                <v-col
                                    class="d-flex justify-start"
                                    cols="6">
                                    Forgot password
                                </v-col>
                                <v-col
                                    class="d-flex justify-end"
                                    cols="6">
                                    Create new account
                                </v-col>
                            </v-row>
                        </v-col>
                    </v-row>
                </v-sheet>
            </v-col>
        </v-row>
    </v-container>
</template>
<script lang="ts">

    import { Component, mixins } from "../commons";
    import PageBase from "../commons/mixins/page-base";

    @Component({
        head: {
            title: "Log In"
        },
        authenticated: "guest",
        layout: "empty",
        components: {
            "re-login-form": require("~/components/login-form.vue").default
        }
    })
    export default class LogInPage extends mixins(PageBase)
    {

    }

</script>
<style lang="scss">
    @import "./assets/style/pages/log-in";
</style>
