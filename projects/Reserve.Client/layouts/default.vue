<template>
    <v-app light>
        <vertical-navigator/>
        <v-container
            class="pb-0">
            <div class="d-flex flex-column mt-md-12">
                <v-app-bar
                    color="transparent"
                    elevation="0">
                    <v-app-bar-nav-icon
                        @click.stop="$modules.vertical_navigator.toggle()"
                        v-if="mobile_view"/>
                    <v-toolbar-title>
                        Reserve.
                    </v-toolbar-title>
                    <v-spacer/>
                    <horizontal-navigator/>
                    <authorized-only>
                        <v-menu
                            v-model="account_menu"
                            nudge-width="256"
                            nudge-left="256"
                            offset-y>
                            <template v-slot:activator="{ on }">
                                <!-- @TODO: Refactor -->
                                <v-btn
                                    icon tile
                                    width="36" height="36"
                                    color="primary"
                                    :rounded="false"
                                    v-on="on">
                                    <v-icon>mdi-dots-horizontal</v-icon>
                                </v-btn>
                            </template>
                            <v-list>
                                <v-list-item
                                    nuxt :to="$routes.Settings">
                                    <v-list-item-icon>
                                        <v-icon>mdi-cog</v-icon>
                                    </v-list-item-icon>
                                    <v-list-item-content>
                                        <v-list-item-title>
                                            Settings
                                        </v-list-item-title>
                                    </v-list-item-content>
                                </v-list-item>
                                <contents
                                    v-authorized="[$roles.Administrator, $roles.Employee]">
                                    <v-divider/>
                                    <v-list-item
                                        exact
                                        nuxt :to="$routes.ControlPanel">
                                        <v-list-item-icon>
                                            <v-icon>mdi-briefcase-edit</v-icon>
                                        </v-list-item-icon>
                                        <v-list-item-content>
                                            <v-list-item-title>
                                                Control panel
                                            </v-list-item-title>
                                        </v-list-item-content>
                                    </v-list-item>
                                </contents>
                                <v-divider/>
                                <v-list-item
                                    @click.stop="logout">
                                    <v-list-item-icon
                                        class="red--text">
                                        <v-icon>mdi-logout</v-icon>
                                    </v-list-item-icon>
                                    <v-list-item-content>
                                        <v-list-item-title
                                            class="red--text">
                                            Logout
                                        </v-list-item-title>
                                    </v-list-item-content>
                                </v-list-item>
                            </v-list>
                        </v-menu>
                    </authorized-only>
                </v-app-bar>
            </div>
            <v-divider/>
        </v-container>
        <v-content>
            <nuxt/>
        </v-content>
        <v-notifications/>
    </v-app>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import ComponentBase from "~/commons/mixins/component-base";

    @Component({
        components: {
            "horizontal-navigator": require("~/components/horizontal-navigator").default,
            "vertical-navigator": require("~/components/vertical-navigator").default,
            "v-notifications": require("~/components/notification").default
        }
    })
    export default class DefaultLayout extends mixins(ComponentBase)
    {
        account_menu: boolean = false;

        get mobile_view(): boolean {
            return this.$vuetify.breakpoint.mdAndDown;
        }

        logout()
        {
            const token_key = process.env.token_key ?? "id_token";

            localStorage.removeItem(token_key);
            this.$modules.identity.dispose();

            this.$router.replace({ path: "/" });
        }
    }

</script>
<style lang="scss">
    @import "./assets/style/layout/default";
</style>
