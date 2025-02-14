<script lang="ts">
	import { page } from "$app/state";
	import { HomeIcon, PlusIcon, UserIcon } from "lucide-svelte";
	import { getContext } from "svelte";
	import type { LayoutData } from "../../.svelte-kit/types/src/routes/$types";
	import { apiBaseUrl } from "$lib/config";

	const context: LayoutData = getContext("userData");

	const returnUrl = page.url.searchParams.get("returnUrl") ?? page.url;

	function getLoginUrl(): string {
		const loginUrl = new URL(`${apiBaseUrl}/api/v1/Auth/signin/bosch`);

		let currentUri = new URL(page.url);
		currentUri.pathname = "/";
		currentUri.search = "";

		try {
			currentUri = new URL(returnUrl);
		} catch (e) {
			console.error("Failed to parse returnUrl on login");
		}

		if (currentUri.hostname === "undefined") {
			currentUri = new URL(`${page.url.protocol}//${page.url.host}`);
		}

		if (currentUri.hostname === "localhost") {
			currentUri.protocol = "http";
		}

		loginUrl.searchParams.set("ReturnUrl", currentUri.toString());

		return loginUrl.toString();
	}
</script>

<aside class="w-64 bg-primary py-[1.5px] text-white">
	<nav class="flex flex-col gap-y-0">
		<div class="p-4">
			<a href="/" class="font-michroma text-xl font-bold">Foodpanel</a>
		</div>

		<!-- Divider -->
		<div class="my-1">
			<div class="mx-2 border-t border-white/100"></div>
		</div>

		<!-- Home -->
		<a class="button-base button-hover flex items-center" class:bg-secondary={page.url.pathname === "/"} href="/">
			<HomeIcon />
			<span class="ml-4">Home</span>
		</a>

		<!-- Profile -->
		<a
			class="button-base button-hover flex items-center"
			class:bg-secondary={page.url.pathname === "/@" + context.user?.userHandle}
			href={context.login ? getLoginUrl() : "/@" + context.user.userHandle}
		>
			<UserIcon />
			{#if context.login}
				<span class="ml-4">Log in</span>
			{:else}
				<span class="ml-4">Profile</span>
			{/if}
		</a>

		<!-- Divider -->
		<div class="my-1">
			<div class="mx-2 border-t border-white/100"></div>
		</div>

		<!-- Create Post -->
		<a class="button-base flex items-center bg-secondary" href="/createPost">
			<PlusIcon />
			<span class="ml-4">Create new Post</span>
		</a>
	</nav>
</aside>

<style>
    .button-base {
        @apply m-2 w-fit rounded-3xl p-4;
    }

    .button-hover {
        @apply hover:bg-secondary;
    }
</style>
