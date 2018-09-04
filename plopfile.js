// Add node file system
const fs = require('fs');

// Require pkg for us to read useful information
// Like all our projects
const pkg = require('./package.json');
const promptDirectory = require('inquirer-directory');

module.exports = function(plop) {
	plop.setPrompt('directory', promptDirectory);

	/**
	 * Generates a Vue Component
	 *
	 * Prompts for:
	 * - Directory
	 * - If a store is needed or not
	 * - Component name
	 */
	plop.setGenerator('Vue component', {
		description: 'Generates a Vue component',
		prompts: [
			{
				type: 'input',
				name: 'name',
				message: 'Name your component:',
			},
			{
				type: 'directory',
				name: 'directory',
				message: 'Choose parent directory',
				basePath: './frontend/src',
			},
			{
				type: 'confirm',
				name: 'seperate',
				message: 'Add component in its own folder?',
				default: true,
			},
			{
				type: 'confirm',
				name: 'store',
				message: 'Add store?',
				default: false,
			},
		],
		actions(data) {
			const actions = [];
			const templatePath = 'frontend/misc/plop-templates/vue';
			const componentPath = (data.seperate)
				? 'frontend/src/{{directory}}/{{properCase name}}'
				: 'frontend/src/{{directory}}';

			// If component has a store
			// add all files from store template folder
			if (data.store) {
				actions.push({
					type: 'addMany',
					destination: componentPath,
					base: templatePath,
					templateFiles: `${templatePath}/store/*.js`,
				});
			}

			// Add index.js if in seperate folder
			if (data.seperate) {
				actions.push({
					type: 'add',
					path: `${componentPath}/index.js`,
					templateFile: `${templatePath}/index.hbs`,
				});
			}

			// Files for all components
			actions.push({
				type: 'add',
				path: `${componentPath}/{{properCase name}}.vue`,
				templateFile: `${templatePath}/component.hbs`,
			});

			actions.push({
				type: 'add',
				path: `${componentPath}/{{properCase name}}.html`,
				templateFile: `${templatePath}/template.hbs`,
			});

			actions.push({
				type: 'add',
				path: `${componentPath}/{{properCase name}}.scss`,
				templateFile: `${templatePath}/styles.hbs`,
			});

			return actions;
		},
	});

	/**
	 * Generates a vue plugin
	 *
	 * Prompts for:
	 * - Directory
	 * - Service name
	 */
	plop.setGenerator('Vue plugin', {
		description: 'Generates a Vue plugin',
		prompts: [
			{
				type: 'input',
				name: 'name',
				message: 'Name your Vue plugin:',
			},
			{
				type: 'directory',
				name: 'directory',
				message: 'Choose parent directory',
				basePath: './frontend/src',
			},
		],
		actions(data) {
			const actions = [];
			const templatePath = 'frontend/misc/plop-templates/vue-plugin';
			const destination = 'frontend/src/{{directory}}';

			actions.push({
				type: 'add',
				path: `${destination}/{{properCase name}}/index.js`,
				templateFile: `${templatePath}/index.hbs`,
			});

			return actions;
		},
	});

	/**
	 * Generates a service
	 *
	 * Prompts for:
	 * - Directory
	 * - Service name
	 */
	plop.setGenerator('js module', {
		description: 'Generates a simple js module',
		prompts: [
			{
				type: 'input',
				name: 'name',
				message: 'Name your module:',
			},
			{
				type: 'directory',
				name: 'directory',
				message: 'Choose parent directory',
				basePath: './frontend/src',
			},
		],
		actions(data) {
			const actions = [];
			const templatePath = 'frontend/misc/plop-templates/js-module';
			const destination = 'frontend/src/{{directory}}';

			actions.push({
				type: 'add',
				path: `${destination}/{{properCase name}}/index.js`,
				templateFile: `${templatePath}/index.hbs`,
			});

			return actions;
		},
	});
};
