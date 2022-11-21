import { Action, Module, Mutation, VuexModule } from "vuex-module-decorators";
import * as layout from "~/commons/requests/layout";
import VisualElement from "~/commons/resources/visual-element";
import VisualElementDictionary from "~/commons/resources/visual-element-dictionary";

@Module({
    name: "modules/layout-module",
    namespaced: true,
    stateFactory: true
})
export default class LayoutModule extends VuexModule
{
    private _elements: VisualElement[] = [];

    @Action({
        rawError: true
    })
    public async preload_visuals()
    {
        return layout.get_visuals().then((visuals) => {
            this.context.commit("set_elements", visuals);
        });
    }

    @Mutation
    set_elements(elements: VisualElement[]) {
        this._elements = elements;
    }

    get elements(): VisualElement[] {
        return this._elements;
    }
}
