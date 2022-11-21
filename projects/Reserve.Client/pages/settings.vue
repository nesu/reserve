<template>
    <v-container>
        <v-row>
            <v-col
                cols="12"
                md="6">
                <v-card
                    outlined
                    height="100%"
                    class="d-flex flex-column">
                    <div class="flex-shrink-1">
                        <v-toolbar
                            flat tile
                            height="56">
                            <v-toolbar-title class="headline">
                                Personal information
                            </v-toolbar-title>
                        </v-toolbar>
                    </div>
                    <v-divider class="mx-4"/>
                    <v-card-text
                        class="flex-shrink-1">
                        <v-form
                            v-model="personal_information_submittable"
                            @keyup.native.enter="save_personal_information">
                            <v-row>
                                <v-col cols="12">
                                    <v-text-field
                                        v-model="personal_information_payload.given_name"
                                        label="First name"
                                        prepend-inner-icon="mdi-form-textbox"
                                        :rules="validators.required"
                                        outlined/>
                                    <v-text-field
                                        v-model="personal_information_payload.family_name"
                                        label="Surname"
                                        prepend-inner-icon="mdi-form-textbox"
                                        :rules="validators.required"
                                        outlined/>
                                    <v-text-field
                                        v-model="personal_information_payload.email"
                                        label="Email address"
                                        prepend-inner-icon="mdi-at"
                                        :rules="validators.email"
                                        outlined/>
                                    <v-text-field
                                        v-model="personal_information_payload.phone_number"
                                        label="Phone number"
                                        prepend-inner-icon="mdi-phone"
                                        :rules="validators.required"
                                        outlined/>
                                </v-col>
                            </v-row>
                        </v-form>
                    </v-card-text>
                    <v-card-actions class="pt-0">
                        <v-btn
                            @click.stop="save_personal_information"
                            :disabled="!personal_information_submittable"
                            :loading="personal_information_loading"
                            outlined large
                            color="primary">
                            Save
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
            <v-col
                cols="12"
                md="6">
                <v-card
                    outlined
                    height="100%"
                    class="d-flex flex-column">
                    <div class="flex-shrink-1">
                        <v-toolbar
                            flat tile
                            height="56">
                            <v-toolbar-title class="headline">
                                Security settings
                            </v-toolbar-title>
                        </v-toolbar>
                    </div>
                    <v-divider class="mx-4"/>
                    <v-card-text
                        class="flex-grow-1">
                        <v-form
                            @keyup.native.enter="change_password"
                            v-model="password_change_submittable">
                            <v-row>
                                <v-col cols="12">
                                    <v-text-field
                                        v-model="password_change_payload.current_password"
                                        type="password"
                                        label="Current password"
                                        prepend-inner-icon="mdi-form-textbox-password"
                                        :rules="validators.required"
                                        outlined/>
                                    <v-text-field
                                        v-model="password_change_payload.new_password"
                                        type="password"
                                        label="New password"
                                        prepend-inner-icon="mdi-form-textbox-password"
                                        :rules="validators.password"
                                        outlined/>
                                    <v-text-field
                                        v-model="password_change_payload.confirmed_new_password"
                                        type="password"
                                        label="Confirm new password"
                                        prepend-inner-icon="mdi-form-textbox-password"
                                        :rules="matching_password_rule"
                                        outlined/>
                                </v-col>
                            </v-row>
                        </v-form>
                    </v-card-text>
                    <v-card-actions
                        class="pt-0">
                        <v-btn
                            @click.stop="change_password"
                            :disabled="!password_change_submittable"
                            :loading="password_change_loading"
                            outlined large
                            color="primary">
                            Save
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>
        <v-dialog
            persistent width="420"
            v-model="delete_account_modal">
            <v-card
                flat tile>
                <v-toolbar
                    flat tile
                    height="56">
                    <v-toolbar-title class="headline">
                        Confirmation required
                    </v-toolbar-title>
                    <v-spacer/>
                    <v-btn
                        @click.stop="delete_account_modal = false"
                        class="mr-1"
                        icon small>
                        <v-icon>
                            mdi-close
                        </v-icon>
                    </v-btn>
                </v-toolbar>
                <v-divider class="mx-4"/>
                <v-card-text class="text-sm-justify">
                    Before deleting your account, please confirm this action by
                    entering your email address (<b>{{ $account.email }}</b>)
                </v-card-text>
                <v-card-text>
                    <v-text-field
                        hide-details outlined/>
                </v-card-text>
                <v-divider class="mx-4"/>
                <v-card-actions>
                    <v-spacer/>
                    <v-btn
                        outlined class="red--text">
                        Delete account
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>
</template>
<script lang="ts">

    import { AxiosError } from "axios";
    import { InputRule } from "~/commons/lib/input-rule";
    import { Component, mixins } from "../commons";
    import { change_password, save_personal_information } from "~/commons/requests/accounts";
    import InputValidators from "~/commons/mixins/input-validators";
    import ChangePasswordRequest from "~/commons/requests/models/change-password-request";
    import SavePersonalInformationRequest from "~/commons/requests/models/save-personal-information-request";
    import PageBase from "../commons/mixins/page-base";

    @Component({
        head: {
            title: "Settings"
        }
    })
    export default class SettingsPage extends mixins(PageBase, InputValidators)
    {
        delete_account_modal: boolean = false;

        personal_information_loading: boolean = false;
        personal_information_submittable: boolean = false;
        personal_information_payload: SavePersonalInformationRequest = {
            email: "",
            given_name: "",
            family_name: "",
            phone_number: ""
        };

        password_change_loading: boolean = false;
        password_change_submittable: boolean = false;
        password_change_payload: ChangePasswordRequest = {
            current_password: "",
            new_password: "",
            confirmed_new_password: ""
        };

        get matching_password_rule(): InputRule[]
        {
            return [
                ...this.validators.required,
                ...this.validators.password,
                (_: any) => this.password_change_payload.new_password === this.password_change_payload.confirmed_new_password || "Passwords must match",
            ];
        }

        mounted()
        {
            this.personal_information_payload = {
                email: this.$account.email,
                given_name: this.$account.given_name ?? "",
                family_name: this.$account.family_name ?? "",
                phone_number: this.$account.phone_number ?? ""
            };
        }

        save_personal_information()
        {
            if (!this.personal_information_submittable || this.personal_information_loading) {
                return;
            }

            this.personal_information_loading = true;
            save_personal_information(this.personal_information_payload).then((_) => {
                this.$notify_message("Personal information updated.");
                this.$modules.identity.fetch();
            })
            .catch((_) => {
                 this.$notify_error("Unable to save personal information.");
            })
            .finally(() => {
                this.personal_information_loading = false;
            });
        }

        change_password()
        {
            if (!this.password_change_submittable || this.password_change_loading) {
                return;
            }

            this.password_change_loading = true;
            change_password(this.password_change_payload).then((_) => {
                this.$notify_message("Password has been changed.");
            })
            .catch((error: AxiosError) =>
            {
                if (error.response?.status === 401) {
                    this.$notify_error("Incorrect current password.");
                    return;
                }

                this.$notify_error("Unable to change password.");
            })
            .finally(() => {
                this.password_change_loading = false;
            });
        }
    }

</script>
