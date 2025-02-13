<script lang="ts">
	import { page } from "$app/state";
	import PostsContainer from "$components/PostsContainer.svelte";
	import type { PostOutDto } from "$lib/api/Api";
	import type { PageProps } from "./$types";
	import StarRating from "$components/star/StarRating.svelte";
	import { goofyRound } from "$lib";

	let { data }: PageProps = $props();

	let posts: PostOutDto[] = $derived(data.posts);

	const averageRating: number = goofyRound(data.userData.averageRating!);
</script>

<div class="flex flex-col divide-y divide-gray-500">
	<div class="flex flex-row mb-4 gap-10">
		<div class="flex-1">
			<h2 class="text-2xl"> {data.userData.name}</h2>
			<h3 class="text-xl text-gray-400">@{page.params.handle}</h3>
		</div>

		<div class="flex-2 flex flex-col">
			<p class="text-right">{data.userData.postCount} posts</p>
			<span class="flex flex-row gap-1 items-center">
				<span class="text-3xl">&#x2300;</span>

				<StarRating stars={averageRating}/>
			</span>
		</div>


	</div>

	<PostsContainer {posts}/>

</div>
