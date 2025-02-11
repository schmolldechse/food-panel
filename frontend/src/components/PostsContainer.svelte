<script lang="ts">
	import Post from "$components/Post.svelte";
	import type { PostOutDto } from "$lib/api/Api";

	let { posts }: { posts: PostOutDto[] } = $props();
</script>

<div class="flex flex-col divide-y divide-gray-500">
	{#each posts as post}
		<Post {post}/>
	{/each}
</div>