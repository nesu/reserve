<template>
    <v-container
        class="fill-height">
        <v-card
            height="100%"
            width="100%"
            outlined>
            <v-toolbar
                flat tile
                height="56">
                <v-toolbar-title class="headline">
                    Services
                </v-toolbar-title>
                <v-spacer/>
            </v-toolbar>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-row class="d-flex flex-row flex-nowrap">
                    <v-col cols="3">
                        <v-subheader class="pl-0">
                            Available locations
                        </v-subheader>
                        <v-radio-group v-model="location">
                            <v-radio
                                v-for="l in locations"
                                :key="l.address"
                                :label="l.address"
                                :value="l.address"
                            ></v-radio>
                        </v-radio-group>
                        <v-subheader class="pl-0">
                            Available specialists
                        </v-subheader>
                        <v-radio-group v-model="specialist">
                            <v-radio
                                v-for="l in specialists"
                                :key="l.label"
                                :label="l.label"
                                :value="l.label"
                            ></v-radio>
                        </v-radio-group>
                        <v-subheader class="pl-0">
                            Show categories
                        </v-subheader>
                        <v-checkbox
                            dense value="1"
                            label="Category 1"
                            hide-details
                        ></v-checkbox>
                        <v-checkbox
                            dense value="1"
                            label="Category 2"
                            hide-details
                        ></v-checkbox>
                        <v-checkbox
                            dense value="1"
                            label="Category 3"
                            hide-details
                        ></v-checkbox>
                        <v-checkbox
                            dense value="1"
                            label="Category 4"
                            hide-details
                        ></v-checkbox>
                        <div class="my-4">
                            <v-divider/>
                            <v-row>
                                <v-col class="d-flex justify-end">
                                    <v-btn
                                        outlined
                                        color="primary">
                                        Apply
                                    </v-btn>
                                </v-col>
                            </v-row>
                        </div>
                    </v-col>
                    <v-col cols="auto" class="d-flex justify-center">
                        <v-divider vertical/>
                    </v-col>
                    <v-col cols="8" class="justify-center align-center">
                        <contents v-for="it in 3" :key="it">
                            <v-card
                                class="my-2"
                                outlined>
                                <v-card-title>
                                    <div class="headline text-truncate py-1">Service {{ it }}</div>
                                </v-card-title>
                                <v-divider></v-divider>
                                <v-card-actions class="py-0">
                                    <v-chip dark disabled text-color="black" color="transparent">
                                        <div class="subheading">
                                            {{ it*3 }}
                                        </div>
                                        <v-icon small class="ml-1">mdi-currency-eur</v-icon>
                                    </v-chip>
                                    <v-chip dark disabled text-color="black" color="transparent">
                                        <div class="subheading">
                                            01:00
                                        </div>
                                        <v-icon small class="ml-1">mdi-clock-outline</v-icon>
                                    </v-chip>
                                    <v-spacer></v-spacer>
                                    <v-btn text color="primary">
                                        Select
                                    </v-btn>
                                </v-card-actions>
                            </v-card>
                        </contents>
                    </v-col>
                </v-row>
            </v-card-text>
        </v-card>
    </v-container>
</template>
<script lang="ts">

    import { Component, mixins } from "../commons";
    import PageBase from "../commons/mixins/page-base";

    @Component
    export default class ServicesPage extends mixins(PageBase)
    {
        location: any = null;
        specialist: any = null;
        locations: any[] = [{
            address: "Address st. 15, Kaunas"
        }, {
            address: "Address st. 14, Vilnius"
        }]

        specialists: any[] = [{
            label: "Specialist 1"
        }, {
            label: "Specialist 2"
        }]
    }

</script>
<style lang="scss">

    .v-services-container {
        height: 100%;
        padding-top: 51px;
        .v-services-wrapper {
            height: 100%;
            position: relative;
        }
    }
    .services-toolbar { border-top: 1px solid rgba($color: #000000, $alpha: 0.1); }
    .v-services-category-active { background-color: rgba($color: #000000, $alpha: 0.1) !important; }
    .v-services-search-field { width: 220px !important; }
    .v-services-drawer { border-right: 1px solid rgba($color: #000000, $alpha: 0.3); }

</style>
