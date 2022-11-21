import { Plugin } from "@nuxt/types";
import { getModule } from "vuex-module-decorators";
import StoreModules from "~/commons/lib/store-modules";
import IdentityModule from "~/store/modules/identity-module";
import NotificationModule from "~/store/modules/notification-module";
import LayoutModule from "~/commons/modules/visuals-module";
import VerticalNavigatorModule from "~/store/modules/vertical-navigator-module";

const store_registry: Plugin = (context, inject) =>
{
    context.$modules = <StoreModules>
    {
        get notifications() {
            return getModule(NotificationModule, context.store);
        },

        get identity() {
            return getModule(IdentityModule, context.store);
        },

        get layout() {
            return getModule(LayoutModule, context.store);
        },

        get vertical_navigator() {
            return getModule(VerticalNavigatorModule, context.store);
        }
    };

    inject("modules", context.$modules);
};

// noinspection JSUnusedGlobalSymbols
export default store_registry;
