import _Object from './helpers/object';
import uniformUrl from './helpers/uniform-url';

export default {
	siteRootId(state) {
		return state.site.siteRootId;
	},
	site(state) {
		return state.site;
	},
	siteChanged(state) {
		return state.siteChanged;
	},
	entry(state) {
		return state.entry;
	},
	dataApi(state) {
		return state.dataApi;
	},
	currentPage(state) {
		return state.currentPage;
	},
	currentTemplate(state) {
		return state.currentTemplate;
	},
	content_invalid(state) {
		return state.cache.content.invalid;
	},
	content_cached(state) {
		return (path) => {
			const key = uniformUrl(path);
			const cached = state.cache.content.items[key];
			if (cached) {
				return cached;
			}
			if (key === state.currentPage.url) {
				return state.currentPage;
			}
			if (state.cache.content.items.length) {
				// Only logging if cache has items avoids unnecessary logs from
				// computed props before asyncData resolves and adds items to cache
				console.error(`Getter 'content_cached' failed - ${key} does not exist in cache.`);
			}
			return null;
		};
	},
	pageTree_invalid(state) {
		return state.cache.pageTree.invalid;
	},
	pageTree_item(state) {
		return (ids) => {
			if (Array.isArray(ids)) {
				return ids.map(id => state.cache.pageTree.items[id]);
			}
			return state.cache.pageTree.items[ids];
		};
	},
	pageTree_itemByUrl(state) {
		return (url) => {
			const id = state.cache.pageTree.urls[url];
			return state.cache.pageTree.items[id];
		};
	},
	/**
	 * Get cached page tree item(s) from id(s)
	 * @param  {number} ids
	 * @return {array[object]} array of page tree items
	 */
	pageTree_itemWithChildren(state) {
		const unflatten = (item) => {
			const itemClone = _Object.clone(item);
			if (itemClone && itemClone.hasChildren) {
				const children = itemClone.children.slice(0);
				itemClone.children = [];
				children.forEach((childId) => {
					const child = state.cache.pageTree.items[childId];
					if (child) {
						itemClone.children.push(unflatten(child));
					}
				});
			}
			return itemClone;
		};
		return (ids) => {
			if (Array.isArray(ids)) {
				return ids.map(id => unflatten(state.cache.pageTree.items[id]));
			}
			return unflatten(state.cache.pageTree.items[ids]);
		};
	},
	/**
	 * Get cached page tree from
	 * @param  {number} parentId
	 * @return {array[object]} array of page tree items
	 */
	pageTree_childrenOf(state) {
		return (parentId) => {
			const parentNode = state.cache.pageTree.items[parentId];
			let children = [];
			if (parentNode && parentNode.hasChildren && parentNode.children.length) {
				children = parentNode.children.map(childId => state.cache.pageTree.items[childId]);
			}

			return children;
		};
	},
	pageTree_hasCachedChildren(state) {
		return function get(id) {
			const cached = state.cache.pageTree.items[id];
			return (cached && cached.hasChildren && cached.children && cached.children.length);
		};
	},
	pageTree_shouldInvalidate(state) {
		return Object.keys(state.cache.pageTree.items).length >= state.cache.pageTree.limit;
	},
	preTransition(state) {
		return state.preTransition;
	},
	/**
	 * Solution specific below (non-spa related)
	 */
};
