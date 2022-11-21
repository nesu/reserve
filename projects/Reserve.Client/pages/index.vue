<template>
    <contents>
        <re-hero-banner/>
        <v-container>
            <v-row>
                <v-col v-for="n in 3" :key="n" cols="12" md="4">
                    <v-card tile>
                        <v-card-title>
                            Pavyzdinis informacinis elementas
                        </v-card-title>
                        <v-divider/>
                        <v-card-text>
                            <p>
                                Pateikiama informacija ar svarbūs pranešimai
                            </p>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </contents>
</template>

<script lang="ts">

    import { Component, mixins } from "~/commons";
    import PageBase from "~/commons/mixins/page-base";

    @Component({
        authenticated: "guest",
        components: {
            "re-hero-banner": require("~/components/hero-banner.vue").default
        }
    })
    export default class IndexPage extends mixins(PageBase)
    {

    }

</script>
