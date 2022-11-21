<template>
    <v-slide-y-transition>
        <v-snackbar
            id="notification-component"
            :value="active"
            @input="dismiss"
            multi-line
            top
            right
        >
            {{ this.model.contents }}
            <v-btn
                icon
                color="white"
                @click.stop="dismiss">
                <v-icon>
                    mdi-close
                </v-icon>
            </v-btn>
        </v-snackbar>
    </v-slide-y-transition>
</template>
<script lang="ts">

    import { Component, Vue } from "../commons";
    import Notification from "~/commons/lib/notification";

    @Component
    export default class NotificationComponent extends Vue
    {
        get model(): Notification {
            return this.$modules.notifications.model;
        }

        get active(): boolean {
            return this.model.active;
        }

        dismiss() {
            this.$modules.notifications.reset();
        }
    }

</script>
<style lang="scss">

    #notification-component .v-snack__wrapper {
        border-radius: 0 !important;
    }

</style>
