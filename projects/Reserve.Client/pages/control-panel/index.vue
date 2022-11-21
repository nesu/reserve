<template>
    <contents
        @mouseup="creator_end"
        @mouseleave="creator_end">
        <default-toolbar>
            <template v-slot:toolbar-title>
                Calendar
            </template>
        </default-toolbar>
        <v-container
            class="management-layout">
            <v-card
                flat tile>
                <v-toolbar
                    flat tile
                    height="78"
                    v-if="toolbar === 0"
                    color="white">
                    <v-btn
                        outlined
                        color="black"
                        class="mr-3"
                        @click="viewing_date = today">
                        Today
                    </v-btn>
                    <v-btn
                        @click="move('previous')"
                        icon>
                        <v-icon dark>
                            mdi-chevron-left
                        </v-icon>
                    </v-btn>
                    <v-btn
                        @click="move('next')"
                        icon>
                        <v-icon dark>
                            mdi-chevron-right
                        </v-icon>
                    </v-btn>
                    <div class="headline hidden-md-and-down">{{ $format_date(viewing_date) }}</div>
                </v-toolbar>
                <v-card-text>
                    <v-sheet>
                        <v-calendar
                            ref="calendar"
                            :now="today"
                            :value="viewing_date"
                            :type="calendar_type"
                            :events="events"
                            v-bind="calendar_schema"
                            @click:event="show_event_view"
                            @mousedown:time="creator_start"
                            @mousemove:time="creator_move"
                            @touchstart:time="creator_start"
                            @touchmove:time="creator_move"
                            @touchmove:time.native.stop="">
                            <template v-slot:event="{ event }">
                                <event-component
                                    :event="event"/>
                            </template>
                            <template v-slot:day-body="{ date, timeToY, minutesToPixels }">
                                <event-placeholder
                                    v-if="creator.start_date === date && creator_valid"
                                    @clear:event-placeholder="creator_reset"
                                    @create:reservation="init_reservation_creator"
                                    :position="timeToY(creator.start_time)"
                                    :height="minutesToPixels(creator.timespan)"
                                    :date="creator.start_date"
                                    :start="creator.start_time"
                                    :timespan="creator.timespan"
                                ></event-placeholder>
                            </template>
                        </v-calendar>
                        <contents
                            v-if="event_view_valid">
                            <reservation-overview
                                :active.sync="event_view.active"
                                :context="event_view.event.reservation_id"
                                @updated:reservations="load_events"/>
                        </contents>
                        <contents>
                            <reservation-creator
                                v-if="creating_reservation"
                                :datetime="creating_reservation_at"
                                :active.sync="creating_reservation"
                                @updated:reservations="load_events"/>
                        </contents>
                    </v-sheet>
                </v-card-text>
            </v-card>
        </v-container>
    </contents>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import ManagementBase from "~/commons/mixins/management-base";
    import { get_events } from "~/commons/requests/management/services";
    import { GetEventRequest } from "~/commons/requests/models/get-event-request";
    import { CalendarEvent } from "~/commons/resources/CalendarEvent";
    import { RoleType } from "~/commons/resources/role-type";

    @Component({
        head: {
            title: "Control Panel - Calendar"
        },
        components: {
            "event-component": require("~/components/management/event/event-component").default,
            "event-placeholder": require("~/components/management/event/event-placeholder").default,
            "reservation-overview": require("~/components/reservation-overview").default,
            "reservation-creator": require("~/components/management/reservation-creator").default
        }
    })
    export default class CalendarPage extends mixins(ManagementBase, DateFunctions)
    {
        toolbar: number = 0;
        events: CalendarEvent[] = [];

        viewing_date: string | null = null;
        today: string | null = null;

        event_view: any = {
            active: false,
            event: null
        }

        creator: any = {
            active: false,
            start_time: null,
            start_date: null,
            timespan: 0
        }

        creating_reservation: boolean = false;
        creating_reservation_at: string | null = null;

        created() {
            this.today = this.$format_date(new Date(), "yyyy-MM-dd");
            this.viewing_date = this.$format_date(new Date(), "yyyy-MM-dd");
            this.load_events();
        }

        init_reservation_creator(datetime: string | null) {
            this.creating_reservation = true;
            this.creating_reservation_at = datetime;
        }

        creator_start(event: any)
        {
            if (this.creator_valid) {
                this.creator_reset();
            }

            this.creator.active = true;
            this.creator.timespan = 15;
            this.creator.start_date = event.date;

            const datetime = this.$parse_date(`${event.date} ${event.time}`);
            const rounded = this.$dateutil.roundToNearestMinutes(datetime, {
                nearestTo: 15
            });

            this.creator.start_time = this.$format_date(rounded, "HH:mm");
        }

        creator_move(event: any)
        {
            if (!this.creator.active) {
                return;
            }

            const datetime = this.$dateutil.roundToNearestMinutes(
                    this.$parse_date(`${this.creator.start_date} ${event.time}`), {
                    nearestTo: 15
                });

            const start_datetime = this.$parse_date(
                `${this.creator.start_date} ${this.creator.start_time}`
            );

            const start_datetime_added = this.$dateutil.addMinutes(start_datetime, this.creator.timespan);

            const diff = this.$dateutil
                .differenceInMinutes(datetime, start_datetime_added);

            this.creator.timespan += diff;
        }

        creator_end()
        {
            if (this.creator.active) {
                this.creator.active = false;
            }
        }

        creator_reset()
        {
            this.creator.active = false;
            this.creator.start_date = null;
            this.creator.end_date = null;
            this.creator.timespan = 0;
        }

        load_events()
        {
            const payload: GetEventRequest = {};
            if (this.$account.role === RoleType.Employee) {
                payload.employee_id = this.$account.id;
            }

            get_events(payload)
                .then((response) => {
                    this.events = response;
                });
        }

        move(side: "next" | "previous")
        {
            if (!this.viewing_date) {
                return;
            }

            if (this.calendar_type == "week")
            {
                switch (side)
                {
                    case "next":
                        const increment = this.$dateutil.addWeeks(this.$parse_date(this.viewing_date), 1);
                        this.viewing_date = this.$format_date(increment, "yyyy-MM-dd");
                        break;
                    case "previous":
                        const decrement = this.$dateutil.subWeeks(this.$parse_date(this.viewing_date), 1);
                        this.viewing_date = this.$format_date(decrement, "yyyy-MM-dd");
                        break;
                }
            }
            else
            {
                switch (side)
                {
                    case "next":
                        const increment = this.$dateutil.addDays(this.$parse_date(this.viewing_date), 1);
                        this.viewing_date = this.$format_date(increment, "yyyy-MM-dd");
                        break;
                    case "previous":
                        const decrement = this.$dateutil.subDays(this.$parse_date(this.viewing_date), 1);
                        this.viewing_date = this.$format_date(decrement, "yyyy-MM-dd");
                        break;
                }
            }
        }

        show_event_view(e: any)
        {
            const event: CalendarEvent = e.event;

            if (!event) {
                return;
            }

            this.event_view.event = event;
            this.event_view.active = true;
        }

        hide_event_view()
        {
            this.event_view = {
                active: false,
                event: null,
                element: null
            }
        }

        event_color_of(event: CalendarEvent)
        {
            const start = this.$parse_date(event.start);
            if (start < new Date()) {
                return "#91BFA6";
            }

            return "#37966F";
        }

        get creator_valid() {
            return this.creator.start_time != null && this.creator.timespan > 0;
        }

        get event_view_valid() {
            return this.event_view.active && !!this.event_view.event;
        }

        get calendar_type() {
            return this.$vuetify.breakpoint.smAndUp ? "week" : "day";
        }

        get calendar_schema()
        {
            return {
                ref: "calendar",
                "short-intervals": false,
                "short-weekdays": false,
                weekdays: [1, 2, 3, 4, 5, 6, 0],
                "first-interval": 32,
                "interval-count": 50,
                "interval-minutes": 15,
                "interval-height": 31,
                "show-interval-label": (interval: any) => interval.minute === 0,
                "interval-format": (interval: any) => interval.time,
                "event-color": this.event_color_of
            }
        }
    }

</script>
<style lang="scss">

    .v-event-timed {
        border-radius: 6px !important;
    }

</style>
