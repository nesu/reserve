<template>
    <v-container>
        <v-row>
            <v-col cols="12">
                <contents
                    v-if="!preview">
                    <v-col
                        cols="12"
                        class="px-0">
                        <v-card
                            v-if="!preview"
                            outlined>
                            <v-toolbar
                                flat tile
                                height="56">
                                <v-toolbar-title class="headline">
                                    Create new page
                                </v-toolbar-title>
                                <v-spacer/>
                                <v-btn
                                    text
                                    color="primary"
                                    nuxt exact to="/control-panel/pages">
                                    Back
                                </v-btn>
                            </v-toolbar>
                            <v-divider class="mx-4"/>
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
                        </v-card>
                    </v-col>
                    <v-col
                        cols="12"
                        class="px-0">
                        <v-card
                            outlined>
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
                                    @click.stop="preview = !preview">
                                    Preview
                                </v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-col>
                </contents>
                <contents v-else>
                    <v-row>
                        <v-col
                            cols="12"
                            class="px-0">
                            <v-card
                                outlined>
                                <v-toolbar
                                    flat tile
                                    height="56">
                                    <v-toolbar-title class="headline">
                                        {{ create_page_payload.title ? create_page_payload.title : "Page title unspecified" }}
                                    </v-toolbar-title>
                                    <v-spacer/>
                                    <v-btn
                                        text
                                        color="primary"
                                        nuxt exact to="/control-panel/pages">
                                        Back
                                    </v-btn>
                                </v-toolbar>
                                <v-divider class="mx-4"/>
                                <v-card-text>
                                    <vue-markdown
                                        class="markdown"
                                        :source="create_page_payload.contents"/>
                                </v-card-text>
                            </v-card>
                        </v-col>
                        <v-col
                            cols="12"
                            class="px-0">
                            <v-card
                                outlined>
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
                                        @click.stop="preview = !preview">
                                        Edit
                                    </v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-col>
                    </v-row>
                </contents>
            </v-col>
        </v-row>
    </v-container>
</template>
<script lang="ts">

    import VueMarkdown from 'vue-markdown'
    import { Component, mixins } from "~/commons";
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
        preview: boolean = false;

        create_page_submittable: boolean = false;
        create_page_loading: boolean = false;
        create_page_payload: CreatePageRequest = {
            title: "",
            identifier: "",
            contents: ""
        }

        create_page()
        {
            if (!this.create_page_submittable || this.create_page_loading) {
                return;
            }

            this.create_page_loading = true;
            create_page(this.create_page_payload).then(() => {
                this.$notify_message("Page created successfully.");
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
