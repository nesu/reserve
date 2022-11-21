<template>
    <v-dialog
        v-model="active"
        persistent width="600">
        <v-card
            v-if="loading"
            :loading="loading"
            tile flat>
            <v-card-text class="text-center">
                <v-progress-circular
                    size="34"
                    indeterminate/>
            </v-card-text>
        </v-card>
        <v-card
            v-else
            tile flat>
            <v-toolbar
                flat
                fixed
                height="76"
                class="pl-4"
                color="transparent"
                width="100%"
            >
                <v-toolbar-title>
                    Edit category
                </v-toolbar-title>
                <v-spacer/>
                <v-dialog
                    v-model="creating_subcategory"
                    width="500"
                    persistent>
                    <template v-slot:activator="{ on }">
                        <v-btn
                            icon
                            v-on="on">
                            <v-icon>mdi-briefcase-edit</v-icon>
                        </v-btn>
                    </template>
                    <v-card
                        flat tile>
                        <v-toolbar
                            flat
                            fixed
                            height="76"
                            class="pl-4"
                            color="transparent"
                            width="100%"
                        >
                            <v-toolbar-title>
                                Create subdirectory
                            </v-toolbar-title>
                            <v-spacer/>
                            <v-btn
                                icon
                                class="mr-2"
                                @click="creating_subcategory = false">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-toolbar>
                        <v-divider class="mx-4"/>
                        <v-card-text>
                            <v-text-field
                                v-model="subcategory_payload.label"
                                outlined hide-details
                                label="Subcategory name"/>
                        </v-card-text>
                        <v-divider class="mx-4"/>
                        <v-card-actions>
                            <v-spacer/>
                            <v-btn
                                @click="create_subcategory"
                                text color="primary">
                                Create
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
                <v-btn
                    icon
                    class="mr-2"
                    @click="close">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-row>
                    <v-col cols="8">
                        <v-text-field
                            v-model="category.label"
                            outlined hide-details
                            label="Category name"/>
                    </v-col>
                    <v-col cols="4">
                        <v-btn
                            @click="alter_category_label"
                            x-large block color="primary">
                            Update
                        </v-btn>
                    </v-col>
                </v-row>
            </v-card-text>
            <v-divider class="mx-4"/>
            <v-card-text>
                <v-row>
                    <v-col
                        cols="12">
                        <div
                            v-if="!category.child_categories || Array.isArray(category.child_categories) && category.child_categories.length === 0"
                            class="text-center">
                            No subcategories created
                        </div>
                        <v-simple-table
                            v-else
                            fixed-header
                            height="200">
                            <template v-slot:default>
                                <thead>
                                    <tr>
                                        <th class="text-left">
                                            Category name
                                        </th>
                                        <th class="text-right">
                                            Actions
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(it, index) in category.child_categories" :key="it.id">
                                        <td>
                                            <v-edit-dialog
                                                @save="alter_category_subcategory_label(it, index)">
                                                {{ it.label }}
                                                <template v-slot:input>
                                                    <v-text-field
                                                        label="Category name"
                                                        single-line
                                                        v-model="it.label">
                                                    </v-text-field>
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td class="text-right">
                                            <confirmable-modal
                                                v-on:confirmed="delete_category(it.id)">
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
                                                    Deleting this subcategory will also remove every created service which uses
                                                    this category. Please confirm this deletion.
                                                </template>
                                                <template v-slot:modal-confirmation-text>
                                                    Delete
                                                </template>
                                            </confirmable-modal>
                                        </td>
                                    </tr>
                                </tbody>
                            </template>
                        </v-simple-table>
                    </v-col>
                </v-row>
            </v-card-text>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">

    import { Component, mixins, Property, Watch } from "~/commons";
    import ComponentBase from "~/commons/mixins/component-base";
    import InputValidators from "~/commons/mixins/input-validators";
    import {
        create_category,
        delete_category,
        get_category,
        save_category
    } from "~/commons/requests/management/categories";
    import { CreateCategoryRequest } from "~/commons/requests/models/create-category-request";
    import { ServiceCategory } from "~/commons/resources/service-category";

    @Component({
        components: {
            "confirmable-modal": require("~/components/confirmable-modal").default
        },
    })
    export default class CategoryEditor extends mixins(ComponentBase, InputValidators)
    {
        @Property({
            required: true,
            type: Boolean
        })
        active!: boolean;

        @Property({
            required: true,
            type: Number
        })
        category_id!: number;

        loading: boolean = true;
        category: ServiceCategory | null = null;

        creating_subcategory: boolean = false;
        subcategory_payload: CreateCategoryRequest = {
            label: ""
        }

        created()
        {
            if (!this.category_id) {
                this.close();
                return;
            }

            this.load_category();
        }

        delete_category(category_id: number)
        {
            delete_category({ id: category_id })
                .then(() => {
                    this.$notify_message("Category deleted");
                    this.load_category();
                    this.$emit("updated:categories");
                });
        }

        close() {
            this.$emit("update:active", false);
        }

        alter_category_label()
        {
            if (!this.category) {
                return;
            }

            save_category(this.category)
                .then((response) =>
                {
                    this.category = response;
                    this.$notify_message("Category name changed");
                    this.$emit("updated:categories");
                });
        }

        alter_category_subcategory_label(category: ServiceCategory, index: number)
        {
            save_category(category)
                .then((response) =>
                {
                    this.$notify_message("Saved");
                    if (!!this.category?.child_categories && this.category.child_categories.length > 0) {
                        this.$set(this.category.child_categories, index, response);
                    }
                })
                .catch((error) => {
                    this.$notify_error(error);
                });
        }

        create_subcategory()
        {
            if (!this.category) {
                return;
            }

            const payload = this.subcategory_payload;

            if (payload.label === "") {
                return;
            }

            payload.parent_category_id = this.category.id;

            create_category(payload)
                .then((response) =>
                {
                    this.$notify_message("Subcategory created");
                    this.$emit("updated:categories");

                    if (!!this.category)
                    {
                        if (!Array.isArray(this.category.child_categories)) {
                            this.category.child_categories = [response];
                        }
                        else {
                            this.category.child_categories.push(response);
                        }
                    }
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.creating_subcategory = false;
                });
        }

        load_category()
        {
            get_category(this.category_id)
                .then((response) => {
                    this.category = response;
                })
                .finally(() => {
                    this.loading = false;
                });
        }
    }

</script>
