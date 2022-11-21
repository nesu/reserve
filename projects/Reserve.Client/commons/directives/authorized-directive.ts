import { DirectiveOptions, VNode } from "vue";
import { DirectiveBinding } from 'vue/types/options';

const directive: DirectiveOptions =
{
    inserted(el: HTMLElement, binding: DirectiveBinding, node: VNode)
    {
        const context = node.context;

        if (!node || !node.elm || !node.elm.parentElement) {
            return;
        }

        if (!context) {
            return;
        }

        let visible = true;

        const parameter = binding.value;

        const authenticated = context.$modules.identity.authenticated;
        const account = context.$modules.identity.account;

        if (!authenticated || !account) {
            visible = false;
        }
        else if (Array.isArray(parameter) && !parameter.includes(account.role)) {
            visible = false;
        }
        else if(!Array.isArray(parameter) && !!parameter && parameter !== account.role) {
            visible = false;
        }

        if (!visible) {
            node.elm.parentElement.removeChild(node.elm);
        }
    }
};

export {
    directive as AuthorizedDirective
};
