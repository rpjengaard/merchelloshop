// Use dotenv to add process variables from optional .env file
require('dotenv').config()

const pkg = require('../package.json')
const fs = require('fs')
const path = require('path')
const LRU = require('lru-cache')
const express = require('express')
const proxy = require('http-proxy-middleware');
// const favicon = require('serve-favicon')
const compression = require('compression')
const resolve = file => path.resolve(__dirname, file)
const { createBundleRenderer } = require('vue-server-renderer')
const chalk = require('chalk')
const SkyLogError = require('../SkyLogError');


const isProd = process.env.NODE_ENV === 'production'
const useMicroCache = process.env.MICRO_CACHE !== 'false'
const publicPath = process.env.PUBLIC_PATH || ''

const app = express()

function createRenderer (bundle, options) {
	// https://github.com/vuejs/vue/blob/dev/packages/vue-server-renderer/README.md#why-use-bundlerenderer
	return createBundleRenderer(bundle, Object.assign(options, {
		// for component caching
		cache: LRU({
			max: 1000,
			maxAge: 1000 * 60 * 15
		}),
		shouldPreload: (file, type) => {
			// type is inferred based on the file extension.
			// https://fetch.spec.whatwg.org/#concept-request-destination
			if (type === 'script' || type === 'style') {
				return true
			}
			if (type === 'font') {
				// only preload (better compressed) woff2 fonts
				return /\.woff2$/.test(file)
			}
		},
		// this is only needed when vue-server-renderer is npm-linked
		basedir: resolve('../dist'),
		// recommended for performance
		runInNewContext: false
	}))
}

let renderer
let readyPromise
const templatePath = isProd
	? resolve('../dist/index.template.html')
	: resolve('../frontend/src/index.template.html');
if (isProd) {
	// In production: create server renderer using template and built server bundle.
	// The server bundle is generated by vue-ssr-webpack-plugin.
	const template = fs.readFileSync(templatePath, 'utf-8')
	const bundle = require('../dist/vue-ssr-server-bundle.json')
	// The client manifests are optional, but it allows the renderer
	// to automatically infer preload/prefetch links and directly add <script>
	// tags for any async chunks used during render, avoiding waterfall requests.
	const clientManifest = require('../dist/vue-ssr-client-manifest.json')
	if (publicPath) {
		clientManifest.publicPath = publicPath;
	}
	renderer = createRenderer(bundle, {
		template,
		clientManifest
	})
} else {
	// In development: setup the dev server with watch and hot-reload,
	// and create a new renderer on bundle / index template update.
	readyPromise = require('../frontend/build/setup-dev-server')(
		app,
		templatePath,
		(bundle, options) => {
			renderer = createRenderer(bundle, options)
		}
	)
}

const serve = (path, cache) => express.static(resolve(path), {
	maxAge: cache && isProd ? 1000 * 60 * 60 * 24 * 30 : 0
})

app.use(compression({ threshold: 0 }))
// app.use(favicon(resolve('../dist/static/favicons/favicon-48x48.png')))
app.use('/', serve('../dist/', true))
app.use('/static', serve('../dist/static', true))
app.use('/manifest.json', serve('../dist/manifest.json', true))
app.use('/service-worker.js', serve('../dist/service-worker.js'))
app.use('/iisnode_log', express.static(resolve('./iisnode_log')));

// proxy everything dev related - HMR + when inspecting node with Chrome Dev Tools
app.use([
	'/json',
	'/__webpack_hmr',
	/(.*)\.hot-update\.json$/
], proxy({
	target: '/',
}));

if (process.env.API_DOMAIN) {
	app.use([
		/^\/umbraco(?!\/dialogs\/Preview\.aspx)/,
		'/media',
		'/App_Data/',
		'/App_Browsers/',
		'/App_Plugins/',
		'/bin/',
		'/config/',
		'/css/',
		'/install/',
		'/Umbraco_Client/',
		`/${pkg.project.id}/`,
	], proxy({
		target: process.env.API_DOMAIN,
		changeOrigin: true,
		ws: true,
	}));
}

