<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Categories
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
                            label="Search category"
                            dense outlined
                            clearable hide-details/>
                    </div>
                    <v-spacer/>
                    <v-menu
                        v-model="creating_parent"
                        :nudge-width="150"
                        :nudge-bottom="50"
                        :close-on-content-click="false"
                        offset-x>
                        <template v-slot:activator="{ on }">
                            <v-btn
                                text
                                v-on="on"
                                color="primary">
                                Create parent category
                            </v-btn>
                        </template>
                        <v-card flat tile>
                            <v-card-text>
                                <v-text-field
                                    label="Parent category name"
                                    v-model="create_parent_payload.label"
                                    outlined hide-details>
                                </v-text-field>
                            </v-card-text>
                            <v-divider/>
                            <v-card-actions>
                                <v-spacer/>
                                <v-btn
                                    text
                                    @click="create_parent_category"
                                    color="primary">
                                    Create
                                </v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-menu>
                </v-toolbar>
                <v-card-text>
                    <v-data-table
                        :loading="loading"
                        v-bind="datatable_scheme">
                        <template v-slot:item.subcategory_count="{ item }">
                            {{ item.child_categories.length }}
                        </template>
                        <template v-slot:item.created_at="{ value }">
                            {{ $format_date(value, 'yyyy-MM-dd HH:mm') }}
                        </template>
                        <template v-slot:item.updated_at="{ value }">
                            {{ $format_date(value, 'yyyy-MM-dd HH:mm') }}
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <v-tooltip bottom>
                                <template v-slot:activator="{ on }">
                                    <v-btn
                                        v-on="on"
                                        @click="activate_category_editor(item)"
                                        small icon>
                                        <v-icon>mdi-briefcase-edit</v-icon>
                                    </v-btn>
                                </template>
                                <span>Manage category children</span>
                            </v-tooltip>
                            <confirmable-modal
                                v-on:confirmed="delete_category(item.id)">
                                <template v-slot:activator="{ on }">
                                    <v-btn
                                        small icon color="red"
                                        v-on="on">
                                        <v-icon>mdi-delete</v-icon>
                                    </v-btn>
                                </template>
                                <template v-slot:modal-title>
                                    Are you sure?
                                </template>
                                <template v-slot:modal-body>
                                    Deleting this category will also remove subcategories and related services.
                                    Please confirm this action.
                                </template>
                                <template v-slot:modal-confirmation-text>
                                    Delete
                                </template>
                            </confirmable-modal>
                        </template>
                    </v-data-table>
                </v-card-text>
            </v-card>
            <category-editor
                v-if="category_editor.active"
                :category_id="category_editor.category_id"
                :active.sync="category_editor.active"
                @updated:categories="load_categories">
            </category-editor>
        </v-container>
    </contents>
</template>
<script lang="ts">

    import Fuse from 'fuse.js';
    import { Component, mixins, Watch } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import { create_category, delete_category, get_base_categories } from "~/commons/requests/management/categories";
    import { CreateCategoryRequest } from "~/commons/requests/models/create-category-request";
    import { ServiceCategory } from "~/commons/resources/service-category";
    import ManagementBase from "~/commons/mixins/management-base";

    @Component({
        head: {
            title: "Control Panel - Service Categories"
        },
        components: {
            "category-editor": require("~/components/management/category-editor.vue").default,
            "confirmable-modal": require("~/components/confirmable-modal").default
        },
        async asyncData()
        {
            const categories = await get_base_categories();

            return {
                categories
            }
        }
    })
    export default class ServiceCategoriesPage extends mixins(ManagementBase, DateFunctions)
    {
        loading: boolean = false;
        search_literal: string = "";

        creating_parent: boolean = false;
        create_parent_payload: CreateCategoryRequest = {
            label: ""
        };

        categories: ServiceCategory[] = [];
        category_editor = {
            active: false,
            category_id: 0
        };

        get datatable_scheme()
        {
            return {
                items: this.categories,
                headers: [{
                    text: "Category name",
                    value: "label",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Date created",
                    value: "created_at",
                    sortable: false,
                    filterable: false
                },{
                    text: "Date modified",
                    value: "updated_at",
                    sortable: false,
                    filterable: false
                },{
                    text: "Subcategory count",
                    value: "subcategory_count",
                    sortable: false,
                    filterable: false
                }, {
                    text: "Actions",
                    align: "right",
                    value: "actions",
                    sortable: false,
                    filterable: false
                }]
            }
        }

        create_parent_category()
        {
            const payload = this.create_parent_payload;

            if (payload.label === null || payload.label.length < 1) {
                return;
            }

            this.loading = true;
            create_category(payload)
                .then((response) => {
                    this.$notify_message("Parent category created");
                    this.categories.push(response);
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.loading = false;
                    this.creating_parent = false;
                    this.create_parent_payload = {
                        label: ""
                    }
                });
        }

        activate_category_editor(category: ServiceCategory) {
            this.category_editor.active = true;
            this.category_editor.category_id = category.id;
        }

        delete_category(category_id: number)
        {
            delete_category({ id: category_id })
                .then(() => {
                    this.$notify_message("Category deleted");
                    this.load_categories();
                });
        }

        load_categories()
        {
            get_base_categories()
                .then((response) => {
                    this.categories = response;
                });
        }

        @Watch("search_literal")
        search_category(value: string | null)
        {
            if (!value || value === "")
            {
                this.loading = true;
                get_base_categories()
                    .then((response) => {
                        this.categories = response;
                    })
                    .finally(() => {
                        this.loading = false;
                    });

                return;
            }

            if (value.length < 3) {
                return;
            }

            const fuse = new Fuse(this.categories, {
                findAllMatches: true,
                matchAllTokens: true,
                keys: ["label"]
            });

            this.categories = fuse
                .search(value)
                .map(it => it.item);
        }
    }

</script>
