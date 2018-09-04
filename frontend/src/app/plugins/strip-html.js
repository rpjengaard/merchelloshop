export default {
	install(Vue) {
		const HTMLregex = /(<([^>]+)>)/ig;
		const BRregex = /(?:(\s*)?<br\s*\/?\s*>(\s*)?)/ig;
		const stripHtml = str => str.replace(BRregex, ' ')
			.replace(HTMLregex, '');

		Vue.prototype.$stripHTML = stripHtml;
		Vue.prototype.$stripHtml = stripHtml;
	},
};
