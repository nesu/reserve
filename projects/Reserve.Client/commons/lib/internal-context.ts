import EventHandler from "~/commons/lib/event-handler";
import StoreModules from "~/commons/lib/store-modules";

export default interface InternalContext {
    $modules: StoreModules;
    $events: EventHandler;
}
