<template>
    <v-dialog
        v-model="active"
        persistent width="600">
        <v-card
            tile flat>
            <v-toolbar
                flat
                fixed
                height="76"
                class="pl-4"
                color="transparent"
                width="100%"
            >
                <v-toolbar-title>
                    Edit account
                </v-toolbar-title>
                <v-spacer/>
                <v-btn
                    class="mr-2"
                    icon @click="close">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-form
                    v-model="submittable">
                    <v-text-field
                        outlined
                        :rules="validators.required"
                        v-model="save_account_request.email"
                        label="Email address*"/>
                    <v-text-field
                        outlined
                        v-model="save_account_request.given_name"
                        label="Name"/>
                    <v-text-field
                        outlined
                        v-model="save_account_request.family_name"
                        label="Surname"/>
                    <v-text-field
                        outlined
                        v-model="save_account_request.phone_number"
                        label="Phone number"/>
                    <v-select
                        outlined
                        :rules="validators.required"
                        v-model="save_account_request.role"
                        :items="$readable_role_array"
                        item-text="role"
                        item-value="value"
                        label="Account role*"/>
                </v-form>
            </v-card-text>
            <v-divider class="mx-4"/>
            <v-card-actions>
                <v-spacer/>
                <v-btn
                    text color="primary"
                    @click="save_account">
                    Save
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">

    import { create_account, get_account, save_account } from "~/commons/requests/management/accounts";
    import { CreateAccountRequest } from "~/commons/requests/models/create-account-request";
    import { SaveAccountRequest } from "~/commons/requests/models/save-account-request";
    import { RoleType } from "~/commons/resources/role-type";
    import { Component, mixins, Property } from "../../commons";
    import ComponentBase from "../../commons/mixins/component-base";
    import InputValidators from "../../commons/mixins/input-validators";

    @Component
    export default class AccountCreatorDialog extends mixins(ComponentBase, InputValidators)
    {
        @Property({
            required: true,
            type: Boolean
        })
        active!: boolean;

        @Property({
            required: true,
            type: Number
        })
        account_id!: number;

        submittable: boolean = false;
        save_account_request: SaveAccountRequest = {
            id: 0,
            email: "",
            given_name: "",
            family_name: "",
            phone_number: "",
            role: RoleType.Client
        }

        created()
        {
            if (this.account_id === 0) {
                return;
            }

            get_account(this.account_id)
                .then((response) =>
                {
                    this.save_account_request = {
                        id: response.id,
                        email: response.email,
                        family_name: response.family_name,
                        given_name: response.given_name,
                        phone_number: response.phone_number,
                        role: response.role
                    };
                })
                .catch((error) => {
                    this.$notify_error(error);
                    this.close();
                });
        }

        save_account()
        {
            if (!this.submittable) {
                return;
            }

            save_account(this.save_account_request)
                .then(() =>
                {
                    this.$notify_message("Account information updated");
                    this.$emit("updated:accounts");

                    this.save_account_request = {
                        id: 0,
                        email: "",
                        given_name: "",
                        family_name: "",
                        phone_number: "",
                        role: RoleType.Client
                    }

                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error);
                });
        }

        close() {
            this.$emit("update:active", false);
        }
    }

</script>
