<script lang="ts">
	import { page } from "$app/state";
	import { HomeIcon, PlusIcon, UserIcon } from "lucide-svelte";
</script>

<aside class="w-64 bg-primary py-[1.5px] text-white">
	<nav class="flex flex-col gap-y-0">
		<div class="p-4">
			<span class="font-michroma text-xl font-bold">Foodpanel</span>
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
			class:bg-secondary={page.url.pathname === "/@userhandle"}
			href="/@userhandle"
		>
			<UserIcon />
			<span class="ml-4">Profile</span>
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
