import { Plugin, Context } from "@nuxt/types";
import { initialize_context } from "~/commons/context";

const accessor: Plugin = (ctx: Context) => initialize_context(ctx);

// noinspection JSUnusedGlobalSymbols
export default accessor;
