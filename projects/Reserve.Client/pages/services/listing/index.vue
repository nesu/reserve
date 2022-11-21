<template>
    <v-container>
        <v-row>
            <v-col
                cols="12">
                <v-card
                    outlined
                    :loading="loading">
                    <v-toolbar
                        flat tile>
                        <div
                            class="d-flex flex-row-reverse">
                            <v-text-field
                                v-model="search_literal"
                                append-icon="mdi-magnify"
                                label="Search service"
                                dense outlined
                                clearable hide-details/>
                        </div>
                        <v-menu
                            offset-x
                            :nudge-right="5"
                            :nudge-width="340"
                            :max-width="340"
                            :close-on-content-click="false">
                            <template v-slot:activator="{ on }">
                                <v-btn
                                    class="mx-3"
                                    v-on="on" icon>
                                    <v-icon>mdi-filter</v-icon>
                                </v-btn>
                            </template>
                            <v-card tile flat elevation="0">
                                <v-card-title>
                                    Categories
                                </v-card-title>
                                <v-divider class="mx-4"/>
                                <v-card-text>
                                    <v-treeview
                                        selectable
                                        v-model="category_filters"
                                        :items="categories"
                                        selection-type="independent"
                                        item-text="label"
                                        item-children="child_categories"
                                        expand-icon="mdi-chevron-down"
                                        open-all
                                        multiple-active
                                    ></v-treeview>
                                </v-card-text>
                                <v-divider class="mx-4"/>
                                <v-card-actions>
                                    <v-btn
                                        outlined color="primary"
                                        @click="clear_filter">
                                        Clear filters
                                    </v-btn>
                                    <v-spacer/>
                                    <v-btn
                                        text color="primary"
                                        @click="apply_filter">
                                        Apply
                                    </v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-menu>
                    </v-toolbar>
                </v-card>
                <div
                    class="my-6">
                    <v-row
                        v-if="services_filtered.length < 1 && !loading">
                        <v-col
                            cols="12"
                            class="d-flex justify-center">
                            There are no services for specified specialist or category filter.
                        </v-col>
                    </v-row>
                    <v-card
                        v-else
                        v-for="it in services_filtered" :key="it.id"
                        class="my-2"
                        outlined>
                        <v-card-title>
                            {{ it.label }}
                            <v-spacer/>
                            <v-btn
                                class="mr-3"
                                icon color="primary"
                                @click="remove_service(it.id)"
                                v-if="selected_services.includes(it.id)">
                                <v-icon>mdi-check-box-outline</v-icon>
                            </v-btn>
                            <v-btn
                                class="mr-3"
                                icon color="primary"
                                @click="append_service((it.id))"
                                v-else>
                                <v-icon>mdi-checkbox-blank-outline</v-icon>
                            </v-btn>
                        </v-card-title>
                        <v-divider class="mx-4"/>
                        <v-card-text
                            class="d-flex flex-row">
                            <v-chip
                                text-color="white"
                                color="primary">
                                <v-icon class="mr-2">mdi-clock</v-icon>
                                {{ it.duration }}min.
                            </v-chip>
                            <v-chip
                                class="ml-3"
                                text-color="white"
                                color="primary">
                                <v-icon class="mr-2">mdi-currency-eur</v-icon>
                                {{ it.price }}
                            </v-chip>
                            <v-chip
                                class="ml-3"
                                text-color="white"
                                color="primary">
                                <v-icon class="mr-2">mdi-label</v-icon>
                                {{ it.category.label }}
                            </v-chip>
                            <v-spacer/>
<!--                            <v-dialog-->
<!--                                width="600">-->
<!--                                <template v-slot:activator="{ on }">-->
<!--                                    <v-btn-->
<!--                                        v-on="on"-->
<!--                                        outlined color="primary">-->
<!--                                        Gallery-->
<!--                                    </v-btn>-->
<!--                                </template>-->
<!--                                <v-card-->
<!--                                    flat tile>-->
<!--                                    <v-card-text>-->
<!--                                        ABC?-->
<!--                                    </v-card-text>-->
<!--                                </v-card>-->
<!--                            </v-dialog>-->
                            <v-dialog
                                v-if="it.description !== null && it.description.length > 0"
                                width="400">
                                <template v-slot:activator="{ on }">
                                    <v-btn
                                        v-on="on"
                                        class="ml-2"
                                        text color="primary">
                                        Read more
                                    </v-btn>
                                </template>
                                <v-card
                                    flat tile>
                                    <v-card-text>
                                        {{ it.description }}
                                    </v-card-text>
                                </v-card>
                            </v-dialog>
                        </v-card-text>
                    </v-card>
                </div>
            </v-col>
        </v-row>
        <v-bottom-sheet
            :value="selected_services.length > 0"
            no-click-animation persistent
            inset hide-overlay>
            <v-card
                outlined
                height="80px"
                class="text-center">
                <v-row
                    class="fill-height" justify="center" align="center">
                    <v-col cols="6">
                        <span class="headline">
                            Total: {{ services_price_total }}&euro;
                        </span>
                    </v-col>
                    <v-col cols="6">
                        <v-btn
                            @click="detalizing = true"
                            color="primary">
                            Continue
                        </v-btn>
                    </v-col>
                </v-row>
            </v-card>
        </v-bottom-sheet>
        <v-dialog
            v-model="detalizing"
            fullscreen persistent>
            <v-sheet
                height="100%">
                <v-container
                    class="narrowed fill-height">
                    <v-card
                        class="d-flex flex-column"
                        height="100%" width="100%"
                        flat tile>
                        <div
                            class="flex-shrink-1">
                            <v-toolbar
                                flat
                                fixed
                                height="76"
                                class="pl-4"
                                color="transparent"
                                width="100%">
                                <v-toolbar-title>
                                    Reservation overview
                                </v-toolbar-title>
                                <v-spacer/>
                                <v-btn
                                    icon
                                    class="mr-2"
                                    @click="detalizing = false">
                                    <v-icon>mdi-close</v-icon>
                                </v-btn>
                            </v-toolbar>
                        </div>
                        <v-divider class="mx-4"/>
                        <div
                            class="pt-6 flex-grow-1">
                            <v-card-text>
                                <v-alert type="info">
                                    You can update your profile information in <nuxt-link :to="$routes.Settings">settings</nuxt-link>
                                </v-alert>
                                <v-text-field
                                    outlined
                                    :value="$account.given_name"
                                    readonly label="Name"/>
                                <v-text-field
                                    outlined
                                    :value="$account.family_name"
                                    readonly label="Surname"/>
                                <v-text-field
                                    outlined
                                    :value="$account.email"
                                    readonly label="Email address"/>
                                <v-text-field
                                    outlined
                                    :value="$account.phone_number"
                                    readonly label="Phone number"/>
                            </v-card-text>
                            <v-card-text>
                                <datetime-select
                                    :datetime.sync="reservation_request.datetime"
                                    :timespan="services_duration_total"
                                    :employee_id="employee"/>
                            </v-card-text>
                        </div>
                        <v-divider class="mx-4"/>
                        <v-card-actions>
                            <v-spacer/>
                            <v-btn
                                color="primary"
                                @click="create_reservation">
                                Submit
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-container>
            </v-sheet>
        </v-dialog>
    </v-container>
