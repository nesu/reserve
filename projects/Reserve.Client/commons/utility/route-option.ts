import { Route } from 'vue-router';

/**
 * Checks if specified option key and its' value exists on given route.
 * @param route
 * @param key
 * @param value
 */
export default function route_option(route: Route, key: string, value: any)
{
    return route.matched.some((m) => {
        return Object.values(m.components).some(
            (component: any) => component.options && component.options[key] === value
        );
    });
}
