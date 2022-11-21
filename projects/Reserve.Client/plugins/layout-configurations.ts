import { Plugin, Context } from "@nuxt/types";
import LayoutModule from "~/commons/modules/visuals-module";

const layout_configurations: Plugin = async (context: Context) => {
    context.store.registerModule("modules/layout-module", LayoutModule);
    await context.$modules.layout.preload_visuals();
}

// noinspection JSUnusedGlobalSymbols
export default layout_configurations;
