import Vue from 'vue';

// Require all svgs in folder with vue-svg-loader which gives us
// pre-compiled render functions
const allSvgs = require.context(
	'!!vue-svg-loader?{"svgo":{"plugins":[{"cleanupIDs":true},{"mergePaths":false},{"removeStyleElement": true}]}}!svgs/',
	true,
	/\.svg$/,
);

// Export object of svg components
export default allSvgs
	.keys()
	.reduce((obj, key) => {
		// Create name of component by finding filename, prepending
		// 'svg-' and transforming all non alpha characters to '-'
		const componentName = key
			.replace('./', 'svg-')
			.split('.svg')
			.join('')
			.replace(/\W+?/g, '-')
			.toLowerCase()
			.replace(/-([a-z])/g, (m, w) => w.toUpperCase())
			.replace(/^(.)/, (m, w) => w.toUpperCase());
		// Create and globally register component with name
		const component = Vue.component(componentName, {
			extends: allSvgs(key),
		});
		obj[componentName] = component;
		return obj;
	}, {});
