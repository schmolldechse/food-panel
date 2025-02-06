import type { Config } from "tailwindcss";

export default {
	content: ["./src/**/*.{html,js,svelte,ts}"],
	darkMode: "class",
	theme: {
		extend: {
			colors: {
				text: "#ffffff",
				background: "#0a0a0a",
				primary: "#404040",
				secondary: "#5000ff",
				accent: "#3eaded"
			},
			fontFamily: {
				michroma: ["Michroma", "sans-serif"]
			}
		}
	}
} as Config;
