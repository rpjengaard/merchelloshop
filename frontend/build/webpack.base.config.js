const pkg = require('../../package.json');
const path = require('path');
const chalk = require('chalk');
const gitlog = require('gitlog');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const NodeNotifier = require('node-notifier');
const VersionFile = require('webpack-version-file');
const SimpleProgressPlugin = require('webpack-simple-progress-plugin');
const FriendlyErrorsPlugin = require('friendly-errors-webpack-plugin');

const isProduction = process.env.NODE_ENV === 'production';
const root = path.resolve(__dirname, '../../');
const paths = {
	dist: path.resolve(root, 'dist'),
	frontend: path.resolve(root, 'frontend'),
	src: path.resolve(root, 'frontend/src'),
	fonts: path.resolve(root, 'frontend/src/fonts'),
	modules: path.resolve(root, 'frontend/src/modules'),
	static: path.resolve(root, 'frontend/src/static'),
	styles: path.resolve(root, 'frontend/src/styles'),
	svgs: path.resolve(root, 'frontend/src/svgs'),
};

let commits = [];
try {
	commits = gitlog({
		repo: root,
		number: 1,
		fields: [
			'abbrevHash',
			'subject',
			'authorDate',
		],
	});
} catch(e) {}

const whiteList = [
	paths.src,
	path.resolve('frontend/build/'),
	path.resolve('SkyLogError.js'),
	path.resolve('node_modules/sky-accordion'),
	path.resolve('node_modules/sky-crop'),
	path.resolve('node_modules/sky-scroll'),
	path.resolve('node_modules/sky-gallery'),
	path.resolve('node_modules/sky-fonts'),
	path.resolve('node_modules/sky-form'),
	path.resolve('node_modules/sky-lightbox'),
	path.resolve('node_modules/sky-list'),
	path.resolve('node_modules/sky-mailchimp'),
	path.resolve('node_modules/sky-overlay'),
	path.resolve('node_modules/sky-reveal'),
	path.resolve('node_modules/sky-sharethis'),
	path.resolve('node_modules/sky-sticky'),
	path.resolve('node_modules/sky-svg'),
	path.resolve('node_modules/sky-swiper'),
	path.resolve('node_modules/sky-video'),
	path.resolve('node_modules/sky-window'),
	path.resolve('node_modules/vue2-google-maps'),
	path.resolve('node_modules/vue-intersect'),
];

// Suppress webpack plugin deprication warnings in production
// Note: Note if these appear in dev and make sure to update deps
// https://github.com/webpack/webpack/issues/6568
process.noDeprecation = isProduction;

console.log('');
console.log('BUILDING ' + chalk.bgRedBright(' ' + pkg.project.id + ' '));
console.log(pkg.name + ' v' + pkg.version);

