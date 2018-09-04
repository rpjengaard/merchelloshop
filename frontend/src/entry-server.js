import create from './index';

// This exported function will be called by `bundleRenderer`.
// This is where we perform data-prefetching to determine the
// state of our application before actually rendering it.
// Since data fetching is async, this function is expected to
// return a Promise that resolves to the app instance.
export default context => new Promise((resolve, reject) => {
	const { app, router, store } = create();

	const { url, host, origin, href, userAgent, webp } = context;
	const { fullPath, path } = router.resolve(url).route;

	// remove hashes and query params from url and reject if route path does not match
	const urlPath = url.split(/[?#]/)[0];
	if (path !== urlPath) {
		reject({ url: fullPath });
	} else {
		// set router's location
		router.push(url);

		store.commit('SET_ENTRY', { host, origin, href, userAgent, webp });

		// wait until router has resolved possible async hooks
		router.onReady(() => {
			const matchedComponents = router.getMatchedComponents();
			// 404 when no matched routes (unlikely as long as we have a '*' route - but
			// needed for SPAs without such dynamic routing)
			if (!matchedComponents.length) {
				reject({
					response: {
						status: 404,
					},
				});
			} else {
				// Call fetchData hooks on components matched by the route.
				// A preFetch hook dispatches a store action and returns a Promise,
				// which is resolved when the action is complete and store state has been
				// updated.
				Promise.all(matchedComponents.map(({ asyncData }) => asyncData && asyncData({
					store,
					route: router.currentRoute,
				}))).then(() => {
					// After all preFetch hooks are resolved, our store is now
					// filled with the state needed to render the app.
					// Expose the state on the render context, and let the request handler
					// inline the state in the HTML response. This allows the client-side
					// store to pick-up the server-side state without having to duplicate
					// the initial data fetching on the client.
					context.state = store.state;
					context.meta = app.$meta();
					resolve(app);
				}).catch(reject);
			}
		}, reject);
	}
});
