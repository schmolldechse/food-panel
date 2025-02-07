<script lang="ts">
	import type {PostOutDto} from "$lib/api/Api";
    let { post }: { post: PostOutDto } = $props();
</script>

<p>{post.id}</p>