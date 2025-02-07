import { browser } from "$app/environment";
import { env } from "$env/dynamic/public";
import { Api } from "$lib/api/Api";
import { apiBaseUrl, withCredentials } from "$lib/config";
import { DateTime } from "luxon";

const ignorePathsForCrashReport: string[] = [];
export class ApiProvider {
	public readonly api: Api<unknown>;

	public constructor(cookies?: { name: string; value: string }[]) {
		this.api = new Api({
			baseURL: apiBaseUrl,
		});
		this.api.instance.defaults.paramsSerializer = {
			indexes: null,
		};

		this.api.instance.interceptors.request.use((config) => {
			let url = config.url;
			let first = true;
			for (const key in config.params ?? []) {
				if (config.params[key] === undefined) continue;
				if (first) {
					url += "?";
					first = false;
				} else {
					url += "&";
				}
				url += `${key}=${config.params[key]}`;
			}

			console.log(`[${DateTime.now().toFormat("yyyy-MM-DD HH:mm:ss")}]`, "-->", config.method?.toUpperCase(), url);

			if (config.baseURL?.startsWith(apiBaseUrl) && !browser && env.PUBLIC_INTERNAL_API_BASE_URL) {
				const startUrl = config.baseURL;
				const targetUrl = env.PUBLIC_INTERNAL_API_BASE_URL;

				config.baseURL = targetUrl;
				console.log("Rewritten request from", startUrl, "to", targetUrl);

				// Setting the cookies is only required because the target domain could be different from the source
				if (cookies) {
					config.headers.set("Cookie", cookies.map((x) => `${x.name}=${x.value}`).join("; "));
				}
			}
			return config;
		});

		this.api.instance.interceptors.request.use((config) => {
			config.withCredentials = withCredentials;

			if (cookies) {
				config.headers.set("Cookie", cookies.map((x) => `${x.name}=${x.value}`).join("; "));
			}

			return config;
		});

		this.api.instance.interceptors.response.use(
			(response) => {
				let url = response.config.url;
				let first = true;
				for (const key in response.config.params ?? []) {
					if (response.config.params[key] === undefined) continue;
					if (first) {
						url += "?";
						first = false;
					} else {
						url += "&";
					}
					url += `${key}=${response.config.params[key]}`;
				}

				console.log(
					`[${DateTime.now().toFormat("YYYY-MM-DD HH:mm:ss")}]`,
					response.status,
					response.config.method?.toUpperCase(),
					url
				);

				return response;
			},
			(error) => {
				if (browser) {
					if (
						ignorePathsForCrashReport.some((x) => error?.request?.responseURL?.toLowerCase()?.includes(x))
					) {
						console.log(
							"Ignoring api exception. Skipping Crash Report due to ignorePathsForCrashReport list"
						);
					} else if (error?.code === "ERR_CANCELED") {
						console.log("Ignoring api exception. Skipping Crash Report due to CanceledError");
					}
				}
				return Promise.reject(error);
			}
		);
	}
}