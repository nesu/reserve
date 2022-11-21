export interface CreateServiceRequest
{
    label: string;
    description: string | null;
    price: number;
    duration: number;
    category_id: number;
}
