import { context } from "~/commons/context";
import { REGISTRATION_ENDPOINT } from "~/commons/requests/endpoints";
import { RegistrationRequest } from "~/commons/requests/models/registration-request";

export function register_account(request: RegistrationRequest) {
    return context.$axios.post(REGISTRATION_ENDPOINT, request);
}
