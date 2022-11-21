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
                    <v-subheader>
                        Appearance
                    </v-subheader>
                    <v-card-text>
                        <v-row>
                            <v-col
                                cols="12"
                                md="6">
                                <v-text-field
                                    outlined
                                    label="Title"
                                    v-model="edit_navigator_element_payload.title"/>
                                <v-text-field
                                    outlined
                                    label="Icon (MDI identifier)"
                                    v-model="edit_navigator_element_payload.mdi_icon"/>
                            </v-col>
                        </v-row>
                    </v-card-text>
                    <v-subheader>
                        Visibility options
                    </v-subheader>
                    <v-card-text>
                        <v-row>
                            <v-col
                                cols="12"
                                md="6">
                                <v-checkbox
                                    label="Mobile view"
                                    v-model="edit_navigator_element_payload.mobile_view"
                                    hide-details/>
                                <v-checkbox
                                    label="Visible only for guests"
                                    v-model="edit_navigator_element_payload.guest_only"
                                    hide-details/>
                                <v-checkbox
                                    v-model="edit_navigator_element_payload.authenticated_only"
                                    label="Visible only for logged in users"/>
                                <v-select
                                    label="Required role(s)"
                                    messages="Applied only if element requires authentication"
                                    outlined color="primary"
                                    :disabled="!edit_navigator_element_payload.authenticated_only"
                                    :items="selectable_role_array"
                                    v-model="edit_navigator_element_payload.permitted_roles"
                                    item-text="key"
                                    item-value="value"
                                    chips multiple/>
                            </v-col>
                        </v-row>
                    </v-card-text>
                    <v-subheader>
                        Functional options
                    </v-subheader>
                    <v-card-text>
                        <v-row>
                            <v-col
                                cols="12"
                                md="6">
                                <v-text-field
                                    outlined
                                    v-model="edit_navigator_element_payload.redirect_to"
                                    label="Destination URL"/>
                            </v-col>
                        </v-row>
                    </v-card-text>
                    <v-subheader>
                        Optional properties
                    </v-subheader>
                    <v-card-text>
                        <v-row>
                            <v-col
                                cols="12"
                                md="6">
                                <v-btn
                                    v-if="additional_properties.length === 0"
                                    @click.stop="append_property"
                                    text color="primary">
                                    Add property
                                </v-btn>
                                <v-row class="flex-column">
                                    <v-col
                                        col="12"
                                        :key="index"
                                        v-for="(property, index) in additional_properties">
                                        <v-row no-gutters>
                                            <v-col cols="6">
                                                <v-text-field
                                                    outlined label="Property key"
                                                    v-model="additional_properties[index].key"
                                                    dense hide-details/>
                                            </v-col>
                                            <v-col cols="3">
                                                <v-select
                                                    outlined
                                                    v-model="additional_properties[index].value"
                                                    :items="selectable_boolean_property"
                                                    item-value="value"
                                                    item-text="key"
                                                    dense hide-details/>
                                            </v-col>
                                            <v-col
                                                class="d-flex align-center justify-center"
                                                cols="3">
                                                <v-btn
                                                    @click.stop="append_property"
                                                    color="primary"
                                                    icon small>
                                                    <v-icon>
                                                        mdi-plus-circle
                                                    </v-icon>
                                                </v-btn>
                                            </v-col>
                                        </v-row>
                                    </v-col>
                                </v-row>
                            </v-col>
                        </v-row>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn
                            color="primary"
                            outlined large
                            @click.stop="edit_navigator_element">
                            Save changes
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import PageBase from "~/commons/mixins/page-base";
    import { create_navigator_element, edit_navigator_element, get_navigator_element } from "~/commons/requests/layout";
    import CreateNavigatorElementRequest from "~/commons/requests/models/create-navigator-element-request";
    import NavigatorElementResource from "~/commons/resources/navigator-element-resource";
    import { RoleType } from "~/commons/resources/role-type";

    @Component
    export default class CreateNavigatorElement extends mixins(PageBase)
    {
        edit_navigator_element_loading: boolean = false;
        edit_navigator_element_payload: NavigatorElementResource = {
            id: 0,
            order: 0,
            title: "",
            mdi_icon: "",
            redirect_to: "",
            mobile_view: false,
            guest_only: false,
            authenticated_only: false,
            permitted_roles: [RoleType.Client, RoleType.Employee, RoleType.Administrator],
            children: [],
            properties: {}
        };

        additional_properties: Array<Record<string, boolean | number | string>> = [];

        mounted()
        {
            const identifier = parseInt(this.$route.params.identifier);

            if (!identifier) {
                this.$router.replace({ path: "/control-panel/pages" });
                return;
            }

            get_navigator_element(identifier).then((element) =>
            {
                this.additional_properties = [];
                const properties = element.properties;

                this.edit_navigator_element_payload = element;

                if (!!properties)
                {
                    Object.keys(properties).forEach((it: string) =>
                    {
                        this.additional_properties.push({
                            key: it,
                            value: properties[it]
                        })
                    });
                }
            })
            .catch((error) => {
                this.$router.replace({ path: "/control-panel/pages" });
            });
        }

        append_property()
        {
            this.additional_properties.push({
                key: "property_key",
                value: false
            });
        }

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

        get selectable_role_array()
        {
            const role_array: any[] = [];
            const roles = this.$roles as Record<string, number>;

            Object.keys(roles).forEach((role_key: string) =>
            {
                role_array.push({
                    key: role_key,
                    value: roles[role_key]
                });
            });

            return role_array;
        }

        edit_navigator_element()
        {
            // @TODO: VERIFY
            if (this.edit_navigator_element_loading) {
                return;
            }

            const properties: Record<string, any> = {};
            this.additional_properties.forEach((it) => {
                properties[it.key as string] = it.value;
            });

            this.edit_navigator_element_loading = true;
            this.edit_navigator_element_payload.properties = properties;

            edit_navigator_element(this.edit_navigator_element_payload).then(() => {
                this.$notify_message("Navigator element was changed.");
                this.$modules.layout.preload_visuals();
                // @TODO: SUTVARKYTI NUORODAS Į VIENĄ VIETĄ
                this.$router.replace({ path: "/control-panel/pages" });
            })
            .catch((error) => {
                this.$notify_error(error);
            })
            .finally(() => {
                this.edit_navigator_element_loading = false;
            });
        }
    }


</script>
