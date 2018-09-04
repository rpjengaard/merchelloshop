/**
 * HOW OUR SPA HANDLES CDN OF FRONTEND ASSETS
 * ================================================================
 * Buckle up - this is a bit complicated, but very awesome. :)
 *
 * This file is used to set the webpack output.publicPath property
 * dynamically at runtime. This is useful in cases where frontend
 * assets are hosted on a CDN (LEGO Foundation etc.). This file
 * takes care of the client side of things, ie. any async chunks.
 *
 *
 * GENERAL CONCEPT:
 * The challenge: We want to set the publicPath dynamically via a
 * node environment variable - ie. only on the live server. So we
 * can't rely on just setting it in the webpack.config like usual,
 * since this assignment only happens build time.
 *
 * It all works like this:
 *
 * 1)	The node environment variable "PUBLIC_PATH" is set on the
 * 		IIS (iisnode via Web.config etc.)
 *
 * 2)	Our node server catches process.env.PUBLIC_PATH (check
 * 		server/index.js) and overrides the publicPath prop in the
 * 		Vue server renderer client bundle
 * 		(/dist/vue-ssr-client-manifest.json)
 *
 * 3)	Also in node, we add the publicPath to the context object
 * 		that is parsed into the vue server render's renderToString()
 * 		method.
 * 		This allows us to reference it in index.template.html and
 * 		prefix any hard-coded assets with the publicPath too
 * 		(manifest.json and favicons etc.)
 *
 * 4) 	The two steps above take care of pointing all frontend assets
 * 		to the defined publicPath (ie. a CDN or whatever) during SSR.
 * 		To handle the client side of things, we also make sure that
 * 		index.template.html includes an inline script, that defines
 * 		the __webpack_public_path__ var on window.
 *
 * 5)	Finally this module reassigns the __webpack_public_path__
 * 		var to the global scope so webpack finds it. See:
 * 		https://webpack.js.org/guides/public-path/#on-the-fly
 */

if (window && window.__webpack_public_path__) {
	/* eslint-disable */
	__webpack_public_path__ = window.__webpack_public_path__;
	/* eslint-enable */
}
