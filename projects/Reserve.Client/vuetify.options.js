// noinspection JSUnusedGlobalSymbols
export default {
    treeShake: true,
    customVariables: [
        "~/assets/style/preset/overrides.scss"
    ],
    defaultAssets: {
        font: {
            family: "Raleway"
        }
    },
    theme: {
        dark: false,
        options: {
            customProperties: true,
        },
        themes: {
            light: {
                primary: "#356859",
                secondary: "#FD5523",
                accent: "#37966F",
                tertiary: "#B9E4C9",
            }
        }
    }
}
