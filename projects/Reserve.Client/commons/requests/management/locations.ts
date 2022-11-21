import { context } from "~/commons/context";
import { CreateLocationRequest } from "~/commons/requests/models/create-location-request";
import { DeleteLocationRequest } from "~/commons/requests/models/delete-location-request";
import { SaveLocationRequest } from "~/commons/requests/models/save-location-request";
import { Location } from "~/commons/resources/location";

const ENDPOINT = "/api/management/locations"

export function get_locations()
{
    return context.$axios
        .get<Location[]>(`${ENDPOINT}`)
        .then((response) => {
            return response.data;
        });
}

export function create_location(
    request: CreateLocationRequest
)
{
    return context.$axios
        .post<Location>(`${ENDPOINT}/create-location`, request)
        .then((response) => {
            return response.data;
        });
}

export function save_location(
    request: SaveLocationRequest
)
{
    return context.$axios
        .post<Location>(`${ENDPOINT}/save-location`, request)
        .then((response) => {
            return response.data;
        });
}

export function delete_location(
    request: DeleteLocationRequest
)
{
    return context.$axios
        .post(`${ENDPOINT}/delete-location`, request)
        .then((response) => {
            return response.data;
        });
}
