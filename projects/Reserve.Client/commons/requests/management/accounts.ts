import { context } from "~/commons/context";
import { AccountCollectionRequest } from "~/commons/requests/models/account-collection-request";
import { CreateAccountRequest } from "~/commons/requests/models/create-account-request";
import { ExactReservationRequest } from "~/commons/requests/models/exact-reservation-request";
import { SaveAccountRequest } from "~/commons/requests/models/save-account-request";
import Account from "~/commons/resources/account";
import { Reservation } from "~/commons/resources/reservation";

const ENDPOINT = "/api/management/accounts";

export function get_account(
    account_id: number)
{
    return context.$axios
        .get<Account>(`${ENDPOINT}/${account_id}`)
        .then((response) => {
            return response.data;
        });
}

export function get_account_collection(
    request: AccountCollectionRequest = { page_index: 1, page_size: 10 })
{
    return context.$axios
        .post<Account[]>(`${ENDPOINT}`, request)
        .then((response) => {
            return response.data;
        });
}

export function create_account(request: CreateAccountRequest)
{
    return context.$axios
        .post<Account>(`${ENDPOINT}/create-account`, request)
        .then((response) => {
            return response.data;
        });
}

export function save_account(request: SaveAccountRequest)
{
    return context.$axios
        .post<SaveAccountRequest>(`${ENDPOINT}/save-account`, request)
        .then((response) => {
            return response.data;
        });
}
