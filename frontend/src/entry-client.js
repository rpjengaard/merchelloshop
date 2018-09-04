import Vue from 'vue';
import deepmerge from 'deepmerge';
import _get from 'lodash.get';
import 'es6-promise/auto';
import createPersistedState from 'vuex-persistedstate';
import { sync } from 'vuex-router-sync';
import create from './index';

// a global mixin that calls `asyncData` when a route component's params change
Vue.mixin({
	// https://router.vuejs.org/guide/advanced/navigation-guards.html#global-guards
	beforeRouteUpdate(to, from, next) {
		const { asyncData } = this.$options;
		if (asyncData) {
			asyncData({
				store: this.$store,
				route: to,
			}).then(() => {
				next();
			}).catch((err) => {
				// Do redirect if error has response data with url inside
				// (client side handled here - see node.js server side error
				// handling for that side of the story)
				const redirectUrl = _get(err, 'response.data.data.url');
				if (redirectUrl) {
					console.info('Redirect:', redirectUrl, to.path);
					next(redirectUrl);
				} else {
					next(err);
					// If in production: Try reloading - either to get page that
					// works or a server side rendered error page.
					if (process.env.NODE_ENV === 'production') {
						window.location.assign(to.path);
					}
				}
			});
		} else {
			next();
		}
	},
});

const { app, router, store } = create();

if (window.__INITIAL_STATE__) {
	try {
		// Before merging initial state with persistedStore we check if the persisted
		// store is invalid (content guid has changed).
		const persistedStore = JSON.parse(window.localStorage.getItem('vuex'));
		if (_get(persistedStore, 'cache.content.guid') === _get(window, '__INITIAL_STATE__.cache.content.guid')) {
			// TODO - check if chunkhashes in js has changed since last visit?
			store.replaceState(deepmerge(persistedStore, window.__INITIAL_STATE__));
			// merging with a persisted store might end up creating duplicate ids in
			// children in page tree nodes, so we clean this up after merging
			store.commit('CACHE_CLEAN_PAGETREE');
		} else {
			store.replaceState(window.__INITIAL_STATE__);
		}
	} catch (err) {
		store.replaceState(window.__INITIAL_STATE__);
	}

	// prime the store with server-initialized state. the state is determined
	// during SSR and inlined in the page markup.
	store.replaceState(window.__INITIAL_STATE__);

	// createPersistedState() doesn't deepmerge, but it will overwrite state,
	// so we delete any persisted state (we already merged manually above)
	window.localStorage.removeItem('vuex');

	// Setup persisted state
	createPersistedState({
		paths: [
			'site',
			'cache.pageTree',
			'cache.content',
		],
	})(store);

	// Keep store in vue devtools updated too
	// TODO: Possibility for pull request on devtool repos on this?
	// https://github.com/vuejs/vue-devtools/
	// https://github.com/vuejs/vuex/blob/2a67103a1f5fc1448a694e7f83a5f0c6d6bc8262/src/plugins/devtool.js
	if (process.env.NODE_ENV !== 'production' && store._devtoolHook) {
		store._devtoolHook.store.replaceState(store.state);
	}
}

// Sync the router with the vuex store. This registers `store.state.route`
// Only do this client side because url hashes are not accessible server-side.
// As a result, any hash would be lost if the store returned a route instanced
// on the server.
sync(store, router);

// wait until router has resolved all async before hooks
// and async components...
router.onReady(() => {
	// Add router hook for handling asyncData.
	// Doing it after initial route is resolved so that we don't double-fetch
	// the data that we already have. Using router.beforeResolve() so that all
	// async components are resolved.
	router.beforeResolve((to, from, next) => {
		const matched = router.getMatchedComponents(to);
		const prevMatched = router.getMatchedComponents(from);
		let diffed = false;
		const activated = matched.filter((c, i) => {
			diffed = diffed || (diffed = (prevMatched[i] !== c));
			return diffed;
		});
		const asyncDataHooks = activated.map(c => c.asyncData).filter(_ => _);
		if (!asyncDataHooks.length) {
			return next();
		}

		return Promise.all(asyncDataHooks.map(hook => hook({ store, route: to })))
			.then(() => {
				next();
			})
			.catch(next);
	});

	// actually mount to DOM
	app.$mount('#app');
});

// service worker
if (window.location.protocol === 'https:' && navigator.serviceWorker) {
	navigator.serviceWorker.register('/service-worker.js');
}
