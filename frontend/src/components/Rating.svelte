<script lang="ts">
	import type { RatingOutDto, UserOutDto } from "$lib/api/Api";
	import StarRating from "$components/star/StarRating.svelte";
	import { getContext } from "svelte";
	import { Trash2 } from "lucide-svelte";
	import apiClient from "$lib/ApiClient";
	import { invalidateAll } from "$app/navigation";

	let { rating }: { rating: RatingOutDto } = $props();

	const userData = getContext<{ currentUser: UserOutDto }>("userData");

	const deleteRating = async (ratingId: string) => {
		await apiClient.ratings.v1RatingsDelete({ ratingId: ratingId });
		await invalidateAll();
	};
</script>

<div>
	<div class="flex flex-row justify-between">
		<span class="text-white/70 text-lg">{rating?.creatorName}</span>
		<div class="flex flex-row gap-x-2 items-center">
			{#if userData?.currentUser.id === rating.creatorId}
				<Trash2 color="red" class="cursor-pointer" onclick={() => {
					if (!rating.id) return;
					deleteRating(rating.id);
				}} />
			{/if}
			<StarRating stars={rating?.stars ?? 0} />
		</div>
	</div>

	<span>{rating?.message}</span>
</div>
