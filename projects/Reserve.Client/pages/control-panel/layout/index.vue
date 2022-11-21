<template>
    <v-container class="pt-0">
        <v-toolbar
            color="transparent"
            elevation="0">
            <v-spacer/>
            <contents>
                <v-btn
                    color="primary"
                    text>
                    Create new page
                </v-btn>
            </contents>
        </v-toolbar>
    </v-container>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import PageBase from "~/commons/mixins/page-base";
    import {
        delete_navigator_element,
        delete_page,
        get_created_pages,
        get_navigator_elements
    } from "~/commons/requests/layout";

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
    export default class PageManagement extends mixins(PageBase)
    {

    }

</script>
