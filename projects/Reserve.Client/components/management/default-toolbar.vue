<template>
    <contents>
        <v-toolbar
            flat fixed
            width="100%"
            class="px-2"
            color="transparent">
            <v-btn
                icon
                class="hidden-md-and-up"
                @click.stop="navigator">
                <v-icon>mdi-menu</v-icon>
            </v-btn>
            <v-toolbar-title>
                <slot name="toolbar-title"></slot>
            </v-toolbar-title>
            <slot name="toolbar-body"></slot>
        </v-toolbar>
        <v-divider class="mx-4"/>
    </contents>
</template>
<script lang="ts">

    import { EventType } from "~/commons/lib/event-type";
    import { Component, Vue } from "../../commons";

    @Component
    export default class DefaultToolbar extends Vue
    {
        navigator() {
            this.$events.trigger(EventType.TRIGGER_MANAGEMENT_NAVIGATOR)
        }
    }

</script>
