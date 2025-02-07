<script lang="ts">
	import "../app.css";
	import Sidebar from "$components/Sidebar.svelte";
	import { setContext } from "svelte";

	let { children, data } = $props();

	setContext("userData", data);
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
