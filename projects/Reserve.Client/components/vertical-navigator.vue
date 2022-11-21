<template>
    <contents>
        <v-navigation-drawer
            floating
            v-model="navigator"
            disable-resize-watcher
            width="420" fixed>
            <v-toolbar
                flat
                class="px-3"
                height="76">
                <v-toolbar-title>
                    Reserve.
                </v-toolbar-title>
                <v-spacer/>
                <v-btn
                    @click.stop="navigator = !navigator" icon>
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>
            <v-divider/>
            <v-list nav>
                <v-list-item-group>
                    <v-list-item
                        v-for="element in navigator_elements"
                        :to="element.redirect_to" nuxt
                        :key="element.title">
                        <v-list-item-icon
                            v-if="!!element.mdi_icon">
                            <v-icon> {{ element.mdi_icon }}</v-icon>
                        </v-list-item-icon>
                        <v-list-item-content>
                            <v-list-item-title>
                                {{ element.title }}
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>
                </v-list-item-group>
            </v-list>
        </v-navigation-drawer>
    </contents>
</template>
<script lang="ts">

    import NavigatorElement from "~/commons/lib/navigator-element";
    import { Component, mixins } from "../commons";
    import { NavigatorBase } from "~/commons/mixins/navigator-base";

    @Component
    export default class VerticalNavigatorComponent extends mixins(NavigatorBase)
    {
        get navigator_elements(): NavigatorElement[]
        {
            const dynamic_elements = this.$modules.layout
                .elements.map(it => it.properties as NavigatorElement);

            const joint_elements = [
                ...this.$static_elements,
                ...dynamic_elements
            ];

            const elements: NavigatorElement[] = joint_elements
                .filter((it) => this.$element_visibility_filter(it, false));

            return elements.sort(function(a, b) {
                return a.order - b.order;
            });
        }

        get navigator(): boolean {
            return this.$modules.vertical_navigator.value;
        }

        set navigator(value: boolean) {
            this.$modules.vertical_navigator.set(value)
        }
    }

</script>
