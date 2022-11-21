import { context } from "~/commons/context";
import { AUTHENTICATION_ENDPOINT } from "~/commons/requests/endpoints";
import { AuthenticationRequest } from "~/commons/requests/models/authentication-request";

export function authenticate(request: AuthenticationRequest) {
    return context.$axios.post(AUTHENTICATION_ENDPOINT, request);
}
