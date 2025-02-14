import { sveltekit } from "@sveltejs/kit/vite";
import { defineConfig } from "vite";

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		proxy: {
			"/img-proxy": {
				target: "http://localhost:9000",
				rewrite: (path) => path.replace(/^\/img-proxy/, '')
			}
		}
	}
});
