import { Context } from "@nuxt/types";
import { NuxtAxiosInstance } from "@nuxtjs/axios";
import StoreModules from "~/commons/lib/store-modules";

export class BaseService
{
    protected context: Context;
    protected modules: StoreModules;
    protected axios: NuxtAxiosInstance;

    constructor(context: Context) {
        this.context = context;
        this.modules = context.$modules;
        this.axios = context.$axios;
    }
}
