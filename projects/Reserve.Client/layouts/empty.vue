<template>
    <v-app light>
        <v-content>
            <nuxt/>
        </v-content>
        <v-notifications/>
    </v-app>
</template>
<script lang="ts">

    import { Component, Vue } from "../commons";

    @Component({
        components: {
            "v-notifications": require("~/components/notification").default
        }
    })
    export default class EmptyLayout extends Vue {}

</script>
