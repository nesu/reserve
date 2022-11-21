import { Module, Mutation, VuexModule } from "vuex-module-decorators";
import Notification from "~/commons/lib/notification";

@Module({
    name: "modules/notification-module",
    namespaced: true,
    stateFactory: true
})
export default class NotificationModule extends VuexModule
{
    private _notification: Notification = {
        active: false,
        contents: "",
        type: "message"
    };

    @Mutation
    error(contents: string)
    {
        this._notification = {
            active: true,
            contents,
            type: "error"
        }
    }

    @Mutation
    message(contents: string)
    {
        this._notification = {
            active: true,
            contents,
            type: "message"
        }
    }

    @Mutation
    reset()
    {
        this._notification = {
            active: false,
            contents: "V",
            type: "message"
        }
    }

    get model(): Notification {
        return this._notification;
    }
}
