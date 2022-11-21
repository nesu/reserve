export default interface ChangePasswordRequest
{
    current_password: string;
    new_password: string;
    confirmed_new_password: string;
}
