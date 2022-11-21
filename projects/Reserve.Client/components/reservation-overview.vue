<template>
    <v-dialog
        v-model="active"
        @click:outside="close"
        width="700">
        <v-card
            v-if="reservation != null"
            flat tile>
            <v-card-title>
                Reservation overview
            </v-card-title>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-list>
                    <v-list-item>
                        <v-list-item-icon>
                            <v-icon>
                                mdi-calendar
                            </v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                Reservation datetime:
                            </v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-content class="text-right">
                            <v-list-item-title>
                                {{ $format_date(reservation.event.start_at, "yyyy-MM-dd HH:mm") }}
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                        <v-list-item-icon>
                            <v-icon>
                                mdi-timer-sand
                            </v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                Reservation duration:
                            </v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-content class="text-right">
                            <v-list-item-title>
                                {{ duration_of(reservation) }} minutes
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                        <v-list-item-icon>
                            <v-icon>
                                mdi-account
                            </v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                Specialist:
                            </v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-content class="text-right">
                            <v-list-item-title>
                                {{ reservation.assignee.given_name }} {{ reservation.assignee.family_name }}
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                </v-list>
            </v-card-text>
            <v-divider class="mx-4"/>
            <v-divider class="mx-4"/>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-list>
                    <v-list-item
                        v-for="s in reservation.services"
                        :key="s.service_id">
                        <v-list-item-icon>
                            <v-icon>
                                mdi-chevron-right
                            </v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                {{ s.service.label }}
                            </v-list-item-title>
                        </v-list-item-content>
                        <v-list-item-content
                            class="text-right">
                            <v-list-item-title>
                                {{ s.service.price }}&euro;
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                </v-list>
            </v-card-text>
            <v-divider class="mx-4"/>
            <v-card-actions>
                <v-row>
                    <v-col
                        cols="12"
                        md="6">
                        <v-btn
                            @click.stop="rescheduling_modal = true"
                            outlined color="primary">
                            Reschedule
                        </v-btn>
                    </v-col>
                    <v-col
                        class="d-flex justify-end"
                        cols="12"
                        md="6">
                        <confirmable-modal
                            v-on:confirmed="cancel_reservation(reservation.id)">
                            <template v-slot:activator="{ on }">
                                <v-btn
                                    v-on="on"
                                    outlined color="red">
                                    Cancel
                                </v-btn>
                            </template>
                            <template v-slot:modal-title>
                                Are you sure?
                            </template>
                            <template v-slot:modal-body>
                                Are you sure you want to cancel this reservation?
                            </template>
                            <template v-slot:modal-cancellation-text>
                                Close
                            </template>
                            <template v-slot:modal-confirmation-text>
                                Confirm
                            </template>
                        </confirmable-modal>
                    </v-col>
                </v-row>

                <v-dialog
                    v-model="rescheduling_modal"
                    width="400">
                    <v-card
                        flat tile>
                        <v-card-title>
                            Reschedule reservation
                        </v-card-title>
                        <v-divider class="mx-4"/>
                        <v-card-text>
                            <datetime-select
                                :datetime.sync="reschedule_reservation_request.datetime"
                                :timespan="duration_of(reservation)"
                                :employee_id="reservation.assignee_id"/>
                        </v-card-text>
                        <v-divider class="mx-4"/>
                        <v-card-actions>
                            <v-btn
                                @click.stop="rescheduling_modal = false"
                                text color="primary">
                                Close
                            </v-btn>
                            <v-spacer/>
                            <v-btn
                                @click="reschedule_reservation(reservation.id)"
                                outlined color="red">
                                Confirm
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">

    import { sumBy } from "lodash-es";
    import {
        cancel_reservation,
        get_reservation,
        reschedule_reservation
    } from "~/commons/requests/accounts/reservations";
    import { ResecheduleReservationRequest } from "~/commons/requests/models/resechedule-reservation-request";
    import { Reservation } from "~/commons/resources/reservation";
    import { Component, mixins, Property } from "../commons";
    import ComponentBase from "../commons/mixins/component-base";
    import DateFunctions from "../commons/mixins/date-functions";

    @Component({
        components: {
            "datetime-select": require("~/components/datetime-select").default,
            "confirmable-modal": require("~/components/confirmable-modal").default,
        }
    })
    export default class ReservationOverviewComponent extends mixins(ComponentBase, DateFunctions)
    {
        @Property({
            type: Boolean,
            required: true
        })
        active!: boolean;

        @Property({
            type: Number,
            required: true
        })
        context!: number;

        loading: boolean = false;
        reservation: Reservation | null = null;

        rescheduling_modal: boolean = false;
        reschedule_reservation_request: ResecheduleReservationRequest = {
            datetime: "",
            reservation_id: 0
        }

        mounted() {
            this.load_reservation();
        }

        duration_of(reservation: Reservation)
        {
            return sumBy(reservation.services, (it) => {
                return it.service.duration
            });
        }

        cancel_reservation(rid: number)
        {
            cancel_reservation({ reservation_id: rid })
                .then(() =>
                {
                    this.$notify_message("Reservation canceled");
                    this.request_reload();
                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error)
                });
        }

        reschedule_reservation(rid: number)
        {
            this.reschedule_reservation_request.reservation_id = rid;

            reschedule_reservation(this.reschedule_reservation_request)
                .then(() =>
                {
                    this.$notify_message("Reservation rescheduled");
                    this.rescheduling_modal = false;
                    this.request_reload();
                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error)
                });
        }

        load_reservation()
        {
            this.loading = true;
            get_reservation(this.context)
                .then((response) => {
                    this.reservation = response;
                })
                .catch(() => {
                    this.$notify_error("Unable to fetch reservation data");
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        request_reload() {
            this.$emit("updated:reservations");
        }

        close() {
            this.$emit("update:active", false);
        }
    }

</script>
