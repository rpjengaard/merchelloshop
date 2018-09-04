import Vue from 'vue';
// import VueAnalytics from 'vue-analytics';
import { createStore } from 'store';

import './errors';
import './svgs';
// import './partials/ProgressBar';
import './partials/AppFooter';
import './partials/AppHeader';
import './partials/PreTransition';
import './partials/Navigation';
import './partials/Breadcrumb';
import './partials/Page';
import './partials/Grid';
import './partials/SpaLink';

import appComponent from './App';
import createRouter from './router';
import VueLodashGet from './plugins/lodash-get';
import VueStripHtml from './plugins/strip-html';
import VueNl2br from './plugins/nl2br';
import Labels from './plugins/labels';

// Always enable devtools (TODO: Find way to toggle per node env vars?)
Vue.config.devtools = true;
// Enable performance monitoring when in development
Vue.config.performance = process.env.NODE_ENV !== 'production';

// Expose a factory function that creates a fresh set of store, router,
// app instances on each call (which is called for each SSR request)
export default function createApp() {
	// create store and router instances
	const store = createStore();
	const router = createRouter();

	Vue.use(VueLodashGet);
	Vue.use(VueStripHtml);
	Vue.use(VueNl2br);
	// Labels for multilang sites (adds $Labels to all instances from site object)
	Vue.use(Labels);

	// Vue.use(VueAnalytics, {
	// 	id: 'UA-XXX-X',
	// 	checkDuplicatedScript: true,
	// 	router,
	// 	autoTracking: {
	// 		pageviewOnLoad: false,
	// 	},
	// });

	// create the app instance.
	// here we inject the router, store and ssr context to all child components,
	// making them available everywhere as `this.$router` and `this.$store`.
	const app = new Vue({
		router,
		store,
		render: h => h(appComponent),
	});

	// expose the app, the router and the store.
	// note we are not mounting the app here, since bootstrapping will be
	// different depending on whether we are in a browser or on the server.
	return { app, router, store };
}
