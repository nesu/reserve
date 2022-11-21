import { Middleware } from "@nuxt/types";
import route_option from "~/commons/utility/route-option";

const assert_authenticated: Middleware = async (context) =>
{
    // Ignore routes where authentication is not required
    if (route_option(context.route, "authenticated", false)) {
        return;
    }

    const token_key = context.env["token_key"] ?? "id_token";
    const id_token = localStorage.getItem(token_key);

    const guest_only = route_option(context.route, "authenticated", "guest");

    // If token key is saved in local storage, fetch account data.
    let result = true;
    if (!!id_token)
    {
        context.$axios.setToken(id_token, "Bearer");

        try {
            await context.$modules.identity.fetch();
        }
        catch {
            localStorage.removeItem(token_key);
            result = false;
        }
    }

    const authenticated = context.$modules
        .identity.authenticated && result;

    if (authenticated)
    {
        // Redirect from guest pages to homepage of authenticated users.
        if (guest_only) {
            const redirect_to = context.env["authenticated_homepage"] ?? "/";
            context.redirect(redirect_to);
        }
    }
    else
    {
        if (!guest_only) {
            const redirect_to = context.env["login_page"] ?? "/login";
            context.redirect(redirect_to);
        }
    }
};

// noinspection JSUnusedGlobalSymbols
export default assert_authenticated;