</template>
<script lang="ts">

    import Fuse from "fuse.js";
    import { sumBy } from "lodash-es";
    import PageBase from "~/commons/mixins/page-base";
    import { Component, mixins, Watch } from "~/commons";
    import { create_reservation } from "~/commons/requests/accounts/reservations";
    import { get_parent_category_collection } from "~/commons/requests/general/categories";
    import { get_service_collection_by_employee } from "~/commons/requests/general/employees";
    import { ReservationRequest } from "~/commons/requests/models/reservation-request";
    import { Service } from "~/commons/resources/service";
    import { ServiceCategory } from "~/commons/resources/service-category";

    @Component({
        head: {
            title: "Service listing"
        },
        components: {
            "datetime-select": require("~/components/datetime-select").default
        }
    })
    export default class ServicesPage extends mixins(PageBase)
    {
        loading: boolean = false;
        employee: number | null = null;
        search_literal: string = "";

        services: Service[] = [];
        services_filtered: Service[] = [];
        selected_services: number[] = [];

        categories: ServiceCategory[] = [];
        category_filters: number[] = [];

        detalizing: boolean = false;
        submittable: boolean = false;

        reservation_request: ReservationRequest = {
            datetime: "",
            services: [],
            employee_id: 0
        };

        created()
        {
            const query = this.$route.query;
            if (!this.isset_string(query.specialist))
            {
                this.$router.push({
                    path: "/services"
                });
            }
            else {
                this.employee = parseInt(query.specialist as any);
                this.load_services();
            }
        }

        create_reservation()
        {
            if (!this.employee || this.selected_services.length < 1)
                return;

            this.reservation_request.employee_id = this.employee;
            this.reservation_request.services = this.selected_services;

            this.loading = true;

            create_reservation(this.reservation_request)
                .then((response) => {
                    this.$notify_message("Reservation created successfully");
                    this.$router.push({
                        path: "/reservations"
                    });
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                })
        }

        get services_price_total()
        {
            const services = this.services.filter((it) => {
                return this.selected_services.includes(it.id);
            });

            return sumBy(services, (it) => {
                return it.price;
            });
        }

        get services_duration_total()
        {
            const services = this.services.filter((it) => {
                return this.selected_services.includes(it.id);
            });

            return sumBy(services, (it) => {
                return it.duration;
            });
        }

        isset_string(value: any) {
            return !!value && typeof value === "string" && value.length > 0;
        }

        load_services()
        {
            if (this.employee == null || isNaN(this.employee)) {
                return;
            }

            this.loading = true;
            get_service_collection_by_employee(this.employee)
                .then((response) => {
                    this.services = response;
                    this.services_filtered = response;
                    return get_parent_category_collection()
                        .then((categories) => {
                            this.categories = categories;
                        });
                })
                .catch((error) => {
                    this.$notify_error(error);
                    this.redirect_out();
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        remove_service(sid: number)
        {
            this.selected_services = this.selected_services.filter((it) => {
                return it != sid;
            });
        }

        append_service(sid: number) {
            this.selected_services.push(sid);
        }

        apply_filter()
        {
            this.services_filtered = this.services.filter((it) => {
                return this.category_filters.includes(it.category_id);
            });
        }

        clear_filter() {
            this.services_filtered = this.services;
            this.category_filters = [];
        }

        redirect_out()
        {
            this.$router.push({
                path: "/services"
            });
        }

        @Watch("search_literal")
        search_category(value: string | null)
        {
            if (!value || value === "")
            {
                this.services_filtered = this.services;
                return;
            }

            if (value.length < 3) {
                return;
            }

            const fuse = new Fuse(this.services, {
                findAllMatches: true,
                matchAllTokens: true,
                keys: ["label", "description"]
            });

            this.services_filtered = fuse
                .search(value)
                .map(it => it.item);
        }
    }

</script>
<style lang="scss">

    .v-alert__content
    {
        a {
            color: white !important;
            text-decoration: underline !important;
        }
    }

</style>
