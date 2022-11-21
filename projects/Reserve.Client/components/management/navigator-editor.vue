<template>
    <v-dialog
        v-model="active"
        fullscreen persistent>
        <v-sheet
            height="100%">
            <v-container
                class="narrowed fill-height">
                <v-card
                    class="d-flex flex-column"
                    height="100%" width="100%"
                    flat tile>
                    <div
                        class="flex-shrink-1">
                        <v-toolbar
                            flat
                            fixed
                            height="76"
                            class="pl-4"
                            color="transparent"
                            width="100%">
                            <v-toolbar-title>
                                Edit navigator element
                            </v-toolbar-title>
                            <v-spacer/>
                            <v-btn
                                icon class="mr-2"
                                @click="close">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-toolbar>
                    </div>
                    <v-divider class="mx-4"/>
                    <div
                        class="pt-6 flex-grow-1">
                        <v-card-text>
                            <v-text-field
                                outlined
                                label="Title"
                                v-model="navigator_element.title"/>
                            <v-text-field
                                outlined
                                label="Icon identifier"
                                hide-details="auto"
                                v-model="navigator_element.mdi_icon"/>
                            <span
                                class="overline">
                                <a target="_blank" href="https://materialdesignicons.com/">
                                    Supported icons
                                </a>
                            </span>
                            <v-text-field
                                outlined class="mt-4"
                                v-model="navigator_element.redirect_to"
                                hide-details label="Destination URL"/>
                            <span
                                class="overline">
                                <v-dialog
                                    width="500"
                                    v-model="selecting_page">
                                    <template v-slot:activator="{ on }">
                                        <a v-on="on">
                                            Select page
                                        </a>
                                    </template>
                                    <v-card
                                        flat tile>
                                        <v-card-title>
                                            Redirect to...
                                        </v-card-title>
                                        <v-card-text
                                            v-if="pages.length === 0"
                                            class="text-center">
                                            Currently there are no pages...
                                        </v-card-text>
                                        <v-card-text
                                            v-else>
                                            <v-select
                                                outlined
                                                :items="pages"
                                                v-model="navigator_element.redirect_to"
                                                :item-value="(it) => `/pages/${it.identifier}`"
                                                @change="selecting_page = false"
                                                item-text="title"/>
                                        </v-card-text>
                                    </v-card>
                                </v-dialog>
                            </span>
                            <v-text-field
                                outlined class="mt-4"
                                label="Order of appearance"
                                v-model="navigator_element.order"/>
                        </v-card-text>
                        <v-card-text>
                            <v-checkbox
                                label="Mobile view"
                                v-model="navigator_element.mobile_view"
                                hide-details/>
                            <v-checkbox
                                label="Visible only for guests"
                                v-model="navigator_element.guest_only"
                                hide-details/>
                            <v-checkbox
                                v-model="navigator_element.authenticated_only"
                                label="Visible only for logged in users"/>
                        </v-card-text>
                        <v-card-text>
                            <v-select
                                label="Required role(s)"
                                messages="Applied only if element requires authentication"
                                outlined color="primary"
                                :disabled="!navigator_element.authenticated_only"
                                :items="$readable_role_array"
                                v-model="navigator_element.permitted_roles"
                                item-text="role"
                                item-value="value"
                                chips multiple/>
                        </v-card-text>
                    </div>
                    <v-divider class="mx-4"/>
                    <v-card-actions>
                        <v-spacer/>
                        <v-btn
                            @click.stop="save_navigator"
                            color="primary">
                            Submit
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-container>
        </v-sheet>
    </v-dialog>
</template>
<script lang="ts">

    import { Component, Property, Watch } from "~/commons";
    import {
        create_navigator_element,
        edit_navigator_element,
        get_created_pages,
        get_navigator_element
    } from "~/commons/requests/layout";
    import CreateNavigatorElementRequest from "~/commons/requests/models/create-navigator-element-request";
    import NavigatorElementResource from "~/commons/resources/navigator-element-resource";
    import Page from "~/commons/resources/page";
    import { RoleType } from "~/commons/resources/role-type";
    import { mixins } from "../../commons";
    import InputValidators from "../../commons/mixins/input-validators";
    import PageBase from "../../commons/mixins/page-base";

    @Component
    export default class NavigatorCreatorComponent extends mixins(PageBase, InputValidators)
    {
        pages: Page[] = [];
        saving: boolean = false;
        selecting_page: boolean = false;

        @Property({
            type: Boolean,
            required: true
        })
        active!: boolean;

        @Property({
            type: Number,
            required: true
        })
        navigator_id!: number;

        navigator_element: NavigatorElementResource = {
            id: 0,
            order: 0,
            title: "",
            mdi_icon: "",
            redirect_to: "",
            mobile_view: false,
            guest_only: false,
            authenticated_only: false,
            permitted_roles: [],
            children: [],
            properties: {
                text: true
            }
        };

        mounted()
        {
            get_navigator_element(this.navigator_id)
                .then((response) => {
                    this.navigator_element = response;
                    return get_created_pages()
                            .then((response) => {
                                this.pages = response;
                            });
                });
        }

        save_navigator()
        {
            if (!this.navigator_element) {
                return;
            }

            this.saving = true;
            this.navigator_element.properties = {
                text: true
            };

            edit_navigator_element(this.navigator_element)
                .then(() => {
                    this.$notify_message("Navigator element changes were saved");
                    this.$emit("update:navigators");
                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.saving = false;
                });
        }

        close() {
            this.$emit("update:active", false);
        }

        @Watch("navigator_element.guest_only")
        guest_only_watcher(value: boolean)
        {
            if (value) {
                this.navigator_element.permitted_roles = [];
                this.navigator_element.authenticated_only = false;
            }
        }

        @Watch("navigator_element.authenticated_only")
        authenticated_only_watcher(value: boolean)
        {
            if (value) {
                this.navigator_element.guest_only = false;
            }
        }
    }


</script>
