import { context } from "~/commons/context";
import { CreateServiceAssignment } from "~/commons/requests/models/create-service-assignment";
import { CreateServiceRequest } from "~/commons/requests/models/create-service-request";
import { DeleteServiceAssignment } from "~/commons/requests/models/delete-service-assignment";
import { DeleteServiceRequest } from "~/commons/requests/models/delete-service-request";
import { GetEventRequest } from "~/commons/requests/models/get-event-request";
import { RemoteReservationRequest } from "~/commons/requests/models/remote-reservation-request";
import { SaveServiceRequest } from "~/commons/requests/models/save-service-request";
import { CalendarEvent } from "~/commons/resources/CalendarEvent";
import { Reservation } from "~/commons/resources/reservation";
import { Service } from "~/commons/resources/service";
import { ServiceAssignee } from "~/commons/resources/service-assignee";

const ENDPOINT = "/api/management/services"

export function get_services()
{
    return context.$axios
        .get<Service[]>(`${ENDPOINT}`)
        .then((response) => {
            return response.data;
        });
}

export function get_events(
    request: GetEventRequest
)
{
    return context.$axios
        .post<CalendarEvent[]>(`${ENDPOINT}/events`, request)
        .then((response) => {
            return response.data;
        });
}

export function create_service(
    request: CreateServiceRequest)
{
    return context.$axios
        .post<Service>(`${ENDPOINT}/create-service`, request)
        .then((response) => {
            return response.data;
        });
}

export function save_service(
    request: SaveServiceRequest)
{
    return context.$axios
        .post<Service>(`${ENDPOINT}/save-service`, request)
        .then((response) => {
            return response.data;
        });
}

export function delete_service(
    request: DeleteServiceRequest)
{
    return context.$axios
        .post(`${ENDPOINT}/delete-service`, request)
        .then((response) => {
            return response.data;
        });
}

export function get_service_assignments(
    service_id: number)
{
    return context.$axios
        .get<ServiceAssignee[]>(`${ENDPOINT}/assignments/${service_id}`)
        .then((response) => {
            return response.data;
        });
}

export function create_service_assignment(
    request: CreateServiceAssignment)
{
    return context.$axios
        .post<ServiceAssignee>(`${ENDPOINT}/create-assignment`, request)
        .then((response) => {
            return response.data;
        });
}

export function delete_service_assignment(
    request: DeleteServiceAssignment)
{
    return context.$axios
        .post(`${ENDPOINT}/delete-assignment`, request)
        .then((response) => {
            return response.data;
        });
}

export function create_remote_reservation(
    request: RemoteReservationRequest)
{
    return context.$axios
        .post<Reservation>(`${ENDPOINT}/create-reservation`, request)
        .then((response) => {
            return response.data;
        });
}
