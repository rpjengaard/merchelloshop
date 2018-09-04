/**
 * uniformUrl
 * @param  {[string]} url [url]
 * @return {[string]}     [url always ending with slash - if source url does not contain query params]
 */
export default (url) => {
	const strippedUrl = url.split(/[?#]/)[0];
	if (strippedUrl.charAt(url.length - 1) === '/') {
		return url;
	}
	return `${url}/`;
};
