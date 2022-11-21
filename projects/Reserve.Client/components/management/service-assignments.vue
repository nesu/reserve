<template>
    <v-dialog
        v-model="active"
        persistent width="700">
        <v-card
            v-if="loading"
            :loading="loading"
            tile flat>
            <v-card-text class="text-center">
                <v-progress-circular
                    size="34"
                    indeterminate/>
            </v-card-text>
        </v-card>
        <v-card
            v-else
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
                    Service assignments
                </v-toolbar-title>
                <v-spacer/>
                <v-dialog
                    v-model="creating_assignment"
                    width="500"
                    persistent>
                    <template v-slot:activator="{ on }">
                        <v-btn
                            icon
                            v-on="on">
                            <v-icon>mdi-briefcase-edit</v-icon>
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
                                Create assignment
                            </v-toolbar-title>
                            <v-spacer/>
                            <v-btn
                                icon
                                class="mr-2"
                                @click="creating_assignment = false">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-toolbar>
                        <v-divider class="mx-4"/>
                        <v-card-text>
                            <v-form
                                v-model="submittable">
                                <v-autocomplete
                                    v-model="create_assignment_request.assignee_id"
                                    :items="employees"
                                    :item-text="(it) => `${it.given_name} ${it.family_name}`"
                                    :rules="validators.required"
                                    item-value="id"
                                    outlined label="Employee"/>
                                <v-autocomplete
                                    v-model="create_assignment_request.location_id"
                                    :items="locations"
                                    :rules="validators.required"
                                    item-text="label"
                                    item-value="id"
                                    outlined label="Location"/>
                            </v-form>
                        </v-card-text>
                        <v-divider class="mx-4"/>
                        <v-card-actions>
                            <v-spacer/>
                            <v-btn
                                @click="create_assignment"
                                text color="primary">
                                Create
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
                <v-btn
                    icon
                    class="mr-2"
                    @click="close">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-row>
                    <v-col
                        cols="12">
                        <div
                            v-if="!service_assignments || Array.isArray(service_assignments) && service_assignments.length === 0"
                            class="text-center">
                            No assignments created
                        </div>
                        <v-simple-table
                            v-else
                            fixed-header
                            height="200">
                            <template v-slot:default>
                                <thead>
                                <tr>
                                    <th class="text-left">
                                        Service
                                    </th>
                                    <th class="text-left">
                                        Assignee
                                    </th>
                                    <th class="text-left">
                                        Location
                                    </th>
                                    <th class="text-right">
                                        Actions
                                    </th>
                                </tr>
                                </thead>
                                <tbody>
                                <tr v-for="(it, index) in service_assignments" :key="it.id">
                                    <td>{{ it.service.label }}</td>
                                    <td>{{ it.assignee.given_name }} {{ it.assignee.family_name }}</td>
                                    <td>{{ it.location.label }}</td>
                                    <td class="text-right">
                                        <confirmable-modal
                                            v-on:confirmed="delete_assignment(it.id)">
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
                                                Are you sure you want to delete this service assignment?
                                            </template>
                                            <template v-slot:modal-confirmation-text>
                                                Delete
                                            </template>
                                        </confirmable-modal>
                                    </td>
                                </tr>
                                </tbody>
                            </template>
                        </v-simple-table>
                    </v-col>
                </v-row>
            </v-card-text>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">

    import { Component, mixins, Property } from "~/commons";
    import ComponentBase from "~/commons/mixins/component-base";
    import InputValidators from "~/commons/mixins/input-validators";
    import { get_account_collection } from "~/commons/requests/management/accounts";
    import { get_locations } from "~/commons/requests/management/locations";
    import {
        create_service_assignment,
        delete_service_assignment,
        get_service_assignments
    } from "~/commons/requests/management/services";
    import { CreateServiceAssignment } from "~/commons/requests/models/create-service-assignment";
    import Account from "~/commons/resources/account";
    import { Location } from "~/commons/resources/location";
    import { RoleType } from "~/commons/resources/role-type";
    import { ServiceAssignee } from "~/commons/resources/service-assignee";

    @Component({
        components: {
            "confirmable-modal": require("~/components/confirmable-modal").default
        },
    })
    export default class ServiceAssignmentEditor extends mixins(ComponentBase, InputValidators)
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
        service_id!: number;

        loading: boolean = true;
        submittable: boolean = false;

        employees: Account[] = [];
        locations: Location[] = [];
        service_assignments: ServiceAssignee[] = [];

        creating_assignment: boolean = false;
        create_assignment_request: CreateServiceAssignment = {
            service_id: this.service_id,
            assignee_id: 0,
            location_id: 0
        }

        created()
        {
            if (!this.service_id) {
                this.close();
                return;
            }

            this.load_service_assignments();

            get_account_collection({
                role: RoleType.Employee,
                page_index: 1,
                page_size: 0
            })
            .then((response) => {
                this.employees = response;
            });

            get_locations().then((response) => {
                this.locations = response;
            });
        }

        create_assignment()
        {
            if (!this.submittable) {
                return;
            }

            this.loading = true;
            const payload = this.create_assignment_request;

            create_service_assignment(payload)
                .then(() =>
                {
                    this.load_service_assignments();
                    this.$notify_message("Assignment created");

                    this.creating_assignment = false;
                    this.create_assignment_request = {
                        service_id: this.service_id,
                        assignee_id: 0,
                        location_id: 0
                    };
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                })
        }

        delete_assignment(assignment_id: number)
        {
            delete_service_assignment({ id: assignment_id })
                .then(() => {
                    this.$notify_message("Assignment deleted");
                    this.load_service_assignments();
                });
        }

        close() {
            this.$emit("update:active", false);
        }

        load_service_assignments()
        {
            this.loading = true;
            get_service_assignments(this.service_id)
                .then((response) => {
                    this.service_assignments = response;
                })
                .finally(() => {
                    this.loading = false;
                });
        }
    }

</script>
