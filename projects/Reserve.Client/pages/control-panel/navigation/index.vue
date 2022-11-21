<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Navigation
            </template>
        </default-toolbar>
        <v-container
            class="management-layout">
            <v-card
                outlined>
                <v-toolbar
                    class="pt-4" flat tile>
                    <v-spacer/>
                    <v-btn
                        @click="creating_navigator = true"
                        text color="primary">
                        Create navigator
                    </v-btn>
                </v-toolbar>
                <v-card-text>
                    <v-data-table
                        :items="navigator_elements"
                        v-bind="datatable_schema">
                        <template v-slot:item.permitted_roles="{ item }">
                            {{ permitted_role_of(item) }}
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <contents
                                class="d-flex flex-row justify-end">
                                <v-btn
                                    icon @click.stop="edit_navigator(item)">
                                    <v-icon>mdi-file-document-edit</v-icon>
                                </v-btn>
                                <confirmable-modal
                                    v-on:confirmed="remove_navigator(item.id)">
                                    <template v-slot:activator="{ on }">
                                        <v-btn
                                            color="red"
                                            icon v-on="on">
                                            <v-icon>mdi-delete</v-icon>
                                        </v-btn>
                                    </template>
                                    <template v-slot:modal-title>
                                        Are you sure?
                                    </template>
                                    <template v-slot:modal-body>
                                        Are you sure you want to remove this navigator element?
                                    </template>
                                    <template v-slot:modal-confirmation-text>
                                        Confirm
                                    </template>
                                </confirmable-modal>
                            </contents>
                        </template>
                    </v-data-table>
                </v-card-text>
            </v-card>
        </v-container>
        <navigator-creator
            v-if="creating_navigator"
            :active.sync="creating_navigator"
            @update:navigators="load_navigators"/>
        <navigator-editor
            v-if="editing_navigator.active"
            :active.sync="editing_navigator.active"
            :navigator_id="editing_navigator.context"
            @update:navigators="load_navigators"/>
    </contents>
</template>
<script lang="ts">

    import { Component, mixins, Watch } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import ManagementBase from "~/commons/mixins/management-base";
    import { delete_navigator_element, get_created_pages, get_navigator_elements } from "~/commons/requests/layout";
    import NavigatorElementResource from "~/commons/resources/navigator-element-resource";
    import Page from "~/commons/resources/page";

    @Component({
        head: {
            title: "Control Panel - Navigation"
        },
        components: {
            "navigator-creator": require("~/components/management/navigator-creator").default,
            "navigator-editor": require("~/components/management/navigator-editor").default,
            "confirmable-modal": require("~/components/confirmable-modal").default,
        },
        async asyncData()
        {
            const navigator_elements = await get_navigator_elements();
            return {
                navigator_elements
            }
        }
    })
    export default class CustomersPage extends mixins(ManagementBase, DateFunctions)
    {
        loading: boolean = false;
        navigator_elements: NavigatorElementResource[] = [];

        creating_navigator: boolean = false;
        editing_navigator: any = {
            active: false,
            context: 0
        };

        remove_navigator(nid: number)
        {
            this.loading = true;
            delete_navigator_element({ id: nid })
                .then((response) => {
                    this.$notify_message("Navigator element removed");
                    this.load_navigators();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        edit_navigator(element: NavigatorElementResource)
        {
            this.editing_navigator = {
                active: true,
                context: element.id
            }
        }

        load_navigators()
        {
            this.loading = true;
            get_navigator_elements()
                .then((response) => {
                    this.navigator_elements = response;
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        permitted_role_of(element: NavigatorElementResource)
        {
            if (element.guest_only) {
                return "Guests only";
            }

            if (element.authenticated_only)
            {
                const roles: string[] = [];
                this.$readable_role_array
                    .forEach((it) =>
                    {
                        if (element.permitted_roles.includes(it.value)) {
                            roles.push(it.role);
                        }
                    });

                if (roles.length === 0) {
                    return "Not restricted";
                }

                return roles.join(", ");
            }

            return "Not restricted";
        }

        get datatable_schema()
        {
            // noinspection UnnecessaryLocalVariableJS
            return {
                headers: [{
                    text: "Title",
                    value: "title",
                    sortable: false,
                    filterable: true
                }, {
                    text: "Redirects to",
                    value: "redirect_to",
                    align: "left",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Permitted roles",
                    value: "permitted_roles",
                    align: "left",
                    sortable: true,
                    filterable: false
                }, {
                    text: "Actions",
                    value: "actions",
                    align: "right",
                    sortable: false,
                    filterable: false
                }]
            };
        }
    }

</script>
