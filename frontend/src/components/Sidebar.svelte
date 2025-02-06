<script lang="ts">
	import { page } from '$app/state';
	import { HomeIcon, PlusIcon, UserIcon } from 'lucide-svelte';
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
		<a href="/" class="flex items-center button-base button-hover" class:bg-secondary={page.url.pathname === "/"}>
			<HomeIcon />
			<span class="ml-4">Home</span>
		</a>

		<!-- Profile -->
		<a href="/@userhandle" class="flex items-center button-base button-hover"
			 class:bg-secondary={page.url.pathname === "/@userhandle"}>
			<UserIcon />
			<span class="ml-4">Profile</span>
		</a>

		<!-- Divider -->
		<div class="my-1">
			<div class="border-t border-white/100 mx-2 "></div>
		</div>

		<!-- Create Post -->
		<button class="flex items-center button-base bg-secondary">
			<PlusIcon />
			<span class="ml-4">Create new Post</span>
		</button>

	</nav>
</aside>

