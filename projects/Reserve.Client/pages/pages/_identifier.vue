<template>
    <contents>
        <v-container
            v-if="!page">
            <v-row
                align="center"
                justify="center">
                <v-col
                    class="d-flex justify-center"
                    cols="12">
                    <v-progress-circular
                        indeterminate
                        size="56"/>
                </v-col>
            </v-row>
        </v-container>
        <v-container
            v-else>
            <v-card
                outlined>
                <v-toolbar
                    flat tile
                    height="56">
                    <v-toolbar-title class="headline">
                        {{ page.title }}
                    </v-toolbar-title>
                </v-toolbar>
                <v-divider class="mx-4"/>
                <v-card-text>
                    <vue-markdown
                        :source="page.contents"
                        class="markdown"/>
                </v-card-text>
            </v-card>
        </v-container>
    </contents>
</template>
<script lang="ts">

    import VueMarkdown from "vue-markdown";
    import { Component, mixins } from "~/commons";
    import PageBase from "~/commons/mixins/page-base";
    import { get_page_by_identifier } from "~/commons/requests/layout";
    import Page from "~/commons/resources/page";

    @Component({
        authenticated: false,
        components: {
            "vue-markdown": VueMarkdown
        },
        async asyncData()
        {

        }
    })
    export default class DynamicPage extends mixins(PageBase)
    {
        page: Page | null = null;

        mounted()
        {
            get_page_by_identifier(this.$route.params.identifier).then((page) => {
                this.page = page;
            })
            .catch(() => {
                this.$router.replace({ path: "/" });
            });
        }

        head()
        {
            const title = this.page?.title ?? "Loading";
            return {
                title
            }
        }
    }

</script>
