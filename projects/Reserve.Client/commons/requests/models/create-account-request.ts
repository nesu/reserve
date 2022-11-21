import { RoleType } from "~/commons/resources/role-type";

export interface CreateAccountRequest
{
    email: string;
    given_name: string;
    family_name: string;
    phone_number: string;
    role: RoleType;
}
