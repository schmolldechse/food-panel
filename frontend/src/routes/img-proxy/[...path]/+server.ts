export const GET = async ({ url, fetch }) => {
	const target = new URL(url.pathname.replace('/img-proxy', ''), 'http://localhost:9000');
	return fetch(target.toString());
};
