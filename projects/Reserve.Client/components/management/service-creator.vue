<template>
    <v-dialog
        v-model="active"
        width="600px"
        persistent>
        <v-sheet
            height="100%">
            <v-container
                class="fill-height">
                <v-card
                    flat tile
                    width="100%" height="100%"
                    class="d-flex flex-column">
                    <div class="flex-shrink-1">
                        <v-toolbar
                            flat
                            fixed
                            height="76"
                            color="transparent"
                            width="100%"
                        >
                            <v-toolbar-title>
                                Create new service
                            </v-toolbar-title>
                            <v-spacer/>
                            <v-btn
                                class="mr-2" icon
                                @click="close">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-toolbar>
                    </div>
                    <v-divider class="mx-4"/>
                    <v-card-text
                        class="flex-grow-1">
                        <v-form
                            v-model="submittable">
                            <v-text-field
                                outlined
                                v-model="service_payload.label"
                                :rules="validators.required"
                                label="Service name"/>
                            <v-text-field
                                outlined
                                v-model="service_payload.price"
                                :rules="validators.number"
                                label="Price (€)"/>
                            <v-text-field
                                outlined
                                v-model="service_payload.duration"
                                :rules="validators.number"
                                label="Duration (minutes)"/>
                            <v-autocomplete
                                outlined
                                v-model="service_payload.category_id"
                                :items="nested_categories"
                                :rules="validators.required"
                                item-text="label"
                                item-value="id"
                                label="Category"
                            ></v-autocomplete>
                            <v-textarea
                                outlined
                                v-model="service_payload.description"
                                label="Description"/>
                        </v-form>
                    </v-card-text>
                    <v-divider class="mx-4"/>
                    <v-card-actions>
                        <v-spacer/>
                        <v-btn
                            @click="create_service"
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

    import { create_category, get_categories } from "~/commons/requests/management/categories";
    import { create_service } from "~/commons/requests/management/services";
    import { CreateServiceRequest } from "~/commons/requests/models/create-service-request";
    import { ServiceCategory } from "~/commons/resources/service-category";
    import { Component, mixins, Property } from "../../commons";
    import ComponentBase from "../../commons/mixins/component-base";
    import InputValidators from "../../commons/mixins/input-validators";

    @Component
    export default class ServiceCreatorDialog extends mixins(ComponentBase, InputValidators)
    {
        @Property({
            required: true,
            type: Boolean
        })
        active!: boolean;
        submittable: boolean = false;

        nested_categories: ServiceCategory[] = [];
        service_payload: CreateServiceRequest = {
            label: "",
            description: null,
            price: 0,
            duration: 0,
            category_id: 0
        };

        created()
        {
            get_categories()
                .then((response) => {
                    this.nested_categories = response;
                });
        }

        create_service()
        {
            if (!this.submittable) {
                return;
            }

            const payload = this.service_payload;

            if (payload.description != null && payload.description.length === 0) {
                payload.description = null;
            }

            create_service(payload)
                .then((response) => {
                    this.$notify_message("New service was created");
                    this.$emit("updated:services", false);
                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error);
                });
        }

        close()
        {
            this.$emit("update:active", false);
            this.service_payload = {
                label: "",
                description: null,
                price: 0,
                duration: 0,
                category_id: 0
            };
        }
    }

</script>
