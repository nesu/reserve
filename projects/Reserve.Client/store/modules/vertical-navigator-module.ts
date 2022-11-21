import { Module, Mutation, VuexModule } from "vuex-module-decorators";

@Module({
    name: "modules/vertical-navigator-module",
    namespaced: true,
    stateFactory: true
})
export default class VerticalNavigatorModule extends VuexModule
{
    private _navigator: boolean = false;

    @Mutation
    set(value: boolean) {
        this._navigator = value;
    }

    @Mutation
    toggle() {
        this._navigator = !this._navigator;
    }

    get value(): boolean {
        return this._navigator;
    }
}
