import { Context } from "@nuxt/types";

let context: Context;

function initialize_context(ctx: Context) {
    context = ctx;
}

export {
    context,
    initialize_context
};
