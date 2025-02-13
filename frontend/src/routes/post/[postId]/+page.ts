import type { PageLoad } from "./$types";
import apiClient from "$lib/ApiClient";
import type { PostOutDto, Rating } from "$lib/api/Api";

export const load: PageLoad = async ({ params }): Promise<{ post: PostOutDto, ratings: Rating[] }> => {
	return {
		post: (await apiClient.post.v1PostDetail(params.postId)).data,
		ratings: (await apiClient.ratings.v1RatingsGetRatingsByPostIdList({ postId: params.postId })).data,
	}
};