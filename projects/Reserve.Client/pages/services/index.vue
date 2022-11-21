<template>
    <v-container
        class="pt-6 fill-height">
        <v-row
            align="center"
            justify="center"
            class="fill-height">
            <v-col
                cols="12" sm="auto">
                <v-card
                    flat tile
                    color="transparent"
                    max-width="380">
                    <div class="initial-choice"/>
                    <v-card-text
                        class="pt-12">
                        <v-autocomplete
                            label="Preferred location"
                            :items="locations"
                            v-model="location"
                            :item-text="(it) => `${it.address}`"
                            item-value="id"
                            class="max-width-380"
                            outlined color="primary"
                            @change="load_employees"/>
                        <v-autocomplete
                            label="Preferred specialist"
                            :items="employees"
                            v-model="employee"
                            :item-text="(it) => `${it.given_name} ${it.family_name}`"
                            :disabled="employees.length === 0"
                            :loading="employees_loading"
                            item-value="id"
                            class="max-width-380"
                            outlined color="primary"/>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn
                            x-large block
                            @click="redirect"
                            color="primary">
                            Continue
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>
<script lang="ts">

    import { get_compact_employees } from "~/commons/requests/general/employees";
    import { get_locations } from "~/commons/requests/general/locations";
    import { CompactEmployee } from "~/commons/resources/compact-employee";
    import { Location } from "~/commons/resources/location";
    import { Component, mixins, Watch } from "../../commons";
    import PageBase from "../../commons/mixins/page-base";

    @Component({
        head: {
            title: "Services"
        },
        async asyncData()
        {
            const locations = await get_locations();
            return {
                locations
            }
        }
    })
    export default class ServicesPage extends mixins(PageBase)
    {
        employees_loading: boolean = false;
        employees: CompactEmployee[] = [];

        locations: Location[] = [];

        employee: number = 0;
        location: number = 0;

        redirect()
        {
            this.$router.push({
                path: "/services/listing",
                query: {
                    specialist: `${this.employee}`
                }
            });
        }

        load_employees(value: number)
        {
            this.employees_loading = true;
            get_compact_employees(value)
                .then((response) => {
                    this.employees = response;
                })
                .finally(() => {
                    this.employees_loading = false;
                });
        }
    }

</script>
<style lang="scss">
    @import "../../assets/style/pages/services";
</style>
