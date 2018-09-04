const pkg = require('../../package.json');
const path = require('path');
const webpack = require('webpack');
const chalk = require('chalk');
const merge = require('webpack-merge');
const createBase = require('./webpack.base.config');
const nodeExternals = require('webpack-node-externals');
const VueSSRServerPlugin = require('vue-server-renderer/server-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');


console.log(chalk.underline('server') + ' build in progress...');
console.log('');

module.exports = merge(createBase(true), {
	target: 'node',
	entry: [
		'./index.template.html',
		'./polyfills.js',
		'./entry-server.js',
	],
	output: {
		filename: 'server-bundle.js',
		libraryTarget: 'commonjs2',
	},
	// https://webpack.js.org/configuration/externals/#externals
	// https://github.com/liady/webpack-node-externals
	externals: nodeExternals({
		whitelist: [
			// do not externalize CSS files in case we need to import it from a dep
			/\.(css|scss)$/,
			// do not externalize "<style src="dep/foo.css">" in .vue files in case we need to import from dep
			/\?vue&type=style/,
		],
	}),
	plugins: [
		new webpack.DefinePlugin({
			'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV || 'development'),
			'process.env.VUE_ENV': '"server"',
			'process.browser': false,
			'process.env.SKY_PROJECT_ID': JSON.stringify(pkg.project.id),
			'process.env.SKY_PROJECT_NAME': JSON.stringify(pkg.project.name),
		}),
		new VueSSRServerPlugin(),
		new CleanWebpackPlugin([
			'static/backoffice/',
		], {
			root: path.resolve(__dirname, '../Dev/web/' + pkg.project.id + '/'),
			verbose: false,
		}),
		new CopyWebpackPlugin([
			{
				context: '../src/svgs/icons/',
				from: '**/*',
				to: '../../Dev/web/' + pkg.project.id + '/static/backoffice/'
			},
		], {
			copyUnmodified: true,
		}),
	],
});
