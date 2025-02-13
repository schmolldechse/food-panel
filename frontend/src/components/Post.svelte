<script lang="ts">
	import type { PostOutDto } from "$lib/api/Api";
	import StarRating from "$components/star/StarRating.svelte";
	import { goofyRound } from "$lib";

	let { post }: { post: PostOutDto } = $props();

	const averageRating: number = goofyRound(post.averageRating!);
</script>

<style lang="postcss">
    .hover-underline-animation {
        @apply inline-block relative;
    }

    .hover-underline-animation::after {
        @apply content-[''] absolute w-full scale-x-0 h-[2px] bottom-0 left-0 origin-left transition-all duration-300;
        background-color: currentColor;
    }

    .hover-underline-animation:hover::after {
        @apply scale-x-100;
    }
</style>

<div class="py-4 flex flex-col">
	<div class="flex flex-row justify-between">
		<a class="text-2xl font-bold hover-underline-animation" href="/post/{post.id}">{post.title}</a>

		<div class="flex flex-wrap gap-x-2 items-end">
			<span class="text-base">by</span>
			<span class="text-xl text-text/75">{post.creatorName}</span>
		</div>
	</div>

	<span>{post.message}</span>
	<img src="http://localhost:9000/foodpanel/{post.id}.png" alt="post haha" height="255px" width="255px" />

	<div class="flex flex-row items-center gap-x-2">
		<StarRating stars={averageRating} />

		<span class="text-3xl align-middle">•</span>
		<span>{post.commentAmount} Ratings</span>
	</div>
</div>