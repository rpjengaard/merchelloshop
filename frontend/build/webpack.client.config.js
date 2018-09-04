const pkg = require('../../package.json');
const webpack = require('webpack');
const chalk = require('chalk');
const merge = require('webpack-merge');
const createBase = require('./webpack.base.config');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const SWPrecachePlugin = require('sw-precache-webpack-plugin');
const VueSSRClientPlugin = require('vue-server-renderer/client-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

const isProduction = process.env.NODE_ENV === 'production';

console.log(chalk.underline('client') + ' build in progress...');
console.log('');

const config = merge(createBase(false), {
	entry: {
		app: [
			'./public-path.js',
			'./index.template.html',
			'./polyfills.js',
			'./entry-client.js',
		],
	},
	optimization: {
		minimizer: [
			new UglifyJsPlugin({
				cache: true,
				parallel: true,
				sourceMap: isProduction, // set to true if you want JS source maps
			}),
			new OptimizeCSSAssetsPlugin({
				cssProcessorOptions: {
					safe: true,
				},
			}),
		],
		runtimeChunk: {
			name: 'manifest',
		},
		splitChunks: {
			cacheGroups: {
				styles: {
					name: 'styles',
					test: /\.css$/,
					chunks: 'all',
					enforce: true,
				},
				vendor: {
					name: 'vendor',
					test: /[\\/]node_modules[\\/]/,
					chunks: 'initial',
					enforce: true,
					priority: -20,
				},
				common: {
					name: 'common',
					minChunks: 2,
					chunks: 'async',
					minSize: 0,
				},
			},
		},
	},
	plugins: [
		// strip dev-only code in Vue source
		new webpack.DefinePlugin({
			'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV || 'development'),
			'process.env.VUE_ENV': '"client"',
			'process.browser': true,
			'process.env.SKY_PROJECT_ID': JSON.stringify(pkg.project.id),
			'process.env.SKY_PROJECT_NAME': JSON.stringify(pkg.project.name),
		}),
		new VueSSRClientPlugin(),
		new CopyWebpackPlugin([
			{
				context: '../src',
				from: 'manifest.json',
			},
			{
				context: '../src',
				from: 'static/**/*',
			},
		], {
			copyUnmodified: true,
		}),
		new BundleAnalyzerPlugin({
			analyzerMode: 'static',
			openAnalyzer: false,
			reportFilename: 'stats.html',
		}),
	],
});

if (process.env.NODE_ENV === 'production') {
	config.plugins.push(
		// auto generate service worker
		new SWPrecachePlugin({
			cacheId: pkg.project.id,
			filename: 'service-worker.js',
			minify: true,
			dontCacheBustUrlsMatching: /./,
			navigateFallback: '/',
			staticFileGlobsIgnorePatterns: [/\.map$/, /\.json$/]
			// runtimeCaching: [
			// 	{
			// 		urlPattern: '/',
			// 		handler: 'networkFirst',
			// 	},
			// ],
		})
	);
}

module.exports = config;
