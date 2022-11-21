import { Component, mixins } from "~/commons";
import ComponentBase from "~/commons/mixins/component-base";

@Component({
    layout: "management",
    components: {
        "default-toolbar": require("~/components/management/default-toolbar").default
    }
})
export default class ManagementBase extends mixins(ComponentBase)
{

}
