<template>
    <contents>
        <v-row>
            <v-col
                cols="12">
                <v-card
                    outlined>
                    <v-toolbar
                        flat tile
                        height="56">
                        <v-toolbar-title class="headline">
                            Created pages
                        </v-toolbar-title>
                        <v-spacer/>
                        <v-btn
                            text
                            color="primary"
                            nuxt to="/control-panel/pages/create-page">
                            Create page
                        </v-btn>
                    </v-toolbar>
                    <v-divider class="mx-4"/>
                    <v-list>
                        <div
                            :key="page.id"
                            v-for="(page, index) in pages">
                            <v-list-item
                                two-line>
                                <v-list-item-content>
                                    <v-list-item-title>
                                        {{ page.title }}
                                    </v-list-item-title>
                                    <v-list-item-subtitle>
                                        {{ `/pages/${page.identifier}` }}
                                    </v-list-item-subtitle>
                                </v-list-item-content>
                                <v-list-item-action class="mr-2">
                                    <v-btn
                                        nuxt :to="`/pages/${page.identifier}`"
                                        target="_blank" icon>
                                        <v-icon>mdi-eye</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                                <v-list-item-action>
                                    <v-btn
                                        nuxt :to="`/control-panel/pages/edit-page/${page.identifier}`"
                                        icon>
                                        <v-icon>mdi-file-document-edit</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                                <v-list-item-action>
                                    <v-btn
                                        @click.stop="delete_page_payload.id = page.id"
                                        class="red--text"
                                        icon>
                                        <v-icon>mdi-delete</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                            </v-list-item>
                            <v-divider
                                class="mx-4"
                                v-if="index !== (pages.length - 1)"/>
                        </div>
                    </v-list>
                </v-card>
            </v-col>
            <v-col
                cols="12">
                <v-card
                    outlined>
                    <v-toolbar
                        flat tile
                        height="56">
                        <v-toolbar-title class="headline">
                            Navigator elements
                        </v-toolbar-title>
                        <v-spacer/>
                        <v-btn
                            text
                            color="primary"
                            nuxt to="/control-panel/layout/create-navigator-element">
                            Create new
                        </v-btn>
                    </v-toolbar>
                    <v-divider class="mx-4"/>
                    <v-list>
                        <div
                            v-for="(nel, index) in navigator_elements"
                            :key="nel.id">
                            <v-list-item
                                two-line>
                                <v-list-item-content>
                                    <v-list-item-title>
                                        {{ nel.title }}
                                    </v-list-item-title>
                                    <v-list-item-subtitle>
                                        {{ nel.redirect_to }}
                                    </v-list-item-subtitle>
                                </v-list-item-content>
                                <v-list-item-action>
                                    <v-btn
                                        nuxt :to="`/control-panel/layout/edit-navigator-element/${nel.id}`"
                                        icon>
                                        <v-icon>mdi-file-document-edit</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                                <v-list-item-action>
                                    <v-btn
                                        @click.stop="delete_navigator_element_payload.id = nel.id"
                                        class="red--text"
                                        icon>
                                        <v-icon>mdi-delete</v-icon>
                                    </v-btn>
                                </v-list-item-action>
                            </v-list-item>
                            <v-divider
                                class="mx-4"
                                v-if="index !== (navigator_elements.length - 1)"/>
                        </div>
                    </v-list>
                </v-card>
            </v-col>
        </v-row>
        <v-dialog
            persistent
            width="400"
            :value="delete_page_payload.id !== 0">
            <v-card
                flat tile>
                <v-toolbar
                    flat tile
                    height="56">
                    <v-toolbar-title class="headline">
                        Confirmation required
                    </v-toolbar-title>
                </v-toolbar>
                <v-divider class="mx-4"/>
                <v-card-text>
                    Are you sure you want to delete this page?
                </v-card-text>
                <v-divider class="mx-4"/>
                <v-card-actions>
                    <v-btn
                        outlined color="primary"
                        @click.stop="delete_page_payload.id = 0">
                        Cancel
                    </v-btn>
                    <v-spacer/>
                    <v-btn
                        text color="red"
                        @click.stop="delete_page">
                        Delete page
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog
            persistent
            width="400"
            :value="delete_navigator_element_payload.id !== 0">
            <v-card
                flat tile>
                <v-toolbar
                    flat tile
                    height="56">
                    <v-toolbar-title class="headline">
                        Confirmation required
                    </v-toolbar-title>
                </v-toolbar>
                <v-divider class="mx-4"/>
                <v-card-text>
                    Are you sure you want to delete this navigator element?
                </v-card-text>
                <v-divider class="mx-4"/>
                <v-card-actions>
                    <v-btn
                        outlined color="primary"
                        @click.stop="delete_navigator_element_payload.id = 0">
                        Cancel
                    </v-btn>
                    <v-spacer/>
                    <v-btn
                        text color="red"
                        @click.stop="delete_navigator_element">
                        Delete element
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </contents>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import NavigatorElement from "~/commons/lib/navigator-element";
    import ManagementBase from "~/commons/mixins/management-base";
    import PageBase from "~/commons/mixins/page-base";
    import {
        delete_navigator_element,
        delete_page,
        get_created_pages,
        get_navigator_elements
    } from "~/commons/requests/layout";
    import DeleteNavigatorElementRequest from "~/commons/requests/models/delete-navigator-element-request";
    import DeletePageRequest from "~/commons/requests/models/delete-page-request";
    import NavigatorElementResource from "~/commons/resources/navigator-element-resource";
    import Page from "~/commons/resources/page";

    @Component({
        async asyncData(context)
        {
            const pages = await get_created_pages();
            const navigator_elements = await get_navigator_elements();

            return {
                pages,
                navigator_elements
            }
        }
    })
    export default class PageManagement extends mixins(ManagementBase)
    {
        pages: Page[] = [];
        navigator_elements: NavigatorElementResource[] = [];

        delete_page_loading: boolean = false;
        delete_page_payload: DeletePageRequest = {
            id: 0
        }

        delete_navigator_element_loading: boolean = false;
        delete_navigator_element_payload: DeleteNavigatorElementRequest = {
            id: 0
        };

        delete_page()
        {
            if (this.delete_page_loading || this.delete_page_payload.id === 0) {
                return;
            }

            this.delete_page_loading = true;
            delete_page(this.delete_page_payload).then(() =>
            {
                return get_created_pages().then((pages) => {
                    this.$notify_message("Page was deleted.");
                    this.pages = pages;
                });
            })
            .catch((error) => {
                 this.$notify_error(error);
            })
            .finally(() => {
                this.delete_page_loading = false;
                this.delete_page_payload.id = 0;
            });
        }

        delete_navigator_element()
        {
            if (this.delete_navigator_element_loading || this.delete_navigator_element_payload.id === 0) {
                return;
            }

            this.delete_navigator_element_loading = true;
            delete_navigator_element(this.delete_navigator_element_payload).then(() =>
            {
                this.$modules.layout.preload_visuals();
                return get_navigator_elements().then((elements) => {
                    this.$notify_message("Navigator element was deleted.");
                    this.navigator_elements = elements;
                });
            })
            .catch((error) => {
                this.$notify_error(error);
            })
            .finally(() => {
                this.delete_navigator_element_loading = false;
                this.delete_navigator_element_payload.id = 0;
            });
        }
    }

</script>
