import { context } from "~/commons/context";
import { CompactEmployee } from "~/commons/resources/compact-employee";
import { Service } from "~/commons/resources/service";

const ENDPOINT = "/api/employees"

export function get_compact_employees(
    location_id: number
)
{
    return context.$axios
        .get<CompactEmployee[]>(`${ENDPOINT}/${location_id}`)
        .then((response) => {
            return response.data;
        });
}

export function get_service_collection_by_employee(
    employee_id: number
)
{
    return context.$axios
        .get<Service[]>(`${ENDPOINT}/services/${employee_id}`)
        .then((response) => {
            return response.data;
        });
}
