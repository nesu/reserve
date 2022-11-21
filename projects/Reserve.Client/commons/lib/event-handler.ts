import { EventType } from "~/commons/lib/event-type";

export default interface EventHandler {
    listen: (type: EventType, callback: Function) => void;
    trigger: (type: EventType, ...args: any[]) => void;
}
