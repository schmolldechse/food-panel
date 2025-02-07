<script lang="ts">
	import apiClient from "$lib/ApiClient";
	import type {PostOutDto} from "$lib/api/Api";
	import Post from "$components/Post.svelte";

	let posts: PostOutDto[] = $state([]);

	const fetchPosts = async () => {
		const response = await apiClient.post.v1PostList();
		if (response.status !== 200) return;

		const data = response.data;
		if (!data || !Array.isArray(data)) return;

		posts = data;
	};

	fetchPosts();
</script>

<div class="flex flex-col divide-y divide-gray-500">
    {#each posts as post}
        <Post {post}/>
    {/each}
</div>
