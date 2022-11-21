import { context } from "~/commons/context";
import { Location } from "~/commons/resources/location";

const ENDPOINT = "/api/locations"

export function get_locations()
{
    return context.$axios
    .get<Location[]>(`${ENDPOINT}`)
    .then((response) => {
        return response.data;
    });
}
