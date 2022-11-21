import { RoleType } from "~/commons/resources/role-type";

export default interface NavigatorElement
{
    title: string;
    order: number;
    mdi_icon?: string;
    redirect_to: string;
    mobile_view: boolean;
    guest_only?: boolean;
    authenticated_only?: boolean;
    permitted_roles?: RoleType[];
    children?: NavigatorElement[];
    properties?: Record<string, any>;
}
