<script lang="ts">
	// Destructure readOnly and fillPercentage from props (id removed)
	let { readOnly = false, fillPercentage }: { readOnly: boolean, fillPercentage: number } = $props();

	// Generate a unique gradient ID for this component instance
	const gradientId = "linear-gradient-" + Math.random().toString(36).substring(2, 9);

	const starConfig = {
		fillColor: "#e4c100",
		strokeColor: "#000",
		unfilledColor: "#808080",
		strokeUnfilledColor: "#000",
		size: 20
	};
</script>

<svg xmlns="http://www.w3.org/2000/svg" width="{starConfig.size}" height="{starConfig.size}"
	 viewBox="0 -10 187.673 179.503">
	{#if fillPercentage < 1 && fillPercentage > 0}
		<defs>
			<linearGradient id="{gradientId}" x1="0%" y1="0%" x2="100%" y2="0%">
				<stop offset="0%" style="stop-color:{starConfig.fillColor};stop-opacity:1" />
				<stop offset="{fillPercentage * 100}%" style="stop-color:{starConfig.fillColor};stop-opacity:1" />
				<stop offset="{fillPercentage * 100}%" style="stop-color:{starConfig.unfilledColor};stop-opacity:1" />
			</linearGradient>
		</defs>
	{/if}
	<path
		stroke={fillPercentage > 0 ? starConfig.strokeColor : starConfig.strokeUnfilledColor}
		fill={fillPercentage === 1
			? starConfig.fillColor
			: fillPercentage === 0
				? starConfig.unfilledColor
				: `url(#${gradientId})`}
		d="M187.183 57.47a9.955 9.955 0 00-8.587-6.86l-54.167-4.918-21.42-50.134a9.978 9.978 0 00-9.172-6.052
        9.972 9.972 0 00-9.172 6.061l-21.42 50.125L9.07 50.611a9.973 9.973 0 00-8.578 6.858
        9.964 9.964 0 002.917 10.596l40.944 35.908-12.073 53.184a9.97 9.97 0 003.878 10.298A9.953
        9.953 0 0042 169.357a9.937 9.937 0 005.114-1.424l46.724-27.925 46.707 27.925a9.936
        9.936 0 0010.964-.478 9.979 9.979 0 003.88-10.298l-12.074-53.184 40.944-35.9a9.98
        9.98 0 002.925-10.604zm0 0"
	/>
</svg>