import InternalContext from "~/commons/lib/internal-context";

declare module "@nuxt/types" {
    interface Context extends InternalContext {}
    interface NuxtAppOptions extends InternalContext {}
}

declare module "vue/types/vue" {
    interface Vue extends InternalContext {}
}

declare module "vuex/types/index" {
    interface Vue extends InternalContext {}
}

declare module 'vue/types/options'
{
    interface ComponentOptions<V extends Vue> {
        authenticated?: "guest" | boolean
    }
}
