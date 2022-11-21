<template>
    <contents>
        <v-dialog
            :return-value.sync="date"
            ref="calendar"
            v-model="active"
            width="290px"
            persistent
            lazy>
            <template v-slot:activator="{ on }">
                <v-text-field
                    :disabled="timespan === 0"
                    outlined v-on="on"
                    v-model="date"
                    :loading="loading"
                    color="primary"
                    label="Reservation date"
                    readonly/>
            </template>
            <v-card
                v-if="loading"
                flat tile>
                <v-card-text
                    class="text-center">
                    <v-progress-circular indeterminate/>
                </v-card-text>
            </v-card>
            <v-date-picker
                v-else
                color="primary"
                :min="range.start"
                :max="range.end"
                :picker-date.sync="viewing_month"
                v-model="date"
                :allowed-dates="is_allowed"
                locale="lt"
                scrollable
                reactive>
                <v-spacer></v-spacer>
                <v-btn
                    text color="primary"
                    @click="$refs.calendar.save(date)">
                    Confirm
                </v-btn>
            </v-date-picker>
        </v-dialog>
        <v-select
            outlined
            :disabled="date === null || timespan === 0"
            :items="time_array"
            v-model="datetime"
            color="primary"
            label="Reservation time"
            no-data-text="Selected date has no available times.">
        </v-select>
    </contents>
</template>
<script lang="ts">

    import DateFunctions from "~/commons/mixins/date-functions";
    import { Component, mixins, Property, Watch } from "../commons";
    import ComponentBase from "../commons/mixins/component-base";

    @Component
    export default class DateTimeSelect extends mixins(ComponentBase, DateFunctions)
    {
        @Property({
            required: true,
            type: Number
        })
        timespan!: Number;

        @Property({
            required: true,
            type: Number
        })
        employee_id!: Number;

        @Property({
            type: String,
            required: false,
            default: null
        })
        vdate!: string | null;

        date: string = "";
        active: boolean = false;
        loading: boolean = false;

        disabled: string[] = [];
        viewing_month: string | null = null;
        availability: Record<string, string[]> = {};

        datetime: string | null = null;

        range: Record<string, string> = {};

        created()
        {
            const date = this.vdate ?? new Date();
            const added = this.$dateutil.addMonths(new Date(), 3);

            const start = this.$format_date(date, "yyyy-MM-dd");
            const end = this.$format_date(added, "yyyy-MM-dd");

            this.range = {
                start,
                end
            };
        }

        is_allowed(date: string) {
            return !!this.availability[date]
        }

        load(date: string = this.date, timespan = this.timespan)
        {
            this.loading = true;
            const payload = {
                date,
                duration: timespan,
                employee_id: this.employee_id
            };

            this.$axios
                .post("/api/employees/availability", payload)
                .then((response) =>
                {
                    this.disabled = response.data.disabled;
                    this.availability = response.data.data;
                    this.date = this.$format_date(response.data.nearest, "yyyy-MM-dd");

                    this.viewing_month = this.$format_date(date, "yyyy-MM");
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        get time_array() {
            return this.availability[this.date] ?? [];
        }

        @Watch("date")
        date_formatter(current: string, previous: string)
        {
            // if (!!current && !!this.viewing_month)
            // {
            //     const v1 = this.$dateutil
            //         .addMonths(this.$parse_date(this.viewing_month), 1);
            //
            //     const v2 = this.$dateutil.startOfMonth(v1);
            //
            //     this.viewing_month = this.$format_date(v2, "yyyy-MM");
            //     this.date = this.$format_date(v2, "yyyy-MM-dd");
            // }
        }

        @Watch("viewing_month")
        month_formatter(current: string, previous: string) {
            this.load(`${current}-01`, this.timespan);
        }

        @Watch("datetime")
        datetime_formatter(current: string, previous: string) {
            this.$emit("update:datetime", current);
        }
    }

</script>
