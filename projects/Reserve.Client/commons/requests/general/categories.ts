import { context } from "~/commons/context";
import { ServiceCategory } from "~/commons/resources/service-category";

const ENDPOINT = "/api/categories"

export function get_parent_category_collection()
{
    return context.$axios
        .get<ServiceCategory[]>(`${ENDPOINT}/root`)
        .then((response) => {
            return response.data;
        });
}
