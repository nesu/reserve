<template>
    <v-container>
        <v-row>
            <v-col
                cols="12">
                <v-card
                    outlined>
                    <v-toolbar
                        flat tile
                        height="56">
                        <v-toolbar-title class="headline">
                            Create navigator element
                        </v-toolbar-title>
                        <v-spacer/>
                        <v-btn
                            text
                            color="primary"
                            nuxt exact to="/control-panel/pages">
                            Back
                        </v-btn>
                    </v-toolbar>
                    <v-divider class="mx-4"/>
                    <v-window
                        v-model="progress">
                        <v-window-item
                            :value="1">
                            <v-card-text>
                                <v-row>
                                    <v-col
                                        cols="12"
                                        md="6">
                                        <v-text-field
                                            outlined
                                            label="Title"
                                            v-model="create_navigator_element_payload.title"/>
                                        <v-text-field
                                            outlined
                                            label="Icon (MDI identifier)"
                                            v-model="create_navigator_element_payload.mdi_icon"/>
                                    </v-col>
                                </v-row>
                            </v-card-text>
                        </v-window-item>
                    </v-window>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import PageBase from "~/commons/mixins/page-base";
    import { create_navigator_element } from "~/commons/requests/layout";
    import CreateNavigatorElementRequest from "~/commons/requests/models/create-navigator-element-request";
    import { RoleType } from "~/commons/resources/role-type";

    @Component
    export default class CreateNavigatorElement extends mixins(PageBase)
    {
        progress: number = 1;

        create_navigator_element_loading: boolean = false;
        create_navigator_element_payload: CreateNavigatorElementRequest = {
            order: 0,
            title: "",
            mdi_icon: "",
            redirect_to: "",
            mobile_view: false,
            guest_only: false,
            authenticated_only: false,
            properties: {},
            permitted_roles: [RoleType.Client, RoleType.Employee, RoleType.Administrator]
        };

        additional_properties: Array<Record<string, boolean | number | string>> = [];

        get selectable_boolean_property()
        {
            return [{
                key: "true",
                value: true
            }, {
                key: "false",
                value: false
            }]
        }

        append_property()
        {
            this.additional_properties.push({
                key: "property_key",
                value: false
            });
        }

        create_navigator_element()
        {
            // @TODO: VERIFY
            if (this.create_navigator_element_loading) {
                return;
            }

            const properties: Record<string, any> = {};
            this.additional_properties.forEach((it) => {
                properties[it.key as string] = it.value;
            });

            this.create_navigator_element_loading = true;
            this.create_navigator_element_payload.properties = properties;

            create_navigator_element(this.create_navigator_element_payload).then(() => {
                this.$notify_message("Navigator element successfully created.");
                this.$modules.layout.preload_visuals();
                // @TODO: SUTVARKYTI NUORODAS Į VIENĄ VIETĄ
                this.$router.replace({ path: "/control-panel/pages" });
            })
            .catch((error) => {
                this.$notify_error(error);
            })
            .finally(() => {
                this.create_navigator_element_loading = false;
            });
        }
    }


</script>
