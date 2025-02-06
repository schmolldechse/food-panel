<script lang="ts">
	import "../app.css";
	import Sidebar from "$components/Sidebar.svelte";

	let { children } = $props();
</script>

<div class="m-4 flex flex-row space-x-6">
	<div class="flex-[1]">
		<Sidebar />
	</div>

	<div class="flex-[6] border-x">
		{@render children()}
	</div>

	<div class="flex-[1]"></div>
</div>


