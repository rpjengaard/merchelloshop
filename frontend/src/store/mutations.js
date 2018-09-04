import Vue from 'vue';
import _Object from './helpers/object';
import uniformUrl from './helpers/uniform-url';
import mergeArrays from './helpers/merge-arrays';

export default {
	/**
	 * Basic mutations
	 */
	SET_CURRENT_PAGE: (state, pageData) => {
		Vue.set(state, 'currentPage', pageData);
	},
	SET_CURRENT_STATUS: (state, statusCode) => {
		Vue.set(state, 'currentStatus', statusCode);
	},
	SET_CURRENT_TEMPLATE: (state, templateName) => {
		Vue.set(state, 'currentTemplate', templateName);
	},
	SET_SITE: (state, site) => {
		Vue.set(state, 'site', site);
	},
	SET_ENTRY: (state, entry) => {
		Vue.set(state, 'entry', entry);
	},
	SET_DATA_API_LATEST: (state, reqUrl) => {
		Vue.set(state.dataApi, 'latestRequest', reqUrl);
	},
	SET_CONTENT_GUID: (state, guid) => {
		Vue.set(state.cache.content, 'guid', guid);
	},
	SET_CONTENT_SITE_ID: (state, siteId) => {
		Vue.set(state.cache.content, 'siteId', siteId);
	},
	CHANGE_SITE: (state, boolean = true) => {
		// Force refetch of site part on next api fetch
		Vue.set(state, 'siteChanged', boolean);
	},
	/**
	 * Cache
	 */
	CACHE_CONTENT: (state, { path, data }) => {
		const givenUrl = uniformUrl(path);

		if (data) {
			const urls = [data.url];
			if (givenUrl !== data.url) {
				urls.push(givenUrl);
			}

			urls.forEach((key) => {
				Vue.set(state.cache.content.items, key, data);
				const existingIndex = state.cache.content.history.indexOf(key);
				if (existingIndex > -1) {
					state.cache.content.history.splice(existingIndex, 1);
				}
				state.cache.content.history.unshift(key);

				// Trim content cache length if it exceeds limit.
				if (state.cache.content.history.length > state.cache.content.limit) {
					const removeUrl = state.cache.content.history.pop();
					Vue.delete(state.cache.content.items, removeUrl);
				}
			});
		}
	},
	CACHE_PAGETREE(state, node) {
		// Clone object to avoid issues when traversing and mutating
		const current = _Object.clone(node);

		// If parent already exists in store make sure its children
		// array contains id of the node we are about to add
		const parent = state.cache.pageTree.items[node.parentId];
		if (parent) {
			// If children prop in parent isn't an array, make it that
			if (!Array.isArray(parent.children)) {
				Vue.set(parent, 'children', []);
			}
			// If node id doesn't already exist in parent children, add it
			if (parent.children.indexOf(current.id) === -1) {
				parent.children.push(current.id);
			}
		}

		// Transform children array to contain ids only
		if (Array.isArray(current.children)) {
			current.children = current.children
				.map(child => child.id)
				.filter((val, index, arr) => arr.indexOf(val) === index);
		} else {
			current.children = [];
		}

		// If current already exists in cache merge the new and existing
		// children arrays before adding to the store
		if (Object.keys(state.cache.pageTree.items).includes(String(current.id))) {
			const existingItem = state.cache.pageTree.items[current.id];
			if (existingItem && Array.isArray(existingItem.children)) {
				current.children = mergeArrays(existingItem.children, current.children);
			}
		}

		// Add transformed node to store
		Vue.set(state.cache.pageTree.items, current.id, current);
		Vue.set(state.cache.pageTree.urls, current.url, current.id);
	},
	CACHE_INVALIDATE_CONTENT: (state) => {
		console.warn('CACHE_INVALIDATE_CONTENT triggered');
		state.cache.content.invalid = true;
	},
	CACHE_INVALIDATE_PAGETREE: (state) => {
		console.warn('CACHE_INVALIDATE_PAGETREE triggered');
		state.cache.pageTree.invalid = true;
	},
	CACHE_CLEAR_CONTENT: (state) => {
		Vue.set(state.cache.content, 'items', {});
		Vue.set(state.cache.content, 'history', []);
		state.cache.content.invalid = false;
	},
	CACHE_CLEAR_PAGETREE: (state) => {
		Vue.set(state.cache.pageTree, 'items', {});
		state.cache.pageTree.invalid = false;
	},
	CACHE_CLEAN_PAGETREE: (state) => {
		Object.keys(state.cache.pageTree.items)
			.forEach((key) => {
				Vue.set(
					state.cache.pageTree.items,
					key,
					state.cache.pageTree.items[key]
						.filter((val, index, arr) => arr.indexOf(val) === index),
				);
			});
	},
	PRE_TRANSITION_ACTIVE: (state, active) => {
		Vue.set(state.preTransition, 'active', active);
	},
	PRE_TRANSITION_IGNORE: (state, ignore) => {
		Vue.set(state.preTransition, 'ignore', ignore);
	},
	PRE_TRANSITION_PROMISE_NEW: (state) => {
		let resolve;
		const promise = new Promise((r) => {
			resolve = r;
		});
		Vue.set(state.preTransition, 'promise', {
			resolve,
			promise,
		});
	},
	PRE_TRANSITION_PROMISE_RESOLVE: (state) => {
		state.preTransition.promise.resolve();
	},
	PRE_TRANSITION_TO: (state, to) => {
		Vue.set(state.preTransition, 'to', to);
	},
	/**
	 * Progress bar
	 */
	PROGRESS_BAR_MOUNT: (state, progressBar) => {
		Vue.set(state, 'progressBar', progressBar);
	},
	PROGRESS_BAR_START: (state) => {
		if (state.progressBar.start) {
			state.progressBar.start();
		}
	},
	PROGRESS_BAR_FINISH: (state) => {
		if (state.progressBar.finish) {
			state.progressBar.finish();
		}
	},
	PROGRESS_BAR_FAIL: (state) => {
		if (state.progressBar.fail) {
			state.progressBar.fail();
		}
	},
	PROGRESS_BAR_SET: (state, num) => {
		if (state.progressBar.set) {
			state.progressBar.set(num);
		}
	},
	/**
	 * Solution specific below (non-spa related)
	 */
};
