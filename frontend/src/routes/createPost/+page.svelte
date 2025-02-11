<script lang="ts">
	import FileUpload from "$components/FileUpload.svelte";
	import apiClient from "$lib/ApiClient";

	import { goto } from "$app/navigation";

	let selectedFile: File | undefined = $state(undefined);
	let title: string = $state("");
	let message: string = $state("");

	async function createPost() {
		if (selectedFile === undefined) return;

		const response = await apiClient.post.v1PostCreate({
			Title: title,
			Message: message,
			Image: selectedFile
		});

		await goto("/");
	}
</script>

<div class="flex items-center justify-center">
	<div class="w-full">
		<div class="flex flex-col gap-4">
			<input bind:value={title} class="bg-primary" maxlength="128" placeholder="Title" type="text" />
			<textarea bind:value={message} class="resize-none bg-primary"></textarea>

			<FileUpload bind:selectedFile />
			<button
				class="flex w-fit items-center justify-center rounded-2xl bg-accent p-2 text-2xl font-bold"
				onclick={createPost}
				type="button"
			>Post
			</button>
		</div>
	</div>
</div>
