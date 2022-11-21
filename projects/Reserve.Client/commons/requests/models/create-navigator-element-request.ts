import { RoleType } from "~/commons/resources/role-type";

export default interface CreateNavigatorElementRequest
{
    title: string;
    order: number;
    mdi_icon: string;
    redirect_to: string;
    mobile_view: boolean;
    guest_only: boolean;
    authenticated_only: boolean;
    properties: Record<string, any>;
    permitted_roles: RoleType[];
}
