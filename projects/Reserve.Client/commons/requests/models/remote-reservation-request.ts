export interface RemoteReservationRequest
{
    account_id: number;
    services: number[];
    employee_id: number;
    datetime: string;
}
