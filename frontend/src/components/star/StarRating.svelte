<script lang="ts">
	import Star from "$components/star/Star.svelte";

	let { stars, readOnly = false }: { stars: number, readOnly?: boolean } = $props();

	// Calculate the integer and fractional parts of the score
	const intScore: number = Math.floor(stars);
	const fractionScore: number = stars - intScore;
</script>

{#each Array(5) as _, id}
	{#if id === intScore}
		<Star readOnly={readOnly} fillPercentage={fractionScore} />
	{:else if id < intScore}
		<Star readOnly={readOnly} fillPercentage={1} />
	{:else}
		<Star readOnly={readOnly} fillPercentage={0} />
	{/if}
{/each}