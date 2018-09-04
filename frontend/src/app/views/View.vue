<script>
// page-level code splitting
const viewComponents = {
	Frontpage: () => import(
		/* webpackChunkName: "frontpage" */
		'./Frontpage/Frontpage',
	),
	Archive: () => import(
		/* webpackChunkName: "archive" */
		'./Archive/Archive',
	),
	Subpage: () => import(
		/* webpackChunkName: "subpage" */
		'./Subpage/Subpage',
	),
};

function getComponentName(content) {
	const template = (content && content.template) ? content.template : null;

	if (Object.keys(viewComponents).indexOf(template) !== -1) {
		return template;
	}

	const remappings = {
		Forside: 'Frontpage',
		Nyhed: 'News',
		Indholdsside: 'Subpage',
	};

	const remapKey = Object.keys(remappings)
		.find(key => (key && template) && (key.toLowerCase() === template.toLowerCase()));

	if (!remapKey) {
		console.warn(`[View.vue] Could not resolve template name '${template}'`);
	}

	// use fallback component 'Subpage' when api template does not exist
	return remappings[remapKey] || 'Subpage';
}

export default {
	name: 'spaView',
	components: viewComponents,
	data() {
		return {
			content: this.$store.getters.currentPage,
		};
	},
	computed: {
		componentName() {
			return getComponentName(this.content);
		},
	},
	mounted() {
		this.$store.dispatch('PRE_TRANSITION_STOP');
	},
	/**
	 * asyncData is a custom method executed in entry-client.js
	 * and entry-server.js. Returns promise that delays rendering
	 * of component both client and server side.
	 */
	asyncData({ store, route }) {
		// If umbraco preview url - include query params
		const url = (route.path.indexOf('/umbraco/dialogs/Preview.aspx') > -1)
			? route.fullPath
			: route.path;
		return new Promise((resolve, reject) => {
			Promise.all([
				store.dispatch('PRE_TRANSITION_START', decodeURIComponent(url)),
				store.dispatch('FETCH_CONTENT', decodeURIComponent(url)),
			]).then((res) => {
				const content = res.slice(-1)[0] || {};
				if (content.template) {
					store.commit('SET_CURRENT_TEMPLATE', getComponentName(content));
				}
				resolve();
			}).catch((err) => {
				reject(err);
			});
		});
	},
};
</script>

<template src="./View.html"></template>
<style src="./View.scss"></style>
