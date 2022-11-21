<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Locations
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
                            v-model="search_literal"
                            append-icon="mdi-magnify"
                            label="Search location"
                            dense outlined
                            clearable hide-details/>
                    </div>
                    <v-spacer/>
                    <v-dialog
                        v-authorized="$roles.Administrator"
                        v-model="creating_location"
                        persistent width="500">
                        <template v-slot:activator="{ on }">
                            <v-btn
                                v-on="on"
                                text color="primary">
                                Create location
                            </v-btn>
                        </template>
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
                                    Create new location
                                </v-toolbar-title>
                                <v-spacer/>
                                <v-btn
                                    icon
                                    class="mr-2"
                                    @click="creating_location = false">
                                    <v-icon>mdi-close</v-icon>
                                </v-btn>
                            </v-toolbar>
                            <v-divider class="mx-4"/>
                            <v-card-text>
                                <v-text-field
                                    outlined
                                    v-model="create_location_request.label"
                                    label="Location name"/>
                                <v-text-field
                                    outlined
                                    v-model="create_location_request.address"
                                    label="Address"/>
                            </v-card-text>
                            <v-divider class="mx-4"/>
                            <v-card-actions>
                                <v-spacer/>
                                <v-btn
                                    text color="primary"
                                    @click="create_location">
                                    Create
                                </v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-dialog>
                </v-toolbar>
                <v-card-text>
                    <v-data-table
                        :loading="loading"
                        v-bind="datatable_scheme">
                        <template v-slot:item.created_at="{ value }">
                            {{ $format_date(value, 'yyyy-MM-dd HH:mm') }}
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <v-btn
                                small icon
                                v-authorized="$roles.Administrator"
                                @click="modify_location(item)">
                                <v-icon>mdi-briefcase-edit</v-icon>
                            </v-btn>
                            <confirmable-modal
                                v-authorized="$roles.Administrator"
                                v-on:confirmed="delete_location(item.id)">
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
                                    Are you sure you want to remove this location?
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
        <v-dialog
            v-model="modifying_location"
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
                        @click="modifying_location = false">
                        <v-icon>mdi-close</v-icon>
                    </v-btn>
                </v-toolbar>
                <v-divider class="mx-4"/>
                <v-card-text>
                    <v-text-field
                        outlined
                        v-model="save_location_request.label"
                        label="Location name"/>
                    <v-text-field
                        outlined
                        v-model="save_location_request.address"
                        label="Address"/>
                </v-card-text>
                <v-divider class="mx-4"/>
                <v-card-actions>
                    <v-spacer/>
                    <v-btn
                        text color="primary"
                        @click="save_location">
                        Save
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </contents>
</template>
<script lang="ts">

    import Fuse from 'fuse.js';
    import DateFunctions from "~/commons/mixins/date-functions";
    import ManagementBase from "~/commons/mixins/management-base";
    import { Component, mixins, Watch } from "~/commons";
    import {
        create_location,
        delete_location,
        get_locations,
        save_location
    } from "~/commons/requests/management/locations";
    import { CreateLocationRequest } from "~/commons/requests/models/create-location-request";
    import { SaveLocationRequest } from "~/commons/requests/models/save-location-request";
    import { Location } from "~/commons/resources/location";

    @Component({
        components: {
            "confirmable-modal": require("~/components/confirmable-modal").default
        },
        head: {
            title: "Control Panel - Service Locations"
        },
        async asyncData()
        {
            const locations = await get_locations();
            return {
                locations
            }
        }
    })
    export default class ServiceLocationsPage extends mixins(ManagementBase, DateFunctions)
    {
        locations: Location[] = [];

        loading: boolean = false;
        search_literal: string = "";

        creating_location: boolean = false;
        create_location_request: CreateLocationRequest = {
            label: "",
            address: ""
        };

        modifying_location: boolean = false;
        save_location_request: SaveLocationRequest = {
            id: 0,
            label: "",
            address: ""
        }

        get datatable_scheme()
        {
            return {
                items: this.locations,
                headers: [{
                    text: "Location name",
                    value: "label",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Address",
                    value: "address",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Date created",
                    value: "created_at",
                    sortable: false,
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

        modify_location(location: Location)
        {
            this.modifying_location = true;
            this.save_location_request = {
                id: location.id,
                label: location.label,
                address: location.address
            };
        }

        delete_location(location_id: number)
        {
            this.loading = true;
            delete_location({ id: location_id })
                .then((response) => {
                    this.$notify_message("Location deleted");
                    this.load_locations();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                })
        }

        create_location()
        {
            const payload = this.create_location_request;

            this.loading = true;
            create_location(payload)
                .then((response) => {
                    this.$notify_message("New location created");
                    this.locations.push(response);
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                    this.creating_location = false;
                    this.create_location_request = {
                        label: "",
                        address: ""
                    }
                });
        }

        save_location()
        {
            const payload = this.save_location_request;

            if (payload.id === 0) {
                return;
            }

            this.loading = true;
            save_location(payload)
                .then(() => {
                    this.load_locations();
                    this.$notify_message("Location record updated");
                    this.modifying_location = false;
                    this.save_location_request = {
                        id: 0,
                        label: "",
                        address: ""
                    }
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        load_locations()
        {
            get_locations()
                .then((response) => {
                    this.locations = response;
                });
        }

        @Watch("search_literal")
        search_location(value: string | null)
        {
            if (!value || value === "")
            {
                this.loading = true;
                get_locations()
                    .then((response) => {
                        this.locations = response;
                    })
                    .finally(() => {
                        this.loading = false;
                    });

                return;
            }

            if (value.length < 3) {
                return;
            }

            const fuse = new Fuse(this.locations, {
                findAllMatches: true,
                matchAllTokens: true,
                keys: ["label", "address"]
            });

            this.locations = fuse
                .search(value)
                .map(it => it.item);
        }
    }

</script>
