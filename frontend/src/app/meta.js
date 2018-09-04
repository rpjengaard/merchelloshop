import Vue from 'vue';
import Meta from 'vue-meta';

Vue.use(Meta);

// Convenience method for auto-generating an array open graph meta values
function ogMeta(metaData) {
	return Object.keys(metaData)
		// only use keys beginning with 'og:*'
		.filter(key => key.indexOf('og:') === 0)
		.reduce((acc, key) => {
			// 'og:image' can contain array of images. Handle this.
			if (key === 'og:image' && Array.isArray(metaData[key])) {
				metaData[key].forEach((image) => {
					acc.push({
						name: 'og:image',
						property: 'og:image',
						content: image.url,
					});
					acc.push({
						name: 'og:image:width',
						property: 'og:image:width',
						content: image.width,
					});
					acc.push({
						name: 'og:image:height',
						property: 'og:image:height',
						content: image.height,
					});
				});
			// Otherwise just push 1:1
			} else {
				acc.push({
					name: key,
					property: key,
					content: metaData[key],
				});
			}
			return acc;
		}, []);
}

export default {
	metaInfo() {
		const currentMeta = this.$_get(this.$store.getters.currentPage, 'meta')
			|| { title: 'Page not found', robots: 'noindex, nofollow' };

		// Check that host is not testserver.nu + not local, ie 'localhost', 'rpjengaard' etc.
		const projectID = process.env.SKY_PROJECT_ID || '0000xx';
		const entryHost = this.$_get(this.$store.getters.entry, 'host') || '';
		const isTestserver = entryHost.includes(projectID);

		// Force noindex nofollow when not live
		if (isTestserver) {
			currentMeta.robots = 'noindex, nofollow';
		}

		return {
			title: this.$_get(currentMeta, 'title', '')
				.replace(/&amp;/g, '&'),
			htmlAttrs: {
				lang: (this.$_get(this.$store.getters.currentPage, 'culture') || '')
					.replace(/-.*$/, '') || 'en',
			},
			link: [
				{
					rel: 'canonical',
					href: this.$_get(currentMeta, 'canonical'),
				},
			],
			meta: [
				{
					name: 'description',
					content: this.$_get(currentMeta, 'description'),
				},
				{
					name: 'robots',
					content: this.$_get(currentMeta, 'robots'),
				},
				...ogMeta(currentMeta),
			],
		};
	},
};
