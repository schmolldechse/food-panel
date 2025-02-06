<script>
	import { Sidebar, SidebarGroup, SidebarItem, SidebarWrapper } from 'flowbite-svelte';
	import { ArrowRightToBracketOutline, EditOutline, EditSolid, HomeSolid, UserSolid } from 'flowbite-svelte-icons';
	import { page } from '$app/state';

	let spanClass = 'flex-1 ms-3 whitespace-nowrap';
	let activeUrl = $derived(page.url.pathname);

	let signedIn = $state(true);
</script>

<Sidebar {activeUrl}>
	<SidebarWrapper>
		<SidebarGroup>
			<SidebarItem label="Home" href="/">
				<svelte:fragment slot="icon">
					<HomeSolid
						class="w-6 h-6 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white" />
				</svelte:fragment>
			</SidebarItem>
			{#if signedIn}
				<SidebarItem label="Profile" {spanClass} href="/@userprofile" active={activeUrl.startsWith("/@")}>
					<svelte:fragment slot="icon">
						<UserSolid
							class="w-6 h-6 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white" />
					</svelte:fragment>
				</SidebarItem>
			{:else}
				<SidebarItem label="Sign In">
					<svelte:fragment slot="icon">
						<ArrowRightToBracketOutline
							class="w-6 h-6 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white" />
					</svelte:fragment>
				</SidebarItem>
				<SidebarItem label="Sign Up">
					<svelte:fragment slot="icon">
						<EditOutline
							class="w-6 h-6 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white" />
					</svelte:fragment>
				</SidebarItem>
			{/if}
		</SidebarGroup>
		<SidebarGroup border>
			<SidebarItem label="Create a post">
				<svelte:fragment slot="icon">
					<EditSolid
						class="w-6 h-6 text-gray-500 transition duration-75 dark:text-gray-400 group-hover:text-gray-900 dark:group-hover:text-white" />
				</svelte:fragment>
			</SidebarItem>
		</SidebarGroup>
	</SidebarWrapper>
</Sidebar>