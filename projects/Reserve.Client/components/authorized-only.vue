<template>
    <contents v-if="authorized">
        <slot/>
    </contents>
</template>
<script lang="ts">

    import { Component, Property, Vue } from "../commons";
    import { RoleType } from "~/commons/resources/role-type";

    @Component
    export default class AuthorizedOnlyComponent extends Vue
    {
        @Property({
            type: Array,
            default: () => [RoleType.Client, RoleType.Employee, RoleType.Administrator]
        })
        roles!: RoleType[];

        get authorized()
        {
            if (!this.$modules.identity.authenticated) {
                return false;
            }

            return this.roles.includes(this.$modules.identity.account!.role);
        }
    }

</script>
