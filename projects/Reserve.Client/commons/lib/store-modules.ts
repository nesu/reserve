import IdentityModule from "~/store/modules/identity-module";
import NotificationModule from "~/store/modules/notification-module";
import LayoutModule from "~/commons/modules/visuals-module";
import VerticalNavigatorModule from "~/store/modules/vertical-navigator-module";

export default interface StoreModules
{
    readonly notifications: NotificationModule;
    readonly identity: IdentityModule;
    readonly layout: LayoutModule;
    readonly vertical_navigator: VerticalNavigatorModule;
}
