import type { PageLoad } from "./$types";
import apiClient from "$lib/ApiClient";
import type { PostOutDto, UserOutDto } from "$lib/api/Api";

export const load: PageLoad = async ({ params }): Promise<{ userData: UserOutDto, posts: PostOutDto[] }> => {
	const userData = (await apiClient.auth.v1AuthDetail(params.handle)).data;

	return {
		userData,
		posts: (await apiClient.post.v1PostDetail(userData.id!)).data
	}
};