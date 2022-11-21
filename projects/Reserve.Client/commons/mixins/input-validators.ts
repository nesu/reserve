import { isNil } from 'lodash-es';
import { Vue, Component } from "~/commons";
import { InputRule } from "~/commons/lib/input-rule";

@Component
export default class InputValidators extends Vue
{
    get validators(): InputValidator
    {
        const required = (v: any) => {
            return !isNil(v) && v.toString().length > 0 || "Field cannot be empty";
        };

        return {
            number: [
                required,
                (v: string | null) => !isNaN(Number(v)) || "Field can only contain numbers"
            ],
            required: [
                required,
            ],
            email:  [
                required,
                (v: any | null) => /.+@.+/.test(v) || "Incorrect email format"
            ],
            password: [
                required,
                (v: any | null) => !isNil(v) && v.length >= 8 || "Password must contain at least 8 symbols",
            ]
        }
    }
}

declare interface InputValidator
{
    number:     InputRule[];
    required:   InputRule[];
    email:      InputRule[];
    password:   InputRule[];
}
