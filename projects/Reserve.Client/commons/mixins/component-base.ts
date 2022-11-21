import { AxiosError } from "axios";
import { Vue, Component, mixins } from "~/commons";
import { AuthorizedDirective } from "~/commons/directives/authorized-directive";
import { GuestOnlyDirective } from "~/commons/directives/guest-only-directive";
import Account from "~/commons/resources/account";

@Component({
    directives: {
        authorized: AuthorizedDirective,
        "guest-only": GuestOnlyDirective
    },
    components: {
        "guest-only": require("~/components/guest-only").default,
        "authorized-only": require("~/components/authorized-only").default,
    }
})
export default class ComponentBase extends Vue
{
    get $authenticated() {
        return this.$modules.identity.authenticated;
    }

    get $account(): Account
    {
        const account = this.$modules.identity.account;
        if (!account) {
            throw new Error("Attempted to retrieve account when not logged in.");
        }

        return account!;
    }

    get $routes()
    {
        return {
            Home: "/",
            LogIn: "/login",
            SignUp: "/signup",
            Reservations: "/reservations",
            Services: "/services",
            Settings: "/settings",
            ControlPanel: "/control-panel",
            LayoutIndex: "/control-panel/layout",
        }
    }

    get $roles()
    {
        return {
            Client: 1000,
            Administrator: 1001,
            Employee: 1002,
        }
    }

    get $readable_role_array()
    {
        const roles: Record<string, number> = this.$roles;
        return Object.keys(roles).map((it) => {
            return { role: it, value: roles[it] }
        });
    }

    get $is_mobile(): boolean {
        return this.$vuetify.breakpoint.mdAndDown;
    }

    $notify_message(message: string) {
        this.$modules.notifications.message(message);
    }

    $notify_error(error: AxiosError | string)
    {
        if (typeof error === "string") {
            this.$modules.notifications.error(error);
            return;
        }

        const message = error.response?.data.error ?? "Unexpected error occurred. Please try again later.";
        this.$modules.notifications.error(message);
    }
}
