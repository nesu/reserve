<template>
    <contents>
        <v-container>
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
                        <v-card
                            tile outlined>
                            <v-card-text>
                                <v-form
                                    v-model="submittable"
                                    @keyup.native.enter="submit">
                                    <v-row>
                                        <v-col
                                            cols="12">
                                            <v-text-field
                                                label="Email address"
                                                v-model="payload.email"
                                                :rules="validators.email"
                                                hide-details="auto"
                                                validate-on-blur
                                                outlined/>
                                        </v-col>
                                        <v-col
                                            cols="12">
                                            <v-text-field
                                                type="password"
                                                label="Password"
                                                v-model="payload.password"
                                                :rules="validators.password"
                                                hide-details="auto"
                                                validate-on-blur
                                                outlined/>
                                        </v-col>
                                        <v-col
                                            cols="12">
                                            <v-text-field
                                                type="password"
                                                label="Confirm password"
                                                v-model="payload.password_confirmation"
                                                :rules="matching_password_rule"
                                                hide-details="auto"
                                                validate-on-blur
                                                outlined/>
                                        </v-col>
                                        <v-col
                                            cols="12">
                                            <v-btn
                                                block
                                                :loading="loading"
                                                :disabled="!submittable"
                                                @click.stop="submit"
                                                color="primary">
                                                Sign Up
                                            </v-btn>
                                        </v-col>
                                    </v-row>
                                </v-form>
                            </v-card-text>
                        </v-card>
                    </v-sheet>
                </v-col>
            </v-row>
        </v-container>
    </contents>
</template>
<script lang="ts">

    import { InputRule } from "~/commons/lib/input-rule";
    import { register_account } from "~/commons/requests/accounts/register-account";
    import { RegistrationRequest } from "~/commons/requests/models/registration-request";
    import { Component, mixins } from "../commons";
    import PageBase from "~/commons/mixins/page-base";
    import InputValidators from "~/commons/mixins/input-validators";

    @Component({
        head: {
            title: "Sign Up"
        },
        authenticated: "guest",
        layout: "empty"
    })
    export default class SignUpPage extends mixins(PageBase, InputValidators)
    {
        loading: boolean = false;
        submittable: boolean = false;
        payload: RegistrationRequest = {
            email: "",
            password: "",
            password_confirmation: ""
        };

        get matching_password_rule(): InputRule[]
        {
            return [
                ...this.validators.required,
                ...this.validators.password,
                (v: any) => this.payload.password === this.payload.password_confirmation || "Passwords must match",
            ];
        }

        submit()
        {
            if (this.loading)
                return;

            this.loading = true;

            register_account(this.payload).then(() => {
                // @TODO: (UX) Informuoti vartotoją dėl sėkmingos registracijos kitu būdu, nes šitos žinutės gali ir nepamatyti.
                this.$modules.notifications.message("New account successfully created.");
                this.$router.push({ path: "/login" });
            })
            .catch((error) => {
                this.$notify_error(error);
            })
            .finally(() => {
                this.loading = false;
            })
        }
    }

</script>
