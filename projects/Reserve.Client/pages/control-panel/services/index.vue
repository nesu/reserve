<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Services
            </template>
        </default-toolbar>
        <v-container
            class="management-layout">
            <v-card
                outlined>
                <v-toolbar
                    class="pt-4" flat tile>
                    <div
                        class="d-flex flex-row-reverse px-4">
                        <v-text-field
                            append-icon="mdi-magnify"
                            v-model="search_literal"
                            label="Search service"
                            dense outlined
                            hide-details/>
                    </div>
                    <v-spacer/>
                    <v-btn
                        v-authorized="$roles.Administrator"
                        text
                        @click="creating_service = true"
                        color="primary">
                        Create service
                    </v-btn>
                </v-toolbar>
                <v-card-text>
                    <v-data-table
                        :loading="loading"
                        v-bind="datatable_scheme">
                        <template v-slot:item.price="{ value }">
                            {{ value }}
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <v-btn
                                v-authorized="$roles.Administrator"
                                small icon
                                @click="modify_service_assignments(item)">
                                <v-icon>mdi-account-edit</v-icon>
                            </v-btn>
                            <v-btn
                                v-authorized="$roles.Administrator"
                                small icon
                                @click="modify_service(item)">
                                <v-icon>mdi-briefcase-edit</v-icon>
                            </v-btn>
                            <confirmable-modal
                                v-authorized="$roles.Administrator"
                                v-on:confirmed="delete_service(item.id)">
                                <template v-slot:activator="{ on }">
                                    <v-btn
                                        small icon color="red"
                                        v-on="on">
                                        <v-icon>mdi-delete</v-icon>
                                    </v-btn>
                                </template>
                                <template v-slot:modal-title>
                                    Are you sure?
                                </template>
                                <template v-slot:modal-body>
                                    Are you sure you want to remove this service?
                                </template>
                                <template v-slot:modal-confirmation-text>
                                    Delete
                                </template>
                            </confirmable-modal>
                            <span
                                v-authorized="$roles.Employee">
                                -
                            </span>
                        </template>
                    </v-data-table>
                </v-card-text>
            </v-card>
        </v-container>

        <service-assignments
            v-if="modifying_service_assignments"
            :active.sync="modifying_service_assignments"
            :service_id="service_id"/>

        <service-creator
            v-if="creating_service"
            :active.sync="creating_service"
            @updated:services="load_services"/>

        <v-dialog
            v-model="modifying_service"
            persistent width="500">
            <v-card
                flat tile>
                <v-toolbar
                    flat
                    fixed
                    height="76"
                    class="pl-4"
                    color="transparent"
                    width="100%"
                >
                    <v-toolbar-title>
                        Edit location
                    </v-toolbar-title>
                    <v-spacer/>
                    <v-btn
                        icon class="mr-2"
                        @click="modifying_service = false">
                        <v-icon>mdi-close</v-icon>
                    </v-btn>
                </v-toolbar>
                <v-divider class="mx-4"/>
                <v-card-text>
                    <v-form
                        v-model="submittable">
                        <v-text-field
                            outlined
                            v-model="save_service_request.label"
                            :rules="validators.required"
                            label="Service name"/>
                        <v-text-field
                            outlined
                            v-model="save_service_request.price"
                            :rules="validators.number"
                            label="Price (€)"/>
                        <v-text-field
                            outlined
                            v-model="save_service_request.duration"
                            :rules="validators.number"
                            label="Duration (minutes)"/>
                        <v-autocomplete
                            outlined
                            v-model="save_service_request.category_id"
                            :items="categories"
                            :rules="validators.required"
                            item-text="label"
                            item-value="id"
                            label="Category"
                        ></v-autocomplete>
                        <v-textarea
                            outlined
                            v-model="save_service_request.description"
                            label="Description"/>
                    </v-form>
                </v-card-text>
                <v-divider class="mx-4"/>
                <v-card-actions>
                    <v-spacer/>
                    <v-btn
                        text color="primary"
                        @click="save_service">
                        Save
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </contents>
</template>
<script lang="ts">

    import Fuse from "fuse.js";
    import { Component, mixins, Watch } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import InputValidators from "~/commons/mixins/input-validators";
    import ManagementBase from "~/commons/mixins/management-base";
    import { get_categories } from "~/commons/requests/management/categories";
    import { delete_service, get_services, save_service } from "~/commons/requests/management/services";
    import { SaveServiceRequest } from "~/commons/requests/models/save-service-request";
    import { Service } from "~/commons/resources/service";
    import { ServiceCategory } from "~/commons/resources/service-category";

    @Component({
        head: {
            title: "Control Panel - Services"
        },
        components: {
            "service-assignments": require("~/components/management/service-assignments").default,
            "service-creator": require("~/components/management/service-creator").default,
            "confirmable-modal": require("~/components/confirmable-modal").default
        },
        async asyncData()
        {
            const services = await get_services();
            const categories = await get_categories();

            return {
                services,
                categories
            }
        }
    })
    export default class ServicesPage extends mixins(ManagementBase, DateFunctions, InputValidators)
    {
        submittable: boolean = false;

        loading: boolean = false;
        services: Service[] = [];
        categories: ServiceCategory[] = [];

        search_literal: string = "";
        creating_service: boolean = false;

        modifying_service: boolean = false;
        save_service_request: SaveServiceRequest = {
            id: 0,
            label: "",
            description: null,
            duration: 0,
            price: 0,
            category_id: 0
        }

        modifying_service_assignments: boolean = false;
        service_id: number = 0;

        get datatable_scheme()
        {
            return {
                items: this.services,
                headers: [{
                    text: "Service",
                    value: "label",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Price (€)",
                    value: "price",
                    sortable: true,
                    filterable: false
                }, {
                    text: "Duration (minutes)",
                    value: "duration",
                    sortable: true,
                    filterable: false
                }, {
                    text: "Actions",
                    align: "right",
                    value: "actions",
                    sortable: false,
                    filterable: false
                }]
            }
        }

        modify_service(service: Service)
        {
            this.modifying_service = true;
            this.save_service_request = {
                id: service.id,
                label: service.label,
                description: service.description,
                price: service.price,
                duration: service.duration,
                category_id: service.category_id
            };
        }

        modify_service_assignments(service: Service)
        {
            this.service_id = service.id;
            this.modifying_service_assignments = true;
        }

        save_service()
        {
            if (!this.submittable) {
                return;
            }

            const payload = this.save_service_request;

            if (payload.description != null && payload.description.length === 0) {
                payload.description = null;
            }

            this.loading = true;
            save_service(payload)
                .then(() => {

                    this.modifying_service = false;
                    this.save_service_request = {
                        id: 0,
                        label: "",
                        description: null,
                        duration: 0,
                        price: 0,
                        category_id: 0
                    }

                    this.$notify_message("Service updated");
                    this.load_services();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        delete_service(service_id: number)
        {
            this.loading = true;
            delete_service({ id: service_id })
                .then(() => {
                    this.$notify_message("Service deleted")
                    this.load_services();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                })
        }

        load_services()
        {
            get_services()
                .then((response) => {
                    this.services = response;
                });
        }

        @Watch("search_literal")
        search_category(value: string | null)
        {
            if (!value || value === "")
            {
                this.loading = true;
                get_services()
                    .then((response) => {
                        this.services = response;
                    })
                    .finally(() => {
                        this.loading = false;
                    });

                return;
            }

            if (value.length < 3) {
                return;
            }

            const fuse = new Fuse(this.services, {
                findAllMatches: true,
                matchAllTokens: true,
                keys: ["label"]
            });

            this.services = fuse
                .search(value)
                .map(it => it.item);
        }
    }

</script>
