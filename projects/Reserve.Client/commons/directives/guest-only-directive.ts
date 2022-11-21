import { DirectiveOptions, VNode } from "vue";
import { DirectiveBinding } from 'vue/types/options';

const directive: DirectiveOptions =
{
    inserted(el: HTMLElement, binding: DirectiveBinding, node: VNode)
    {
        if (!node.context || !node.elm || !node.elm.parentElement) {
            return;
        }

        const authenticated = node.context.$modules.identity.authenticated;

        if (authenticated) {
            node.elm.parentElement.removeChild(node.elm);
        }
    }
};

export {
    directive as GuestOnlyDirective
};
