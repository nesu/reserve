<template>
    <v-app light>
        <v-navigation-drawer
            floating
            v-model="navigator"
            class="px-4"
            color="white"
            width="420"
            app>
            <v-toolbar
                flat tile
                color="transparent">
                <v-toolbar-title>
                    <nuxt-link nuxt to="/">Reserve.</nuxt-link>
                </v-toolbar-title>
                <v-spacer/>
                <v-btn
                    @click.stop="navigator = !navigator"
                    class="hidden-md-and-up"
                    small icon>
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>
            <v-divider/>
            <v-list
                class="mt-4"
                nav>
                <v-list-item
                    active-class="layout-navigator-active-item"
                    exact nuxt to="/control-panel">
                    <v-list-item-icon>
                        <v-icon>mdi-view-dashboard</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>
                            Calendar
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item
                    v-authorized="$roles.Administrator"
                    active-class="layout-navigator-active-item"
                    exact nuxt to="/control-panel/pages">
                    <v-list-item-icon>
                        <v-icon>mdi-file-document</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>
                            Pages
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item
                    v-authorized="$roles.Administrator"
                    active-class="layout-navigator-active-item"
                    exact nuxt to="/control-panel/navigation">
                    <v-list-item-icon>
                        <v-icon>mdi-page-layout-header</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>
                            Navigation
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item
                    active-class="layout-navigator-active-item"
                    exact nuxt to="/control-panel/services">
                    <v-list-item-icon>
                        <v-icon>mdi-star</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>
                            Services
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item
                    v-authorized="$roles.Administrator"
                    active-class="layout-navigator-active-item"
                    exact nuxt to="/control-panel/categories">
                    <v-list-item-icon>
                        <v-icon>mdi-briefcase</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>
                            Service categories
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-item
                    active-class="layout-navigator-active-item"
                    exact nuxt to="/control-panel/locations">
                    <v-list-item-icon>
                        <v-icon>mdi-map-marker</v-icon>
                    </v-list-item-icon>
                    <v-list-item-content>
                        <v-list-item-title>
                            Locations
                        </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
                <v-list-group>
                    <template v-slot:activator>
                        <v-list-item-icon>
                            <v-icon>mdi-account</v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                Accounts
                            </v-list-item-title>
                        </v-list-item-content>
                    </template>
                    <v-list-item-group>
                        <v-list-item-group>
                            <v-list-item
                                active-class="layout-navigator-active-item"
                                exact nuxt to="/control-panel/accounts/employees">
                                <v-list-item-icon>
                                    <v-icon>mdi-account-hard-hat</v-icon>
                                </v-list-item-icon>
                                <v-list-item-content>
                                    <v-list-item-title>
                                        Employees
                                    </v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item
                                active-class="layout-navigator-active-item"
                                exact nuxt to="/control-panel/accounts/customers">
                                <v-list-item-icon>
                                    <v-icon>mdi-account-group</v-icon>
                                </v-list-item-icon>
                                <v-list-item-content>
                                    <v-list-item-title>
                                        Customers
                                    </v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list-item-group>
                    </v-list-item-group>
                </v-list-group>
            </v-list>
        </v-navigation-drawer>
        <v-content>
            <nuxt/>
        </v-content>
        <v-notifications/>
    </v-app>
</template>
<script lang="ts">

    import { EventType } from "~/commons/lib/event-type";
    import { Component, mixins } from "../commons";
    import ComponentBase from "../commons/mixins/component-base";

    @Component({
        components: {
            "v-notifications": require("~/components/notification").default
        }
    })
    export default class ManagementLayout extends mixins(ComponentBase)
    {
        navigator: boolean = true;

        created()
        {
            this.$events.listen(EventType.TRIGGER_MANAGEMENT_NAVIGATOR, () => {
                this.navigator = !this.navigator;
            });
        }
    }

</script>
<style lang="scss">
    @import "./assets/style/layout/management";
</style>

