<script lang="ts">
	import { page } from "$app/state";
	import { HomeIcon, PlusIcon, UserIcon } from "lucide-svelte";
</script>

<style>
    .button-base {
        @apply rounded-3xl p-4 m-2 w-fit;
    }

    .button-hover {
        @apply hover:bg-secondary ;
    }
</style>

<aside class="bg-primary text-white w-64 py-[1.5px]">

	<nav class="flex flex-col gap-y-0">
		<div class="p-4">
			<span class="text-xl font-bold font-michroma">Foodpanel</span>
		</div>

		<!-- Divider -->
		<div class="my-1">
			<div class="border-t border-white/100 mx-2 "></div>
		</div>

		<!-- Home -->
		<a class="flex items-center button-base button-hover" class:bg-secondary={page.url.pathname === "/"} href="/">
			<HomeIcon />
			<span class="ml-4">Home</span>
		</a>

		<!-- Profile -->
		<a class="flex items-center button-base button-hover" class:bg-secondary={page.url.pathname === "/@userhandle"}
		   href="/@userhandle">
			<UserIcon />
			<span class="ml-4">Profile</span>
		</a>

		<!-- Divider -->
		<div class="my-1">
			<div class="border-t border-white/100 mx-2 "></div>
		</div>

		<!-- Create Post -->
		<a class="flex items-center button-base bg-secondary" href="/createPost">
			<PlusIcon />
			<span class="ml-4">Create new Post</span>
		</a>

	</nav>
</aside>

