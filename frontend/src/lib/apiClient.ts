import { Api, HttpClient } from "$lib/api/Api";

// Create a single HTTP client with common configuration (base URL, headers, etc.)
const httpClient = new HttpClient({
	baseUrl: "http://localhost:5226" // replace with your API base URL
	// Optionally add a security worker or interceptors here
});

// Create a single instance of your API client
export default new Api(httpClient);
