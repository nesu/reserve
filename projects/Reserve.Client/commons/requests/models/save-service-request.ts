export interface SaveServiceRequest
{
    id: number;
    label: string;
    description: string | null;
    price: number;
    duration: number;
    category_id: number;
}