function createBase(isServer = false) {
	const config = {
		// choose appropriate source map setting based on your needs
		// https://webpack.js.org/configuration/devtool/
		devtool: isProduction ? '(none)' : 'cheap-source-map',
		context: paths.src,
		output: {
			path: paths.dist,
			publicPath: '/',
			filename: isProduction
				? '[name].[contenthash].js'
				: '[name].js',
			chunkFilename: isProduction
				? '[name].[contenthash].js'
				: '[name].js',
		},
		mode: isProduction ? 'production' : 'development',
		optimization: {
			minimize: isProduction,
		},
		parallelism: 4,
		watchOptions: {
			aggregateTimeout: 500,
			ignored: /node_modules/,
		},
		resolve: {
			modules: [
				root,
				paths.modules,
				paths.src,
				'node_modules',
			],
			alias: {
				'~': root,
				'@': path.resolve(root, 'node_modules'),
				'static': paths.static,
				'styles': paths.styles,
				'fonts': paths.fonts,
				'svgs': paths.svgs,
			},
			extensions: [
				'.js',
				'.json',
				'.vue',
			],
		},
		module: {
			rules: [
				{
					test: /\.vue$/,
					include: whiteList,
					use: [
						'vue-loader',
					],
				},
				{
					test: /\.js$/,
					include: whiteList,
					use: isProduction
						? ['babel-loader']
						: [
							{
								loader: 'cache-loader',
								options: {
									cacheDirectory: path.resolve('node_modules/.cache-loader'),
								},
							},
							{
								loader: 'babel-loader',
								options: {
									cacheDirectory: true,
								},
							},
						],
				},
				{
					test: /\.(css|scss)$/,
					include: whiteList,
					use: (isServer && isProduction) // use null loader server-side in production (extracted css doesn't work there)
						? [
							'null-loader',
						]
						: [
							(isProduction)
								? MiniCssExtractPlugin.loader
								: {
									loader: 'vue-style-loader',
									options: {
										sourceMap: !isProduction,
									},
								},
							{
								loader: 'css-loader',
								options: {
									sourceMap: !isProduction,
								},
							},
							{
								loader: 'postcss-loader',
								options: {
									ident: 'postcss',
									sourceMap: !isProduction,
									plugins: [
										require('postcss-inline-svg')({
											path: paths.src,
											removeFill: true,
										}),
										require('cssnano')({ safe: true }),
										require('css-mqpacker')(),
										require('autoprefixer')(),
									],
								},
							},
							{
								loader: 'sass-loader',
								options: {
									sourceMap: !isProduction,
								},
							},
						],
				},
				{
					test: /\.(png|jpg|gif|svg|mp4|webm)$/,
					exclude: [
						/node_modules/,
						paths.svgs,
						paths.fonts,
					],
					use: [
						{
							// Url-loader inlines data-url of files below limit (10kb)
							loader: 'url-loader',
							options: {
								limit: 10000,
								name: '[path][name].[ext]?[hash]',
							},
						},
					],
				},
				{
					test: /\.(eot|svg|ttf|woff|woff2)$/,
					use: {
						loader: 'file-loader',
						options: {
							name: '[path][name].[ext]',
						},
					},
					exclude: [
						paths.svgs,
						paths.static,
					],
				},
				{
					test: /\index\.template\.html$/,
					use: [
						{
							loader: 'file-loader',
							options: {
								name: '[path][name].[ext]',
							},
						},
						{
							loader: 'extract-loader',
						},
						{
							loader: 'html-loader',
							options: {
								minimize: true,
								caseSensitive: true,
								removeComments: false,
								ignoreCustomFragments: [/\{\{\{.*?}}}/],
							},
						},
					],
				},
				{
					test: /manifest\.json$/,
					loader: 'file-loader',
					exclude: /node_modules/,
					options: {
						name: '[path][name].[ext]',
					},
				},
			],
		},
		plugins: [
			new VueLoaderPlugin(),
			new FriendlyErrorsPlugin({
				onErrors: (severity, errors) => {
					if (severity !== 'error') {
						return;
					}
					const error = errors[0];
					const errorFile = error.file
						.split('./')
						.slice(-1)[0];
					NodeNotifier.notify({
						title: 'error',
						group: 'skyBuild',
						remove: 'skyBuild',
						subtitle: error.message || '',
						message: errorFile.length < 49 ? errorFile : '...' + errorFile.substr(-46)
					});
				},
			}),
		],
	};
	if (isProduction) {
		config.plugins = config.plugins.concat([
			new MiniCssExtractPlugin({
				// Options similar to the same options in webpackOptions.output
				// both options are optional
				filename: isProduction
					? '[name].[contenthash].css'
					: '[name].css',
				chunkFilename: isProduction
					? '[name].[contenthash].css'
					: '[name].css',
			}),
			new SimpleProgressPlugin({
				messageTemplate: [ chalk.green(':percent'), ':bar', ':msg'].join(' '),
				progressOptions: {
					complete: chalk.bgGreen(' '),
					incomplete: chalk.bgHex('#333')(' '),
					width: 30,
					total: 100,
					clear: true,
				},
			}),
			new VersionFile({
				output: './server/version.txt',
				package: './package.json',
				data: {
					commit: commits[0] || {
						abbrevHash: '',
						subject: '',
						authorDate: '',
					},
				},
				templateString: '<%= name %>@<%= version %>\r\n'
					+'\r\n'
					+'Build date: <%= buildDate %>\r\n'
					+'Project: <%= project.id %> - <%= project.name %>\r\n'
					+'\r\n'
					+'Commit hash: <%= commit.abbrevHash %>\r\n'
					+'Commit subject: <%= commit.subject %>\r\n'
					+'Commit date: <%= commit.authorDate %>\r\n'
			})
		]);
	} else {
		config.module.rules = config.module.rules.concat([
			// lint local *.vue files in development mode
			{
				enforce: 'pre',
				test: /\.(js|vue|html)$/,
				loader: 'eslint-loader',
				exclude: [
					/node_modules/,
					/\.template\.html$/,
				],
				options: {
					fix: true,
					cache: true,
				},
			},
		]);
		config.plugins = config.plugins.concat([
			new SimpleProgressPlugin({
				messageTemplate: [chalk.dim(':percent'), ':bar', ':msg'].join(' '),
				progressOptions: {
					complete: chalk.bgHex('#666').dim(' '),
					incomplete: chalk.bgHex('#333')(' '),
					width: 30,
					total: 100,
					clear: true,
				},
			}),
		]);
	}

	return config;
}


module.exports = createBase;
