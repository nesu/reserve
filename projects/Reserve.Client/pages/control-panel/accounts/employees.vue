<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Employees
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
                            label="Search employee"
                            dense outlined
                            hide-details/>
                    </div>
                    <v-spacer/>
                    <v-btn
                        text color="primary"
                        v-authorized="$roles.Administrator"
                        @click="creating_employee = true">
                        Create employee
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
                                @click="modify_employee(item)">
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
            @updated:accounts="load_employees"
            :active.sync="creating_employee"/>
        <account-editor
            v-if="modifying_employee && employee_id !== 0"
            @updated:accounts="load_employees"
            :active.sync="modifying_employee"
            :account_id="employee_id"/>
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
            title: "Control Panel - Employees"
        },
        components: {
            "account-creator": require("~/components/management/account-creator").default,
            "account-editor": require("~/components/management/account-editor").default,
        },
        async asyncData()
        {
            const employees = await get_account_collection({
                role: RoleType.Employee,
                page_size: 10,
                page_index: 1
            });

            return {
                employees: employees
            }
        }
    })
    export default class EmployeesPage extends mixins(ManagementBase, DateFunctions)
    {
        search_literal: string = "";

        loading: boolean = false;
        employees: Account[] = [];

        creating_employee: boolean = false;
        modifying_employee: boolean = false;
        employee_id: number = 0;

        collection_request: AccountCollectionRequest = {
            page_index: 1,
            page_size: 10,
            role: RoleType.Employee
        }

        get datatable_scheme()
        {
            return {
                items: this.employees,
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

        modify_employee(account: Account) {
            this.modifying_employee = true;
            this.employee_id = account.id;
        }

        load_employees()
        {
            this.loading = true;
            get_account_collection(this.collection_request)
                .then((response) => {
                    this.employees = response;
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        @Watch("collection_request.page_size")
        @Watch("collection_request.page_index")
        collection_load() {
            this.load_employees();
        }

        @Watch("search_literal")
        search_location(value: string | null)
        {
            if (!value || value === "")
            {
                this.load_employees();
                return;
            }

            if (value.length < 3) {
                return;
            }

            const fuse = new Fuse(this.employees, {
                findAllMatches: true,
                matchAllTokens: true,
                keys: ["given_name", "family_name", "email", "phone_number"]
            });

            this.employees = fuse
                .search(value)
                .map(it => it.item);
        }
    }

</script>
