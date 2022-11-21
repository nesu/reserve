<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Pages
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
                            label="Search page"
                            dense outlined
                            hide-details/>
                    </div>
                    <v-spacer/>
                    <v-btn
                        @click="creating_page = true"
                        text color="primary">
                        Create page
                    </v-btn>
                </v-toolbar>
                <v-card-text>
                    <v-data-table
                        :items="pages"
                        v-bind="table_schema">
                        <template v-slot:item.created_at="{ value }">
                            {{ $format_date(value, 'yyyy-MM-dd HH:mm') }}
                        </template>
                        <template v-slot:item.updated_at="{ value }">
                            {{ $format_date(value, 'yyyy-MM-dd HH:mm') }}
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <contents
                                class="d-flex flex-row justify-end">
                                <v-btn
                                    nuxt :to="`/pages/${item.identifier}`"
                                    target="_blank" icon>
                                    <v-icon>mdi-eye</v-icon>
                                </v-btn>
                                <v-btn
                                    icon
                                    nuxt :to="`/control-panel/pages/edit-page/${item.identifier}`">
                                    <v-icon>mdi-file-document-edit</v-icon>
                                </v-btn>
                                <confirmable-modal
                                    v-on:confirmed="delete_page(item.id)">
                                    <template v-slot:activator="{ on }">
                                        <v-btn
                                            color="red"
                                            icon v-on="on">
                                            <v-icon>mdi-delete</v-icon>
                                        </v-btn>
                                    </template>
                                    <template v-slot:modal-title>
                                        Are you sure?
                                    </template>
                                    <template v-slot:modal-body>
                                        Are you sure you want to remove this page?
                                    </template>
                                    <template v-slot:modal-confirmation-text>
                                        Delete
                                    </template>
                                </confirmable-modal>
                            </contents>
                        </template>
                    </v-data-table>
                </v-card-text>
            </v-card>
        </v-container>
        <page-creator
            :active.sync="creating_page"
            @updated:pages="load_pages"/>
    </contents>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import DateFunctions from "~/commons/mixins/date-functions";
    import ManagementBase from "~/commons/mixins/management-base";
    import { delete_page, get_created_pages } from "~/commons/requests/layout";
    import Page from "~/commons/resources/page";

    @Component({
        components: {
            "confirmable-modal": require("~/components/confirmable-modal").default,
            "page-creator": require("~/components/management/page-creator").default
        },
        async asyncData(context)
        {
            return {
                pages: await get_created_pages()
            }
        }
    })
    export default class PageControls extends mixins(ManagementBase, DateFunctions)
    {
        loading: boolean = true;
        creating_page: boolean = false;
        search_literal: string = "";

        pages: Page[] = [];
        selected_pages: Page[] = [];

        delete_page(pid: number)
        {
            delete_page({ id: pid })
                .then(() =>
                {
                    return get_created_pages().then((pages) => {
                        this.$notify_message("Page was deleted.");
                        this.pages = pages;
                    });
                })
                .catch((error) => {
                    this.$notify_error(error);
                });
        }

        load_pages()
        {
            get_created_pages()
                .then((response) => {
                    this.pages = response;
                })
                .finally(() => {
                    this.loading = false;
                });
        }

        get table_schema()
        {
            // noinspection UnnecessaryLocalVariableJS
            const schema =
            {
                headers: [{
                    text: "Title",
                    value: "title",
                    align: "left",
                    sortable: true,
                    filterable: true
                }, {
                   text: "Identifier",
                   value: "identifier",
                   align: "left",
                   sortable: false,
                   filterable: false
                }, {
                    text: "Creation date",
                    value: "created_at",
                    align: "left",
                    sortable: true,
                    filterable: false
                }, {
                    text: "Last modified",
                    value: "updated_at",
                    align: "left",
                    sortable: true,
                    filterable: false
                }, {
                    text: "Actions",
                    value: "actions",
                    align: "right",
                    sortable: false,
                    filterable: false
                }]
            }

            return schema;
        }
    }


</script>
