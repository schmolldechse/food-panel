<script lang="ts">
	import type { PageProps } from "./$types";
	import Post from "$components/Post.svelte";

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col divide-y divide-gray-500">
	<Post post={data.post} />

	{#each data.ratings as rating}
		<span>{rating.stars}</span>
	{/each}
</div>
