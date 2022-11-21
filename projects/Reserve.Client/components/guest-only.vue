<template functional>
    <contents v-if="!authenticated">
        <slot/>
    </contents>
</template>
<script lang="ts">

    import { Component, Vue } from "../commons";

    @Component
    export default class GuestOnlyComponent extends Vue
    {
        get authenticated() {
            return this.$modules.identity.authenticated;
        }
    }

</script>
