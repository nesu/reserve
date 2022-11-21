<template>
    <contents>
        <v-container
            v-if="reservations.length === 0"
            class="fill-height">
            <v-row>
                <v-col
                    class="d-flex justify-center"
                    cols="12">
                    <div
                        class="no-reservations-visual"/>
                </v-col>
                    <v-col
                        class="d-flex justify-center"
                        cols="12">
                    <span
                        class="headline font-weight-light text-center">
                        You currently have no reservations
                    </span>
                    </v-col>
                    <v-col
                        class="d-flex justify-center"
                        cols="12">
                        <v-btn
                            :to="$routes.Services"
                            outlined large
                            color="primary">
                            View services
                        </v-btn>
                    </v-col>
            </v-row>
        </v-container>
        <v-container
            v-else>
            <v-row>
                <v-col
                    cols="12">
<!--                    <v-card-->
<!--                        outlined>-->
<!--                        <v-toolbar-->
<!--                            flat tile-->
<!--                            height="56">-->
<!--                            <v-toolbar-title class="headline">-->
<!--                                Reservations-->
<!--                            </v-toolbar-title>-->
<!--                        </v-toolbar>-->
<!--                    </v-card>-->
                    <div
                        v-for="it in reservations" :key="it.id">
                        <v-card
                            outlined
                            class="my-3">
                            <v-list
                                two-line>
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-list-item-title>
                                            {{ it.assignee.given_name }} {{ it.assignee.family_name }}
                                        </v-list-item-title>
                                        <v-list-item-subtitle>
                                            {{ $format_date(it.event.start_at, "yyyy-MM-dd HH:mm") }}
                                        </v-list-item-subtitle>
                                    </v-list-item-content>
                                    <v-list-item-content>
                                        <v-list-item-title>
                                            {{ formatted_timespan(it) }}
                                        </v-list-item-title>
                                        <v-list-item-subtitle>
                                            {{ price_of(it) }}&euro;
                                        </v-list-item-subtitle>
                                    </v-list-item-content>
                                    <v-list-item-action>
                                        <v-btn
                                            icon @click.stop="activate_overview(it.id)">
                                            <v-icon
                                                color="primary">
                                                mdi-chevron-right
                                            </v-icon>
                                        </v-btn>
                                    </v-list-item-action>
                                </v-list-item>
                            </v-list>
                        </v-card>
                    </div>
                </v-col>
            </v-row>
            <reservation-overview
                v-if="reservation_overview.active"
                :active.sync="reservation_overview.active"
                :context="reservation_overview.context"
                @updated:reservations="load_reservations"/>
        </v-container>
    </contents>
</template>
<script lang="ts">

    import { sumBy } from "lodash-es";
    import DateFunctions from "~/commons/mixins/date-functions";
    import {
        cancel_reservation,
        get_reservations,
        reschedule_reservation
    } from "~/commons/requests/accounts/reservations";
    import { CancelReservationRequest } from "~/commons/requests/models/cancel-reservation-request";
    import { ResecheduleReservationRequest } from "~/commons/requests/models/resechedule-reservation-request";
    import { Reservation } from "~/commons/resources/reservation";
    import { Component, mixins } from "../commons";
    import PageBase from "../commons/mixins/page-base";

    @Component({
        head: {
            title: "My reservations"
        },
        components: {
            "datetime-select": require("~/components/datetime-select").default,
            "confirmable-modal": require("~/components/confirmable-modal").default,
            "reservation-overview": require("~/components/reservation-overview").default,
        },
        async asyncData() {
            const reservations = await get_reservations();
            return {
                reservations
            }
        }
    })
    export default class ReservationPage extends mixins(PageBase, DateFunctions)
    {
        reservations: Reservation[] = [];

        reschedule_reservation_request: ResecheduleReservationRequest = {
            datetime: "",
            reservation_id: 0
        }

        reservation_overview: any = {
            active: false,
            context: 0
        };

        rescheduling_modal: number = 0;
        overview_modal: number = 0;

        load_reservations()
        {
            get_reservations()
                .then((response) => {
                    this.reservations = response;
                });
        }

        price_of(reservation: Reservation)
        {
            return sumBy(reservation.services, (it) => {
                return it.service.price
            });
        }

        formatted_timespan(reservation: Reservation)
        {
            const start = this.$parse_date(reservation.event.start_at);
            const end = this.$parse_date(reservation.event.end_at);

            return this.$dateutil.formatDistance(end, start, {
                includeSeconds: false
            })
        }

        activate_overview(rid: number)
        {
            this.reservation_overview = {
                active: true,
                context: rid
            }
        }
    }

</script>
<style lang="scss">
    @import "./assets/style/pages/reservations";
</style>
