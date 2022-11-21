import { Action, Module, Mutation, VuexModule } from "vuex-module-decorators";
import { get_authenticated_account } from "~/commons/requests/accounts";
import Account from "~/commons/resources/account";

@Module({
    name: "modules/identity-module",
    namespaced: true,
    stateFactory: true
})
export default class IdentityModule extends VuexModule
{
    private _account: Account | null = null;

    @Action({
        rawError: true
    })
    public async fetch()
    {
        return get_authenticated_account().then((account) => {
            this.context.commit("set_account", account);
        });
    }

    @Action({
        rawError: true
    })
    public dispose() {
        this.context.commit("set_account", null);
    }

    @Mutation
    set_account(account: Account | null) {
        this._account = account;
    }

    get authenticated(): boolean {
        return this._account !== null && !!this._account.email;
    }

    get account(): Account | null {
        return this._account;
    }
}
