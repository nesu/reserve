import Vue from "vue";
import { Plugin, Context } from "@nuxt/types";
import { EventType } from "~/commons/lib/event-type";

const event_handler: Plugin = (context: Context, inject) =>
{
    const event_bus = new Vue();

    context.$events =
    {
        listen(type: EventType, callback: Function) {
            event_bus.$on(type.toString(), callback);
        },
        trigger(type: EventType, ...args: any[]) {
            event_bus.$emit(type.toString(), args);
        }
    }

    inject("events", context.$events);
};

// noinspection JSUnusedGlobalSymbols
export default event_handler;
