<script lang="ts">
	import Star from "$components/star/Star.svelte";
	import { goofyRound } from "$lib";

	let { stars = $bindable(0.5), readOnly = true }: { stars: number, readOnly?: boolean } = $props();

	// Calculate the integer and fractional parts of the score
	const intScore: number = $derived(Math.floor(stars));
	const fractionScore: number = $derived(stars - intScore);

	const handleClick = (event: MouseEvent) => {
		if (readOnly) return;

		const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
		const percentage = (event.clientX - rect.left) / rect.width;
		// Hilfe vom Arbeitskollegen (Herr GPT)
		stars = goofyRound(Math.min(5, Math.max(0.5, (percentage * 4.5) + 0.5)));
	};
</script>

<button class="relative" class:cursor-default={readOnly} onclick={handleClick}>
	<span class="flex flex-row">
		{#each Array(5) as _, id}
			{#if id === intScore}
				<Star readOnly={readOnly} fillPercentage={fractionScore} />
			{:else if id < intScore}
				<Star readOnly={readOnly} fillPercentage={1} />
			{:else}
				<Star readOnly={readOnly} fillPercentage={0} />
			{/if}
		{/each}
	</span>

	{#if !readOnly}
		<input class="slider" type="range" min={0.5} max={5} step={0.5} bind:value={stars} aria-label="Star rating" />
	{/if}
</button>

<style lang="postcss">
    .slider {
		@apply opacity-0 cursor-pointer absolute top-0 left-0 right-0 h-full;
    }
</style>