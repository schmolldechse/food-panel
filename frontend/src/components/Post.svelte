<script lang="ts">
	import type { PostOutDto } from "$lib/api/Api";
	import Star from "$components/svg/Star.svelte";

	let { post }: { post: PostOutDto } = $props();
</script>

<div class="py-4 flex flex-col">
	<div class="flex flex-row justify-between">
		<span class="text-2xl font-bold">{post.title}</span>

		<div class="flex flex-wrap gap-x-2 items-end">
			<span class="text-base">by</span>
			<span class="text-xl text-text/75">{post.creatorName}</span>
		</div>
	</div>

	<span>{post.message}</span>
	<img src="http://localhost:9000/foodpanel/{post.id}.png" alt="bastard" height="255px" width="255px" />

	<div class="flex flex-row items-center gap-x-2">
		<Star />
		<span>{post.averageRating}</span>

		<span class="text-3xl align-middle">•</span>
		<span>{post.commentAmount} Ratings</span>
	</div>
</div>