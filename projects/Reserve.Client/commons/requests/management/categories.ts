import { context } from "~/commons/context";
import { CreateCategoryRequest } from "~/commons/requests/models/create-category-request";
import { DeleteCategoryRequest } from "~/commons/requests/models/delete-category-request";
import { ServiceCategory } from "~/commons/resources/service-category";

const ENDPOINT = "/api/management/categories"

export function get_base_categories()
{
    return context.$axios
        .get<ServiceCategory[]>(`${ENDPOINT}`)
        .then((response) => {
            return response.data;
        });
}

export function get_categories()
{
    return context.$axios
        .get<ServiceCategory[]>(`${ENDPOINT}/nested`)
        .then((response) => {
            return response.data;
        });
}

export function create_category(
    request: CreateCategoryRequest)
{
    return context.$axios
        .post<ServiceCategory>(`${ENDPOINT}/create-category`, request)
        .then((response) => {
            return response.data;
        });
}

export function get_category(
    category_id: number)
{
    return context.$axios
        .get<ServiceCategory>(`${ENDPOINT}/${category_id}`)
        .then((response) => {
            return response.data;
        });
}

export function save_category(
    category: ServiceCategory)
{
    return context.$axios
        .post<ServiceCategory>(`${ENDPOINT}/save-category`, category)
        .then((response) => {
            return response.data;
        });
}

export function delete_category(
    request: DeleteCategoryRequest)
{
    return context.$axios
        .post(`${ENDPOINT}/delete-category`, request)
        .then((response) => {
            return response.data;
        });
}
