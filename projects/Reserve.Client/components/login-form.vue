<template>
    <v-card
        outlined tile>
        <v-card-text>
            <v-form
                @keyup.native.enter="submit">
                <v-row
                    class="flex-column">
                    <v-col
                        cols="12">
                        <v-text-field
                            v-model="payload.email"
                            label="Email address"
                            outlined hide-details/>
                    </v-col>
                    <v-col
                        cols="12">
                        <v-text-field
                            v-model="payload.password"
                            label="Password" type="password"
                            outlined hide-details/>
                    </v-col>
                    <v-col
                        cols="12">
                        <v-btn
                            block
                            color="primary"
                            :loading="loading"
                            @click.stop="submit">
                            Log In
                        </v-btn>
                    </v-col>
                </v-row>
            </v-form>
        </v-card-text>
    </v-card>
</template>
<script lang="ts">

    import { Component, mixins  } from "~/commons";
    import ComponentBase from "~/commons/mixins/component-base";
    import { authenticate } from "~/commons/requests/accounts/authenticate";
    import { AuthenticationRequest } from "~/commons/requests/models/authentication-request";

    @Component
    export default class LogInFormComponent extends mixins(ComponentBase)
    {
        loading: boolean = false;
        payload: AuthenticationRequest = {
            email: "",
            password: ""
        };

        submit()
        {
            if (this.loading)
                return;

            this.loading = true;
            authenticate(this.payload).then(({ data }) =>
            {
                const authenticated_homepage = process.env["authenticated_homepage"] ?? "/reservationsv";
                const token_key = process.env["token_key"] ?? "id_token";
                localStorage.setItem(token_key, data.security_token)
                this.$router.replace({ path: authenticated_homepage });
            })
            .catch(() => {
                this.payload.password = "";
                this.$notify_error("Invalid email address or password.");
            })
            .finally(() => {
                this.loading = false;
            });
        }
    }

</script>
