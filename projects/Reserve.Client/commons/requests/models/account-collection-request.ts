import { PageRequest } from "~/commons/requests/models/page-request";
import { RoleType } from "~/commons/resources/role-type";

export interface AccountCollectionRequest extends PageRequest
{
    role?: RoleType;
}
