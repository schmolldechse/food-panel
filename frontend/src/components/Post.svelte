<script lang="ts">
	import type { PostOutDto } from "$lib/api/Api";
	import StarRating from "$components/star/StarRating.svelte";
	import { goofyRound } from "$lib";

	let { post }: { post: PostOutDto } = $props();

	const averageRating: number = $derived(goofyRound(post.averageRating!));
</script>

<div class="py-4 flex flex-col">
	<div class="flex flex-row justify-between">
		<a class="text-2xl font-bold hover-underline-animation" href="/post/{post.id}">{post.title}</a>

		<div class="flex flex-wrap gap-x-2 items-end">
			<span class="text-base">by</span>
			<a href="/@{post.creatorHandle}" class="text-xl text-text/75 hover-underline-animation">{post.creatorName}</a>
		</div>
	</div>

	<span>{post.message}</span>
	<img src="/img-proxy/foodpanel/{post.id}.png" alt="post haha" height="255px" width="255px" />

	<div class="flex flex-row items-center gap-x-2">
		<StarRating stars={averageRating} />

		<span class="text-3xl align-middle">•</span>
		<span>{post.commentAmount} Ratings</span>
	</div>
</div>