<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Customers
            </template>
        </default-toolbar>
        <v-container
            class="management-layout">
            <v-card
                outlined>
                <v-toolbar
                    class="pt-4" flat tile>
                    <div
                        class="d-flex flex-row-reverse px-4">
                        <v-text-field
                            v-model="search_literal"
                            append-icon="mdi-magnify"
                            label="Search customer"
                            dense outlined
                            hide-details/>
                    </div>
                    <v-spacer/>
                    <v-btn
                        text color="primary"
                        v-authorized="$roles.Administrator"
                        @click="creating_customer = true">
                        Create customer
                    </v-btn>
                </v-toolbar>
                <v-card-text>
                    <v-data-table
                        v-bind="datatable_scheme"
                        :page.sync="collection_request.page_index"
                        :items-per-page.sync="collection_request.page_size">
                        <template v-slot:item.given_name="{ value }">
                            {{ value === null || value.length === 0 ? '-' : value }}
                        </template>
                        <template v-slot:item.family_name="{ value }">
                            {{ value === null || value.length === 0 ? '-' : value }}
                        </template>
                        <template v-slot:item.created_at="{ value }">
                            {{ $format_date(value, 'yyyy-MM-dd HH:mm') }}
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <v-btn
                                small icon
                                v-authorized="$roles.Administrator"
                                @click="modify_customer(item)">
                                <v-icon>mdi-account-edit</v-icon>
                            </v-btn>
                            <span
                                v-authorized="$roles.Employee">
                                -
                            </span>
                        </template>
                    </v-data-table>
                </v-card-text>
            </v-card>
        </v-container>
        <account-creator
            @updated:accounts="load_customers"
            :active.sync="creating_customer"/>
        <account-editor
            v-if="modifying_customer && customer_id !== 0"
            @updated:accounts="load_customers"
            :active.sync="modifying_customer"
            :account_id="customer_id"/>
    </contents>
</template>
<script lang="ts">

    import Fuse from "fuse.js";
    import { Component, mixins, Watch } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import ManagementBase from "~/commons/mixins/management-base";
    import { get_account_collection } from "~/commons/requests/management/accounts";
    import { AccountCollectionRequest } from "~/commons/requests/models/account-collection-request";
    import Account from "~/commons/resources/account";
    import { RoleType } from "~/commons/resources/role-type";

    @Component({
        head: {
            title: "Control Panel - Customers"
        },
        components: {
            "account-creator": require("~/components/management/account-creator").default,
            "account-editor": require("~/components/management/account-editor").default,
        },
        async asyncData()
        {
            const customers = await get_account_collection({
                role: RoleType.Client,
                page_size: 10,
                page_index: 1
            });

            return {
                customers
            }
        }
    })
    export default class CustomersPage extends mixins(ManagementBase, DateFunctions)
    {
        search_literal: string = "";

        loading: boolean = false;
        customers: Account[] = [];

        creating_customer: boolean = false;
        modifying_customer: boolean = false;
        customer_id: number = 0;

        collection_request: AccountCollectionRequest = {
            page_index: 1,
            page_size: 10,
            role: RoleType.Client
        }

        get datatable_scheme()
        {
            return {
                items: this.customers,
                headers: [{
                    text: "Name",
                    value: "given_name",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Surname",
                    value: "family_name",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Email address",
                    value: "email",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Registration date",
                    value: "created_at",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Actions",
                    value: "actions",
                    align: "right",
                    sortable: false,
                    filterable: false
                }]
            }
        }

        modify_customer(account: Account) {
            this.modifying_customer = true;
            this.customer_id = account.id;
        }

        load_customers()
        {
            this.loading = true;
            get_account_collection(this.collection_request)
                .then((response) => {
                    this.customers = response;
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        @Watch("collection_request.page_size")
        @Watch("collection_request.page_index")
        collection_load() {
            this.load_customers();
        }

        @Watch("search_literal")
        search_location(value: string | null)
        {
            if (!value || value === "")
            {
                this.load_customers();
                return;
            }

            if (value.length < 3) {
                return;
            }

            const fuse = new Fuse(this.customers, {
                findAllMatches: true,
                matchAllTokens: true,
                keys: ["given_name", "family_name", "email", "phone_number"]
            });

            this.customers = fuse
                .search(value)
                .map(it => it.item);
        }
    }

</script>
