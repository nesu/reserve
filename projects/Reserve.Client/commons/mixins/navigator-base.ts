import { Component, mixins } from "~/commons";
import NavigatorElement from "~/commons/lib/navigator-element";
import ComponentBase from "~/commons/mixins/component-base";

@Component
export class NavigatorBase extends mixins(ComponentBase)
{
    $element_visibility_filter(element: NavigatorElement, evaluate_mobile: boolean = true): boolean
    {
        let visible = true;

        if (this.$authenticated)
        {
            if (element.guest_only) {
                visible = false;
            }

            if (element.authenticated_only && !!element.permitted_roles) {
                visible = element.permitted_roles.includes(this.$account!.role);
            }
        }
        else
        {
            if (element.authenticated_only) {
                visible = false;
            }
        }

        if (visible && evaluate_mobile && this.$is_mobile) {
            // For mobile view include only those elements that has icons.
            visible = element.mobile_view;
        }

        return visible;
    }

    get $static_elements(): NavigatorElement[]
    {
        return [{
            order: 0,
            title: "Home",
            mdi_icon: "mdi-home",
            mobile_view: true,
            redirect_to: this.$routes.Home,
            guest_only: true,
            properties: {
                text: true
            }
        }, {
            order: 10,
            title: "Services",
            mdi_icon: "mdi-star",
            mobile_view: false,
            redirect_to: this.$routes.Services,
            properties: {
                text: true
            }
        }, {
            order: 20,
            title: "Reservations",
            mdi_icon: "mdi-clipboard-list",
            mobile_view: false,
            authenticated_only: true,
            redirect_to: this.$routes.Reservations,
            properties: {
                text: true
            }
        }, {
            order: 30,
            title: "Log In",
            mdi_icon: "mdi-account",
            mobile_view: true,
            guest_only: true,
            redirect_to: this.$routes.LogIn,
            properties: {
                text: true
            }
        }, {
            order: 40,
            title: "Sign Up",
            mdi_icon: "mdi-account-check",
            mobile_view: false,
            guest_only: true,
            redirect_to: this.$routes.SignUp,
            properties: {
                outlined: true
            }
        }];
    }
}
