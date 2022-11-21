<template>
    <contents>
        <default-toolbar>
            <template v-slot:toolbar-title>
                Pages
            </template>
        </default-toolbar>
        <v-container
            class="management-layout pt-0">
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
                                        Edit page
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
                                        v-model="edit_page_submittable">
                                        <v-text-field
                                            outlined
                                            label="Page title"
                                            :rules="validators.required"
                                            v-model="edit_page_payload.title"/>
                                        <v-text-field
                                            outlined
                                            label="Unique page identifier"
                                            hint="Used in URL: /pages/<identifier>"
                                            :rules="validators.required"
                                            v-model="edit_page_payload.identifier"/>
                                        <v-textarea
                                            v-model="edit_page_payload.contents"
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
                                        :disabled="!edit_page_submittable"
                                        :loading="edit_page_loading"
                                        @click.stop="edit_page">
                                        Save changes
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
                                            {{ edit_page_payload.title ? edit_page_payload.title : "Page title unspecified" }}
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
                                            :source="edit_page_payload.contents"/>
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
                                            :disabled="!edit_page_submittable"
                                            :loading="edit_page_loading"
                                            @click.stop="edit_page">
                                            Save changes
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
    </contents>
</template>
<script lang="ts">

    import VueMarkdown from 'vue-markdown'
    import { AxiosError } from "axios";
    import { Component, mixins } from "~/commons";
    import InputValidators from "~/commons/mixins/input-validators";
    import ManagementBase from "~/commons/mixins/management-base";
    import PageBase from "~/commons/mixins/page-base";
    import { edit_page, get_page_by_identifier } from "~/commons/requests/layout";
    import EditPageRequest from "~/commons/requests/models/edit-page-request";

    @Component({
        components: {
            "vue-markdown": VueMarkdown
        }
    })
    export default class EditPage extends mixins(ManagementBase, InputValidators)
    {
        preview: boolean = false;

        edit_page_submittable: boolean = false;
        edit_page_loading: boolean = false;
        edit_page_payload: EditPageRequest = {
            id: 0,
            title: "",
            identifier: "",
            contents: ""
        }

        mounted()
        {
            get_page_by_identifier(this.$route.params.identifier).then((page) => {
                this.edit_page_payload = page;
            })
            .catch(() => {
                this.$router.replace({ path: "/control-panel/create-page" });
            });
        }

        edit_page()
        {
            if (!this.edit_page_submittable || this.edit_page_loading) {
                return;
            }

            this.edit_page_loading = true;
            edit_page(this.edit_page_payload).then(() => {
                this.$notify_message("Changes saved successfully.");
            })
            .catch((error: AxiosError) =>
            {
                 if(error.response?.status === 409) {
                     this.$notify_error(`Page with identifier '${this.edit_page_payload.identifier}' already exists.`);
                 }
                 else {
                     this.$notify_error(error);
                 }
            })
            .finally(() => {
                this.edit_page_loading = false;
            });
        }
    }

</script>
