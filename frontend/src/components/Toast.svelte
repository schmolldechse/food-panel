<script lang="ts">
	import { onMount } from "svelte";
	import { fade } from "svelte/transition";

	let visible = $state(false);

	let { message = "This is a toast notification!", duration = 3000, ondissmiss = () => {  } }: {
		message: string,
		duration: number,
		ondissmiss: () => void
	} = $props();

	onMount(() => {
		visible = true;
		// Auto-dismiss after the specified duration
		const timer = setTimeout(() => {
			dismiss();
		}, duration);
		return () => clearTimeout(timer);
	});

	function dismiss() {
		visible = false;
		ondissmiss();
	}
</script>

{#if visible}
	<div
		class="fixed bottom-5 right-5 bg-red-600 text-white p-4 rounded shadow-lg flex items-center space-x-4 transform transition-all duration-300"
		in:fade={{ duration: 200 }}
		out:fade={{ duration: 400 }}
	>
		<div>{message}</div>
		<button onclick={dismiss} class="text-white hover:text-gray-300 focus:outline-none">
			<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
				 stroke="currentColor">
				<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
			</svg>
			<span class="sr-only">Close</span>
		</button>
	</div>
{/if}