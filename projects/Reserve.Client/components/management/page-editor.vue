<template>
    <v-dialog
        v-model="active"
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
                                Create new page
                            </v-toolbar-title>
                            <v-spacer/>
                            <v-btn
                                icon class="mr-2"
                                @click="close">
                                <v-icon>mdi-close</v-icon>
                            </v-btn>
                        </v-toolbar>
                    </div>
                    <v-divider class="mx-4"/>
                    <div
                        class="pt-6 flex-grow-1">
                        <contents
                            v-if="!preview">
                            <v-card-text>
                                <v-form
                                    v-model="create_page_submittable">
                                    <v-text-field
                                        outlined
                                        label="Page title"
                                        :rules="validators.required"
                                        v-model="create_page_payload.title"/>
                                    <v-text-field
                                        outlined
                                        label="Unique page identifier"
                                        hint="Used in URL: /pages/<identifier>"
                                        :rules="validators.required"
                                        v-model="create_page_payload.identifier"/>
                                    <v-textarea
                                        v-model="create_page_payload.contents"
                                        label="Contents"
                                        rows="8"
                                        class="mt-8 font-regular"
                                        outlined>
                                    </v-textarea>
                                </v-form>
                            </v-card-text>
                        </contents>
                        <contents
                            v-else>
                            <v-card flat tile>
                                <v-toolbar
                                    flat tile
                                    height="56">
                                    <v-toolbar-title class="headline">
                                        {{ create_page_payload.title ? create_page_payload.title : "Page title unspecified" }}
                                    </v-toolbar-title>
                                </v-toolbar>
                                <v-divider class="mx-4"/>
                                <v-card-text>
                                    <vue-markdown
                                        class="markdown"
                                        :source="create_page_payload.contents"/>
                                </v-card-text>
                            </v-card>
                        </contents>
                    </div>
                    <v-divider class="mx-4"/>
                    <v-card-actions>
                        <v-btn
                            large
                            outlined
                            color="primary"
                            :disabled="!create_page_submittable"
                            :loading="create_page_loading"
                            @click.stop="create_page">
                            Create page
                        </v-btn>
                        <v-spacer/>
                        <v-btn
                            text large
                            color="teal darken-4"
                            v-if="!preview"
                            @click.stop="preview = !preview">
                            Preview
                        </v-btn>
                        <v-btn
                            text large
                            color="teal darken-4"
                            v-else
                            @click.stop="preview = !preview">
                            Edit
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-container>
        </v-sheet>
    </v-dialog>
</template>
<script lang="ts">

    import VueMarkdown from 'vue-markdown'
    import { Component, mixins, Property } from "~/commons";
    import InputValidators from "~/commons/mixins/input-validators";
    import PageBase from "~/commons/mixins/page-base";
    import { create_page } from "~/commons/requests/layout";
    import CreatePageRequest from "~/commons/requests/models/create-page-request";

    @Component({
        components: {
            "vue-markdown": VueMarkdown
        }
    })
    export default class CreatePage extends mixins(PageBase, InputValidators)
    {
        @Property({
            type: Boolean,
            required: true
        })
        active!: boolean;

        preview: boolean = false;

        create_page_submittable: boolean = false;
        create_page_loading: boolean = false;
        create_page_payload: CreatePageRequest = {
            title: "",
            identifier: "",
            contents: ""
        }

        close() {
            this.$emit("update:active", false);
        }

        create_page()
        {
            if (!this.create_page_submittable || this.create_page_loading) {
                return;
            }

            this.create_page_loading = true;
            create_page(this.create_page_payload)
                .then(() => {
                    this.$notify_message("Page created successfully.");
                    this.$emit("updated:pages");
                    this.close();
                })
                .catch((error) => {
                    this.$notify_error(error);
                })
                .finally(() => {
                    this.create_page_loading = false;
                });
        }
    }

</script>
