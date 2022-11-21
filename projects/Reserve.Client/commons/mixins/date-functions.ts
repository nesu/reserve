import { Component, Vue } from "nuxt-property-decorator";
import { dateFns } from "@nuxtjs/date-fns/types/date-fns";

declare type DateFormat = "yyyy-MM-dd"
    | "yyyy-MM"
    | "yyyy-MM-dd HH:mm"
    | "yyyy-MM-dd HH:mm:ss"
    | "HH:mm"
    | "HH:mm:ss"
    | "yyyy"
    | "MM-dd"

@Component
export default class DateFunctions extends Vue
{
    get $dateutil(): dateFns {
        return this.$nuxt.$dateFns;
    }

    $format_date(date: string | Date, format: DateFormat = "yyyy-MM-dd"): string
    {
        const v: Date = typeof date === "string" ? this.$parse_date(date) : date;
        return this.$dateutil.format(v, format);
    }

    $parse_date(date: string): Date {
        return this.$dateutil.parseISO(date);
    }
}
