export default interface Notification
{
    active: boolean;
    contents: string;
    type: "error" | "message";
}
