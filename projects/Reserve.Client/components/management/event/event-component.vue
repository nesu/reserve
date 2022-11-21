<template>
    <v-sheet
        color="transparent"
        class="white--text px-2"
        width="100%" height="100%"
        @mousedown.stop
        @touchstart.stop>
        <v-row
            class="fill-height">
            <v-col>
                <v-row no-gutters>
                    <v-col>
                        {{ event.account.given_name }} {{ event.account.family_name }} ({{ $format_date(event.start, "HH:mm") }})
                    </v-col>
                    <v-col>
                        {{ event.account.email || 'Email not set'  }}
                    </v-col>
                    <v-col>
                        {{ event.account.phone_number || 'Phone number not set'  }}
                    </v-col>
                </v-row>
            </v-col>
            <v-col>
                {{ event.employee.given_name }} {{ event.employee.family_name }}
            </v-col>
        </v-row>
    </v-sheet>
</template>
<script lang="ts">

    import { Component, mixins, Property } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import { CalendarEvent } from "~/commons/resources/CalendarEvent";
    import ComponentBase from "~/commons/mixins/component-base";

    @Component
    export default class EventComponent extends mixins(ComponentBase, DateFunctions)
    {
        @Property({
            type: Object,
            required: true
        })
        event!: CalendarEvent;
    }

</script>
