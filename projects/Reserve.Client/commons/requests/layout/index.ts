import { context } from "~/commons/context";
import NavigatorElement from "~/commons/lib/navigator-element";
import { LAYOUT_ENDPOINT } from "~/commons/requests/endpoints";
import CreateNavigatorElementRequest from "~/commons/requests/models/create-navigator-element-request";
import CreatePageRequest from "~/commons/requests/models/create-page-request";
import DeleteNavigatorElementRequest from "~/commons/requests/models/delete-navigator-element-request";
import DeletePageRequest from "~/commons/requests/models/delete-page-request";
import EditPageRequest from "~/commons/requests/models/edit-page-request";
import NavigatorElementResource from "~/commons/resources/navigator-element-resource";
import VisualElement from "~/commons/resources/visual-element";
import Page from "~/commons/resources/page";

export function get_visuals()
{
    return context.$axios
        .get<VisualElement[]>(`${LAYOUT_ENDPOINT}/visuals`)
        .then((response) => {
            return response.data;
        });
}

export function get_navigator_elements()
{
    return context.$axios
        .get<NavigatorElementResource[]>(`${LAYOUT_ENDPOINT}/navigator-elements`)
        .then((response) => {
            return response.data;
        });
}

export function get_navigator_element(identifier: number)
{
    return context.$axios
        .get<NavigatorElementResource>(`${LAYOUT_ENDPOINT}/navigator-element/${identifier}`)
        .then((response) => {
            return response.data;
        });
}

export function create_navigator_element(request: CreateNavigatorElementRequest)
{
    return context.$axios
        .post<NavigatorElement[]>(`${LAYOUT_ENDPOINT}/navigator-element`, request);
}

export function edit_navigator_element(request: CreateNavigatorElementRequest)
{
    return context.$axios
        .post<NavigatorElement[]>(`${LAYOUT_ENDPOINT}/edit-navigator-element`, request);
}

export function delete_navigator_element(request: DeleteNavigatorElementRequest)
{
    return context.$axios
        .post(`${LAYOUT_ENDPOINT}/delete-navigator-element`, request);
}

export function get_page_by_identifier(identifier: string)
{
    return context.$axios
        .get<Page>(`${LAYOUT_ENDPOINT}/page/${identifier}`)
        .then((response) => {
            return response.data;
        });
}

export function get_created_pages()
{
    return context.$axios
        .get<Page[]>(`${LAYOUT_ENDPOINT}/pages`)
        .then((response) => {
            return response.data;
        });
}

export function create_page(request: CreatePageRequest)
{
    return context.$axios
        .post<Page>(`${LAYOUT_ENDPOINT}/create-page`, request)
        .then((response) => {
            return response.data;
        });
}

export function edit_page(request: EditPageRequest)
{
    return context.$axios
        .post<Page>(`${LAYOUT_ENDPOINT}/edit-page`, request)
        .then((response) => {
            return response.data;
        });
}

export function delete_page(request: DeletePageRequest)
{
    return context.$axios
        .post<Page>(`${LAYOUT_ENDPOINT}/delete-page`, request)
        .then((response) => {
            return response.data;
        });
}