// 1-second microcache.
// https://www.nginx.com/blog/benefits-of-microcaching-nginx/
const microCache = LRU({
	max: 100,
	maxAge: 1000
})

// since this app has no user-specific content, every page is micro-cacheable.
// if your app involves user-specific content, you need to implement custom
// logic to determine whether a request is cacheable based on its url and
// headers.
const isCacheable = req => useMicroCache

function render(req, res) {
	res.setHeader("Content-Type", "text/html")

	const handleError = (err) => {
		const response = (err.response) ? err.response : err;
		if (response.data && response.data.data && response.data.data.url) {
			console.info(`${response.status} redirect ${req.url} --> ${response.data.data.url}`);
			res.redirect(response.status, response.data.data.url);
		} else if (response.status === 404) {
			// 404 Page (usually dataApi returns 200 OK with an actual "Page Not Found" page.
			// This is a fallback for when the api doesn't respond with one).
			res.status(404).send('404 | Page Not Found');
			SkyLogError.log({
				origin: `${req.protocol}://${req.get('host')}`,
				host: req.get('host'),
				message: `404 | ${response.statusText || 'Page Not Found'}\r\nReceived no "Page Not Found" page from api. ${err.message || ''}`,
				url: `${req.protocol}://${req.get('host')}${req.originalUrl}`,
				userAgent: req.headers['user-agent'],
				errorOrigin: 'node',
				error: err
			});
		} else {
			res.status(500).send('500 | Internal Server Error')
			// Error Page
			SkyLogError.log({
				origin: `${req.protocol}://${req.get('host')}`,
				host: req.get('host'),
				message: `500 | ${response.statusText || 'Internal Server Error'}\r\n${err.message || ''}`,
				url: `${req.protocol}://${req.get('host')}${req.originalUrl}`,
				userAgent: req.headers['user-agent'],
				errorOrigin: 'node',
				error: err
			});
		}
	}

	const cacheable = isCacheable(req)
	if (cacheable) {
		const hit = microCache.get(req.url)
		if (hit) {
			if (!isProd) {
				console.log(`Server.js: Served ${req.url} from cache!`);
			}
			return res.send(hit)
		}
	}

	const context = {
		publicPath,
		url: req.originalUrl,
		origin: `${req.protocol}://${req.get('host')}`,
		host: req.get('host'),
		href: `${req.protocol}://${req.get('host')}${req.originalUrl}`,
		userAgent: req.headers['user-agent'],
		webp: req.headers.accept && req.headers.accept.indexOf('image/webp') !== -1,
	};

	renderer.renderToString(context, (err, html) => {
		if (err) {
			return handleError(err)
		} else {
			if (context.state.currentStatus === 404) {
				res.status(404).send(html)
			} else {
				res.send(html)
			}

			if (cacheable) {
				microCache.set(req.url, html)
			}
		}
	})
}

app.get('/robots.txt', (req, res) => {
	res.type('text/plain');
	res.send('User-agent: *\nDisallow: /iisnode_log');
});
app.get('/favicon.ico', (req, res) => {
	res.status(204).send();
});
app.get('*', (req, res) => {
	if (/^\/static($|\/)/.test(req.url)) {
		// unfound assets under /static should trigger simple 404,
		// not boot up the app, ask the api etc.
		res.status(404).send();
	} else {
		isProd
			? render(req, res)
			: readyPromise.then(() => render(req, res));
	}
});

const port = process.env.PORT || 8080
app.listen(port, () => {
	console.log(`server started at localhost:${port}`)

	if (process.env.API_DOMAIN) {
		console.log('connected to:')
		console.log('---', `API_DOMAIN: ${process.env.API_DOMAIN}`)
		console.log('---', `API_HOST: ${process.env.API_HOST}`)
	}

	console.log(`Use ${chalk.bold('Ctrl+C')} to stop it`)
})