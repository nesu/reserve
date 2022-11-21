<template>
    <v-dialog
        v-model="active"
        fullscreen persistent>
        <v-sheet
            height="100%">
            <v-container
                class="narrowed fill-height">
                <v-card
                    v-if="!loading"
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
                                Create reservation
                            </v-toolbar-title>
                            <v-spacer/>
                            <v-btn
                                icon
                                class="mr-2"
                                @click="close">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-toolbar>
                    </div>
                    <v-divider class="mx-4"/>
                    <div
                        class="flex-grow-1">
                        <v-card-text>
                            <v-alert
                                type="warning">
                                Clients without valid contact info are not displayed!
                            </v-alert>
                            <v-autocomplete
                                outlined label="Client"
                                :items="accounts"
                                :item-text="(it) => `${it.given_name} ${it.family_name} (${it.email})`"
                                return-object
                                @change="init_account"/>
                        </v-card-text>
                        <v-card-text>
                            <v-autocomplete
                                outlined label="Employee"
                                :items="employees"
                                :item-text="(it) => `${it.given_name} ${it.family_name}`"
                                item-value="id"
                                @change="load_employee_services"/>
                            <v-select
                                multiple hide-details
                                v-model="create_remote_reservation.services"
                                outlined label="Services"
                                item-value="id"
                                item-text="label"
                                :disabled="services.length === 0"
                                :items="services"/>
                        </v-card-text>
                        <v-card-text>
                            <v-text-field
                                outlined
                                v-if="datetime !== null"
                                readonly label="Datetime"
                                :value="$format_date(datetime, 'yyyy-MM-dd HH:mm')"/>
                            <datetime-select
                                v-else
                                :datetime.sync="create_remote_reservation.datetime"
                                :timespan="services_duration_total"
                                :employee_id="create_remote_reservation.employee_id"/>
                        </v-card-text>
                    </div>
                    <v-divider class="mx-4"/>
                    <v-card-actions>
                        <v-spacer/>
                        <v-btn
                            @click.stop="create_reservation"
                            :loading="creating_reservation"
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


    import { sumBy } from "lodash-es";
    import { Component, mixins, Property } from "~/commons";
    import ComponentBase from "~/commons/mixins/component-base";
    import DateFunctions from "~/commons/mixins/date-functions";
    import { get_service_collection_by_employee } from "~/commons/requests/general/employees";
    import { get_account_collection } from "~/commons/requests/management/accounts";
    import { create_remote_reservation } from "~/commons/requests/management/services";
    import { RemoteReservationRequest } from "~/commons/requests/models/remote-reservation-request";
    import Account from "~/commons/resources/account";
    import { RoleType } from "~/commons/resources/role-type";
    import { Service } from "~/commons/resources/service";

    @Component({
        components: {
            "datetime-select": require("~/components/datetime-select").default,
            "confirmable-modal": require("~/components/confirmable-modal").default,
        }
    })
    export default class ReservationCreator extends mixins(ComponentBase, DateFunctions)
    {
        @Property({
            type: Boolean,
            required: true
        })
        active!: boolean;

        @Property({
            type: String,
            required: false,
            default: null
        })
        datetime!: string | null;

        loading: boolean = false;
        creating_reservation: boolean = false;

        accounts: Account[] = [];
        employees: Account[] = [];
        services: Service[] = [];

        create_remote_reservation: RemoteReservationRequest = {
            account_id: 0,
            employee_id: 0,
            datetime: "",
            services: []
        };

        mounted()
        {
            this.loading = true;
            get_account_collection({
                role: RoleType.Client,
                page_index: 1,
                page_size: 0
            })
            .then((response) =>
            {
                this.accounts = response.filter((it) => {
                    return !!it.family_name && !!it.given_name && !!it.email
                });

                return get_account_collection({
                    role: RoleType.Employee,
                    page_index: 1,
                    page_size: 0
                })
                .then((response) => {
                    this.employees = response;
                });
            })
            .finally(() => {
                this.loading = false;
            });

            if (!!this.datetime) {
                this.create_remote_reservation.datetime = this.datetime;
            }
        }

        create_reservation()
        {
            this.creating_reservation = true;
            create_remote_reservation(this.create_remote_reservation)
                .then((response) => {
                    this.$notify_message("Reservation created successfully");
                    this.$emit("updated:reservations");
                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.creating_reservation = false;
                });
        }

        init_account(account: Account) {
            this.create_remote_reservation.account_id = account.id;
        }

        load_employee_services(eid: number)
        {
            this.create_remote_reservation.employee_id = eid;
            get_service_collection_by_employee(eid)
                .then((response) => {
                    this.services = response;
                });
        }

        close() {
            this.$emit("update:active", false);
        }

        get services_duration_total()
        {
            const services = this.services.filter((it) => {
                return this.create_remote_reservation.services.includes(it.id);
            });

            return sumBy(services, (it) => {
                return it.duration;
            });
        }
    }

</script>
