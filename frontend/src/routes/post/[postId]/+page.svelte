<script lang="ts">
	import type {PageProps} from "./$types";
	import Post from "$components/Post.svelte";
	import StarRating from "$components/star/StarRating.svelte";
	import {page} from "$app/state";
	import apiClient from "$lib/ApiClient";
	import {invalidateAll} from "$app/navigation";
	import Rating from "$components/Rating.svelte";

	let {data}: PageProps = $props();

	let stars = $state<number>(0.5);
	let value = $state<string>("");

	const rate = async () => {
		if (!stars) return;
		if (stars < 0.5 || stars > 5) return;
		if (!value) return;

		await apiClient.ratings.v1RatingsCreate({
			postId: page.params.postId,
			stars: stars,
			message: value
		});
		await invalidateAll();
	}
</script>

<div class="flex flex-col gap-y-2">
    <Post post={data.post}/>

    <span>Rate this post below:</span>
    <div class="flex flex-col border rounded-lg px-2.5 pb-2 flex-wrap">
        <textarea
                bind:value
                placeholder="Rate with a message"
                class="bg-background py-4 w-full px-2 focus:outline-none
         break-words whitespace-pre-wrap overflow-y-auto resize-none"
        ></textarea>
        <div class="flex flex-row justify-between items-center">
            <StarRating bind:stars readOnly={false}/>
            <button class="bg-secondary rounded-lg px-4 py-2" onclick={rate}>
                Rate
            </button>
        </div>
    </div>

    {#if data.ratings.length > 0}
        <div class="flex flex-col divide-y-2 gap-y-2">
            <span>View other comments:</span>
            {#each data.ratings as rating}
                <Rating rating={rating}/>
            {/each}
        </div>
    {/if}
</div>
