<script lang="ts">
	let dragActive = $state(false);
	let { selectedFile = $bindable() }: { selectedFile: File | undefined } = $props();

	function handleDragOver(event: DragEvent) {
		event.preventDefault();
		dragActive = true;
	}

	function handleDragLeave(event: DragEvent) {
		event.preventDefault();
		dragActive = false;
	}

	function handleDrop(event: DragEvent) {
		event.preventDefault();
		dragActive = false;
		if (event.dataTransfer?.files?.length) {
			selectedFile = event.dataTransfer.files[0];
		}
	}

	// Handle when a file is selected via the file input
	function handleFileChange(event: Event) {
		const input = event.target as HTMLInputElement;
		if (input.files && input.files.length) {
			selectedFile = input.files[0];
		}
		// Reset the input's value so that the file dialog does not re-open automatically
		input.value = "";
	}

	// For keyboard accessibility: trigger file selection when Enter or Space is pressed
	function handleKeyDown(event: KeyboardEvent) {
		if (event.key === "Enter" || event.key === " ") {
			const fileInput = document.getElementById("fileInput") as HTMLInputElement;
			if (fileInput) {
				fileInput.click();
			}
		}
	}
</script>

<!-- Always-visible drag-and-drop area -->
<div
	class="relative flex cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed p-6 transition-colors duration-200 {dragActive
		? 'border-blue-500 bg-blue-50'
		: 'border-gray-300'}"
	ondragleave={handleDragLeave}
	ondragover={handleDragOver}
	ondrop={handleDrop}
	onkeydown={handleKeyDown}
	role="button"
	tabindex="0"
>
	<p class="text-gray-600">Drag &amp; drop your file here</p>
	<p class="text-sm text-gray-500">or click to select</p>
	<!-- The invisible file input covering the entire drop area -->
	<input
		class="absolute inset-0 h-full w-full cursor-pointer opacity-0"
		id="fileInput"
		onchange={handleFileChange}
		type="file"
	/>
</div>

{#if selectedFile !== undefined}
	<p>Selected file: {selectedFile.name}</p>
{/if}
