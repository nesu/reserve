import { RoleType } from "~/commons/resources/role-type";

export interface SaveAccountRequest
{
    id: number;
    email: string;
    given_name: string;
    family_name: string;
    phone_number: string;
    role: RoleType;
}
