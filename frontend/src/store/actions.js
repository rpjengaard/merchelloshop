import _get from 'lodash.get';
import dataApi from './api/data';
import _Object from './helpers/object';

export default {
	CACHE_SHOULD_INVALIDATE: ({ commit, getters, state }, newGuid) => {
		// Compare guid in cache to new guid received
		const contentChanged = state.cache.content.guid !== ''
							&& state.cache.content.guid !== newGuid;

		// Also make sure to check site id in cache with siteRootId
		// in store. If they do not match, we are on a different
		// (sub)site and the cache should be invalidated too.
		const siteId = getters.site.siteRootId;
		const siteChanged = siteId !== state.cache.content.siteId;

		if (contentChanged || siteChanged) {
			commit('CACHE_INVALIDATE_CONTENT');
			commit('CACHE_INVALIDATE_PAGETREE');
		} else if (getters.pageTree_shouldInvalidate) {
			commit('CACHE_INVALIDATE_PAGETREE');
		}

		commit('SET_CONTENT_GUID', newGuid);
		commit('SET_CONTENT_SITE_ID', siteId);
	},
	CACHE_CHECK: ({ commit, getters }) => {
		if (getters.content_invalid) {
			commit('CACHE_CLEAR_CONTENT');
		}
		if (getters.pageTree_invalid) {
			commit('CACHE_CLEAR_PAGETREE');
		}
	},
	CACHE_PAGETREE_ITEMS({ commit }, payload) {
		// Make sure we always have an array of one or more nodeTrees
		const nodeTrees = Array.isArray(payload) ? payload : [payload];

		nodeTrees.forEach((nodeTree) => {
			// Traverse each child node of the nested node tree
			_Object.forEachDeep(nodeTree, 'children', (node) => {
				commit('CACHE_PAGETREE', node);
			});
		});
	},
	CACHE_HAS_CHILDREN({ getters }, path) {
		const cached = getters.content_cached(path);
		if (cached) {
			const hasAllChildren = getters.pageTree_hasCachedChildren(cached.id);
			if (hasAllChildren) {
				return true;
			}
		}
		return false;
	},
	SAVE_CONTENT: ({ dispatch, commit }, { res, parts, path }) => {
		// Save site data + cache main navigation if site part is included
		if (res.data.site) {
			commit('SET_SITE', res.data.site);
			dispatch('CACHE_PAGETREE_ITEMS', res.data.site.mainNavigation);
		} else if (parts.indexOf('site') > -1) {
			console.error(`getData requested 'site' part, but received '${res.data.site}'. Hint: Make sure host is correctly set in project config in package.json`);
		}

		// Cache page tree nodes if navigation part is included
		if (res.data.navigation) {
			dispatch('CACHE_PAGETREE_ITEMS', res.data.navigation.context);
			dispatch('CACHE_PAGETREE_ITEMS', res.data.navigation.children);
		} else if (parts.indexOf('navigation') > -1) {
			console.error(`getData requested 'navigation' part, but received '${JSON.stringify(res.data.navigation)}'.`);
		}

		// Set current page content if content part is included.
		if (res.data.content) {
			if (res.status) {
				commit('SET_CURRENT_STATUS', res.status);
			}
			commit('SET_CURRENT_PAGE', res.data.content);
			if (!res.data.content.noCache) {
				commit('CACHE_CONTENT', {
					path,
					data: res.data.content,
				});
			}
		} else if (parts.indexOf('content') > -1) {
			console.error(`getData requested 'content' part, but received '${JSON.stringify(res.data.content)}'.`);
		}

		if (res.data.contentGuid) {
			// Use content guid to check if cache is invalidated and should be cleared
			// on next route change
			dispatch('CACHE_SHOULD_INVALIDATE', res.data.contentGuid);
		}
	},
	FETCH_CONTENT: ({ dispatch, commit, getters }, path) => new Promise((resolve, reject) => {
		const isServer = process.env.VUE_ENV === 'server';
		const fetchSite = getters.siteChanged;
		const noCache = getters.content_invalid || getters.pageTree_invalid || isServer;
		const serveFromCache = !noCache
			&& getters.content_cached(path)
			&& dispatch('CACHE_HAS_CHILDREN', path);

		// If able to use cache, resolve with cached immediately and fetch in background
		if (serveFromCache) {
			commit('SET_CURRENT_PAGE', getters.content_cached(path));
			resolve(getters.content_cached(path));
		} else {
			commit('PROGRESS_BAR_START');
		}

		const parts = (isServer || fetchSite)
			? ['content', 'navigation', 'site']
			: ['content', 'navigation'];

		const params = {
			parts: parts.join(','),
			url: path,
			navLevels: 2,
		};

		if (!isServer) {
			// When we're on the client we have the root id of the site and can use this
			// instead of appHost (this yields faster server response time)
			params.appSiteId = getters.siteRootId;
		}

		if (noCache) {
			// Make sure to get nav context (all parent tree nodes, including siblings
			// on each level)
			params.navContext = true;
		}

		// Initate request from dataApi
		dataApi.get(params, getters.entry)
			.then((res) => {
				if (res.request) {
					commit('SET_DATA_API_LATEST', dataApi.latestRequest);
				}
				// Check if cache should be cleared (if it was invalidated by guid
				// from previous api response)
				dispatch('CACHE_CHECK');

				if (res.data && typeof res.data === 'object') {
					dispatch('SAVE_CONTENT', {
						res,
						parts,
						path,
					});
					// Resolve with current page content
					resolve(getters.currentPage);
				} else {
					console.error('getData api fetch received no data.', res);
					commit('SET_CURRENT_STATUS', 500);
					res.status = 500;
					res.statusText = 'Internal server error. Received no data from api';
					reject(res);
				}

				// Finish progress bar
				commit('PROGRESS_BAR_FINISH');
			})
			.catch((err) => {
				if (err.request) {
					commit('SET_DATA_API_LATEST', (err.request.responseURL) ? err.request.responseURL : `${err.request.path}`);
				}
				const cached = getters.content_cached(path);

				if (
					err.response
					&& err.response.status === 404
					&& err.response.data
					&& err.response.data.content
				) {
					console.warn('404 error', err.response);
					dispatch('SAVE_CONTENT', {
						res: err.response,
						parts,
						path,
					});
					// Resolve with current page content but parse status throught to store
					resolve(getters.currentPage);
				} else if (!isServer && cached) {
					commit('SET_CURRENT_PAGE', cached);
					console.warn('getData api fetch encountered error. Serving from cache.', err);
					resolve(cached);
				} else {
					err.message += ` ${err.response.statusText || ''}. FETCH_CONTENT action failed`;
					reject(err);
				}

				if (!serveFromCache) {
					commit('PROGRESS_BAR_SET', 100);
					commit('PROGRESS_BAR_FAIL');
				}
			});
	}),
	FETCH_NAVIGATION({ dispatch, getters }, nodeId) {
		return new Promise((resolve, reject) => {
			const cached = getters.pageTree_itemWithChildren(nodeId);
			if (cached) {
				if ((cached.hasChildren && cached.children.length) || !cached.hasChildren) {
					resolve(cached);
					return;
				}
			}

			const params = {
				parts: 'navigation',
				nodeId,
				navLevels: 3,
				appSiteId: getters.siteRootId,
			};

			dataApi.get(params, getters.entry).then((res) => {
				if (res.data && res.data.navigation) {
					dispatch('CACHE_PAGETREE_ITEMS', res.data.navigation);
					resolve(getters.pageTree_itemWithChildren(res.data.id));
				} else {
					console.error('FETCH_NAVIGATION action received no navigation items.');
					reject(res);
				}
			}).catch((err) => {
				// If the requested nodeId has a redirect on it, we need to request again
				// with the new nodeId we receive.
				// TODO: Maybe handle this better in the backend, so we just get the right
				// data on the first try as long as we're only requesting navigation?
				const redirectToNodeId = _get(err, 'response.data.data.nodeId');
				console.info('redirect FETCH_NAVIGATION request:', redirectToNodeId, err);
				if (redirectToNodeId) {
					dispatch('FETCH_NAVIGATION', redirectToNodeId)
						.then(resolve)
						.catch(reject);
				} else {
					err.message += ` ${err.response.statusText || ''} FETCH_NAVIGATION action failed.`;
					reject(err);
				}
			});
		});
	},
	PRE_TRANSITION_START({ getters, commit, dispatch }, url) {
		commit('PRE_TRANSITION_TO', getters.pageTree_itemByUrl(url));
		dispatch('CHECK_SITE_CHANGE');
		if (process.env.VUE_ENV === 'server') {
			return Promise.resolve();
		}
		commit('PRE_TRANSITION_ACTIVE', true);
		commit('PRE_TRANSITION_PROMISE_NEW');
		return getters.preTransition.promise.promise;
	},
	PRE_TRANSITION_DONE({ commit }) {
		commit('PRE_TRANSITION_PROMISE_RESOLVE');
	},
	PRE_TRANSITION_STOP({ commit }) {
		commit('PRE_TRANSITION_ACTIVE', false);
	},
	CHECK_SITE_CHANGE({ getters, commit }) {
		if (getters.preTransition.to) {
			const cultureChanged = getters.preTransition.to.culture !== getters.currentPage.culture;
			if (cultureChanged) {
				commit('CHANGE_SITE');
			} else {
				commit('CHANGE_SITE', false);
			}
		}
	},
};
