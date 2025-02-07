import { ApiProvider } from "$lib/provider/ApiProvider";

export const apiProvider = new ApiProvider();

// Create a single instance of your API client
export default apiProvider.api;
