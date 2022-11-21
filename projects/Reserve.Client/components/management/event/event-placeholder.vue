<template>
    <div
        :style="styling"
        class="v-calendar-event-placeholder"
        @mousedown.stop
        @touchstart.stop>
        <v-card
            color="#91BFA6"
            height="100%"
            class="pa-0 white--text mx-auto user-select-disabled">
            <v-menu
                fixed
                v-model="controls"
                :nudge-right="120"
                :close-on-content-click="false"
                :close-on-click="false"
                offset-overflow
                z-index="3">
                <template v-slot:activator="{ on }">
                    <div v-on="on"></div>
                </template>
                <v-list
                    class="pa-0">
                    <v-list-item
                        @click="invoke_creator">
                        <v-list-item-icon>
                            <v-icon>mdi-calendar-plus</v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                Create reservation
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                    <v-list-item
                        @click="close">
                        <v-list-item-icon>
                            <v-icon>mdi-close</v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                Dismiss
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                </v-list>
            </v-menu>
            <v-card-text>
                {{ start }} - {{ ending }}
            </v-card-text>
        </v-card>
    </div>
</template>
<script lang="ts">

    import { Component, mixins, Property } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";

    @Component
    export default class EventPlaceholder extends mixins(DateFunctions)
    {
        @Property({
            type: Number,
            required: true
        })
        position!: number;

        @Property({
            type: Number,
            required: true
        })
        height!: number;

        @Property({
            type: String,
            required: true
        })
        date!: string;

        @Property({
            type: String,
            required: true
        })
        start!: string;

        @Property({
            type: Number,
            required: true
        })
        timespan!: number;

        @Property({
            type: Number,
            required: false,
            default: 3
        })
        offset!: number;

        controls: boolean = true;

        invoke_creator()
        {
            const formatted = `${this.date} ${this.start}`;
            this.$emit("create:reservation", formatted);
            this.close();
        }

        close() {
            this.$emit("clear:event-placeholder");
        }

        get ending()
        {
            const start = this.$dateutil.parse(this.start, "HH:mm", new Date());
            const added = this.$dateutil.addMinutes(start, this.timespan);

            return this.$format_date(added, "HH:mm");
        }

        get styling()
        {
            return {
                top: `${this.position}px`,
                height: `${this.height - this.offset}px`
            }
        }
    }


</script>

<style lang="scss">

    .v-calendar-event-placeholder
    {
        position: absolute;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        left: 4px;
        right: 4px;

        z-index: 3;
        -webkit-transition: height 0.1s;
        -moz-transition: height 0.1s;
        -ms-transition: height 0.1s;
        -o-transition: height 0.1s;
        transition: height 0.1s;
    }

    .user-select-disabled
    {
        -webkit-user-drag: none;
        -khtml-user-drag: none;
        -moz-user-drag: none;
        -o-user-drag: none;
        user-drag: none;

        -webkit-touch-callout: none; /* iOS Safari */
        -webkit-user-select: none; /* Safari */
        -khtml-user-select: none; /* Konqueror HTML */
        -moz-user-select: none; /* Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
        user-select: none; /* Non-prefixed version, currently
                                        supported by Chrome and Opera */
    }

</style>
