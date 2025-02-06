<script lang="ts">
	import FileUpload from "$components/FileUpload.svelte";
	import apiClient from "$lib/apiClient";

	import { goto } from "$app/navigation";


	let selectedFile: File | undefined = $state(undefined);
	let title: string = $state("");
	let message: string = $state("");

	async function createPost() {
		if (selectedFile === undefined)
			return;

		const response = await apiClient.post.v1PostCreate({
			CreatorId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
			Title: title,
			Message: message,
			Image: selectedFile
		});

		if (!response.ok)
			return;

		await goto("/");
	}
</script>

<div class="flex justify-center items-center">
	<div class="w-[50rem]">
		<div class="flex flex-col gap-4">
			<input type="text" placeholder="Title" class="bg-primary" maxlength="128" bind:value={title}>
			<textarea class="resize-none bg-primary" bind:value={message}></textarea>

			<FileUpload bind:selectedFile={selectedFile} />
			<button type="button" onclick={createPost}
					class="w-fit flex justify-center items-center bg-accent text-2xl font-bold p-2 rounded-2xl">Post
			</button>
		</div>
	</div>
</div>
