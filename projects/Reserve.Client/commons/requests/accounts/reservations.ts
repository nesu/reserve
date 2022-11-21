import { context } from "~/commons/context";
import { CancelReservationRequest } from "~/commons/requests/models/cancel-reservation-request";
import { ResecheduleReservationRequest } from "~/commons/requests/models/resechedule-reservation-request";
import { ReservationRequest } from "~/commons/requests/models/reservation-request";
import { Reservation } from "~/commons/resources/reservation";

const ENDPOINT = "/api/account";

export function create_reservation(
    request: ReservationRequest)
{
    return context.$axios
        .post(`${ENDPOINT}/create-reservation`, request)
        .then((response) => {
            return response.data;
        });
}

export function get_reservations()
{
    return context.$axios
        .get<Reservation[]>(`${ENDPOINT}/reservations`)
        .then((response) => {
            return response.data;
        });
}

export function get_reservation(rid: number)
{
    return context.$axios
        .get<Reservation>(`${ENDPOINT}/reservation/${rid}`)
        .then((response) => {
            return response.data;
        });
}

export function reschedule_reservation(
    request: ResecheduleReservationRequest
)
{
    return context.$axios
        .post<Reservation>(`${ENDPOINT}/reschedule-reservation`, request)
        .then((response) => {
            return response.data;
        });
}

export function cancel_reservation(
    request: CancelReservationRequest
)
{
    return context.$axios
        .post(`${ENDPOINT}/cancel-reservation`, request)
        .then((response) => {
            return response.data
        });
}
