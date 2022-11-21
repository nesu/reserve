<template>
    <contents>
        <v-btn
            v-for="element in element_array"
            v-bind="is_mobile ? {} : element.properties"

            :to="element.redirect_to"
            :key="element.title"
            :icon="is_mobile"

            class="mx-2"
            color="primary"
            nuxt
        >
            <contents v-if="is_mobile">
                <v-icon>{{ element.mdi_icon }}</v-icon>
            </contents>
            <contents v-else>
                {{ element.title }}
            </contents>
        </v-btn>
    </contents>
</template>
<script lang="ts">

    import { Component, mixins } from "~/commons";
    import { NavigatorBase } from "~/commons/mixins/navigator-base";
    import NavigatorElement from "~/commons/lib/navigator-element";

    @Component
    export default class HorizontalNavigatorComponent extends mixins(NavigatorBase)
    {
        get element_array(): NavigatorElement[]
        {
            const static_elements = this.$static_elements;
            const dynamic_elements = this.$modules.layout
                .elements.map(it => it.properties as NavigatorElement);

            const joint_elements = [
                ...static_elements,
                ...dynamic_elements
            ];

            const elements: NavigatorElement[] =joint_elements
                .filter((it: NavigatorElement) => this.$element_visibility_filter(it));

            return elements.sort(function (a, b) {
                return a.order - b.order;
            });
        }

        get is_mobile(): boolean {
            return this.$vuetify.breakpoint.mdAndDown;
        }
    }

</script>
