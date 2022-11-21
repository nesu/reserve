<template>
    <v-dialog
        width="400"
        v-model="visible"
        persistent>
        <template v-slot:activator="{ on }">
            <slot name="activator" :on="on"></slot>
        </template>
        <slot name="body">
            <v-card tile>
                <v-card-title
                    class="pa-4">
                    <slot name="modal-title"/>
                </v-card-title>
                <v-divider class="mx-4"/>
                <v-card-text>
                    <slot name="modal-body"/>
                </v-card-text>
                <v-divider class="mx-4"/>
                <v-card-actions class="pa-4">
                    <v-btn
                        text tile
                        color="primary"
                        @click.stop="cancel">
                        <slot name="modal-cancellation-text">
                            Cancel
                        </slot>
                    </v-btn>
                    <v-spacer/>
                    <v-btn
                        outlined
                        color="red"
                        @click.stop="confirmed">
                        <slot name="modal-confirmation-text">
                            Yes
                        </slot>
                    </v-btn>
                </v-card-actions>
            </v-card>
        </slot>
    </v-dialog>
</template>
<script lang="ts">

    import { Component, mixins } from "../commons";
    import ComponentBase from "../commons/mixins/component-base";

    @Component
    export default class ConfirmableModalComponent extends mixins(ComponentBase)
    {
        visible: boolean = false;

        confirmed() {
            this.$emit("confirmed");
            this.cancel();
        }

        cancel() {
            this.visible = false;
        }
    }

</script>
