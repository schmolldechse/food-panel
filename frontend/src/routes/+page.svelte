<script lang="ts">
	import apiClient from "$lib/ApiClient";
	import type {PostOutDto} from "$lib/api/Api";
    import { onMount } from "svelte";
    import PostsContainer from "$components/PostsContainer.svelte";

	let posts: PostOutDto[] = $state([]);

	const fetchPosts = async () => {
		const response = await apiClient.post.v1PostList();
		if (response.status !== 200) return;

		const data = response.data;
		if (!data || !Array.isArray(data)) return;

		posts = data;
	};

    onMount(() => fetchPosts());
</script>

<PostsContainer {posts} />
