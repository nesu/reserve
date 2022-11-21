
// noinspection JSUnusedGlobalSymbols
export default {
    mode: "spa",
    env: {
        token_key: "id_token",
        authenticated_homepage: "/reservations",
        login_path: "/login"
    },
    head: {
        titleTemplate: "%s - Reserve.",
        title: process.env.npm_package_name || "",
        meta: [
            { charset: "utf-8" },
            { name: "viewport", content: "width=device-width, initial-scale=1" },
            { hid: "description", name: "description", content: process.env.npm_package_description || "" }
        ],
        link: [
            { rel: "stylesheet", href: "https://fonts.googleapis.com/css?family=Raleway:300,100,200,400" },
        ]
    },
    loading: { color: "#fff" },
    css: [
        "~/assets/style/global.scss"
    ],
    router: {
        middleware: [
            "assert-authenticated"
        ]
    },
    plugins: [
        {
            ssr: false,
            mode: 'client',
            src: "~/plugins/context-accessor.ts",
        },
        {
            ssr: false,
            mode: 'client',
            src: "~/plugins/store-registry.ts",
        },
        {
            ssr: false,
            mode: 'client',
            src: "~/plugins/layout-configurations.ts",
        },
        {
            ssr: false,
            mode: 'client',
            src: "~/plugins/event-handler.ts",
        }
    ],
    buildModules: [
        "@nuxt/typescript-build",
        "@nuxtjs/vuetify",
        "@nuxtjs/date-fns"
    ],
    dateFns: {
        methods: [
            "format",
            "parse",
            "parseISO",
            "isValid",
            "isFuture",
            "isAfter",
            "addMonths",
            "addWeeks",
            "addDays",
            "addMinutes",
            "subWeeks",
            "subMonths",
            "subDays",
            "startOfMonth",
            "startOfWeek",
            "startOfDay",
            "formatDistance",
            "roundToNearestMinutes",
            "differenceInMinutes"
        ]
    },
    modules: [
        "@nuxtjs/axios",
    ],
    axios: {
        credentials: true,
        https: true,
        baseURL: "https://localhost:5001/",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        }
    },
    vue: {
        config: {
            ignoredElements: ["contents"]
        }
    },
    vuetify: require("./vuetify.options").default,
    build:
    {
        babel:
        {
            plugins: [
                [ "@babel/plugin-proposal-decorators", { legacy: true } ],
                [ "@babel/plugin-proposal-class-properties", { loose: true } ]
            ]
        }
    }
}
