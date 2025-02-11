import { apiBaseUrl, fallbackReturnUrl } from "$lib/config";
import { ApiProvider } from "$lib/provider/ApiProvider";
import { redirect, type ServerLoadEvent } from "@sveltejs/kit";
import type { LayoutServerLoad } from "./$types";
import type { UserOutDto } from "$lib/api/Api";

export const load: LayoutServerLoad = async (context) => {
	if (context.cookies.getAll().length === 0 && context.request.headers.get("Authorization") === null) {
		console.log("user has no cookies --> login");

		return {
			login: true,
			user: null as unknown as UserOutDto,
			currentUserId: null as unknown as string,
		};
	}

	const api = new ApiProvider(context.cookies.getAll()).api;

	let user;
	try {
		user = await api.auth.v1AuthMeList();
	} catch (e) {
		console.log("user auth request failed --> login", e);

		return {
			login: true,
			user: null as unknown as UserOutDto,
			currentUserId: null as unknown as string,
		};
	}

	if (!user?.data) {
		console.log("user is not authorized, redirecting to oauth login");
		throw getOAuthRedirect(context);
	}

	return {
		login: false,
		user: user.data,
		currentUser: user.data,
		currentUserId: user.data?.id,
	};
};

function getOAuthRedirect(context: ServerLoadEvent) {
	const loginUrl = new URL(`${apiBaseUrl}/api/v1/Auth/signin/bosch`);
	let currentUri = context.url ?? new URL(fallbackReturnUrl);

	if (currentUri.hostname === "undefined") {
		currentUri = new URL(fallbackReturnUrl);
	}

	if (currentUri.hostname === "localhost") {
		currentUri.protocol = "http";
	}

	loginUrl.searchParams.set("ReturnUrl", currentUri.toString());
	console.log("Redirecting to", loginUrl.toString());
	return redirect(302, loginUrl.toString());
}