<script lang="ts">
	import type {RatingOutDto} from "$lib/api/Api";
	import StarRating from "$components/star/StarRating.svelte";

	let {rating}: { rating: RatingOutDto } = $props();
</script>

<div>
    <div class="flex flex-row justify-between">
        <span class="text-white/70 text-lg">{rating?.creatorName}</span>
        <StarRating stars={rating?.stars}/>
    </div>

    <span>{rating?.message}</span>
</div>
