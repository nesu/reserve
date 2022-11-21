import { context } from "~/commons/context";
import { ACCOUNT_ENDPOINT } from "~/commons/requests/endpoints";
import ChangePasswordRequest from "~/commons/requests/models/change-password-request";
import SavePersonalInformationRequest from "~/commons/requests/models/save-personal-information-request";
import Account from "~/commons/resources/account";

export function get_authenticated_account(): Promise<Account>
{
    return context.$axios
        .get<Account>(ACCOUNT_ENDPOINT)
        .then((response) => {
            return response.data;
        });
}

export function save_personal_information(request: SavePersonalInformationRequest): Promise<Account>
{
    return context.$axios
        .post<Account>(`${ACCOUNT_ENDPOINT}/personal-information`, request)
        .then((response) => {
            return response.data;
        });
}

export function change_password(request: ChangePasswordRequest)
{
    return context.$axios
        .post(`${ACCOUNT_ENDPOINT}/change-password`, request)
        .then((response) => {
            return response.data;
        });
}
