module.exports = {
	extends: [
		'airbnb',
		'plugin:vue/base',
		'plugin:vue/essential',
		'plugin:vue/strongly-recommended',
		'plugin:vue/recommended',
	],
	plugins: [
		'vue',
		'import'
	],
	parser: 'vue-eslint-parser',
	parserOptions: {
		parser: 'babel-eslint',
	},
	settings: {
		'import/resolver': {
			webpack: {
				config: 'frontend/build/webpack.base.config.js',
			}
		},
		'eslint.validate': [
		    {
		    	language: 'vue',
		    	autoFix: true
		    }
		],
	},
	env: {
		browser: true,
	},
	rules: {
		indent: ['error', 'tab'],
		'no-tabs': 0,
		'no-new': 0,
		'no-console': 0,
		'no-plusplus': 0,
		'no-continue': 1,
		'no-underscore-dangle': 0,
		'no-param-reassign': 0,
		'spaced-comment': 0,
		'arrow-body-style': ['error', 'as-needed'],
		'brace-style' : ['error', '1tbs', { 'allowSingleLine': false }],
		'no-restricted-syntax': [
			'error',
			'ForOfStatement',
			'LabeledStatement',
			'WithStatement',
		],
		'linebreak-style': 0,
		'import/extensions': ['error', 'always', {
			js: 'never',
			jsx: 'never',
			json: 'never',
			vue: 'never',
		}],
		'import/no-unresolved': [2, { caseSensitive: false }],
		'import/no-named-as-default': 0,
		'import/no-named-as-default-member': 0,
		'no-mixed-operators': [
			'error',
			{
				'groups': [
					// ['+', '-', '*', '/', '%', '**'], // Let's us use different arithmics on the same line
					['&', '|', '^', '~', '<<', '>>', '>>>'],
					['==', '!=', '===', '!==', '>', '>=', '<', '<='],
					['&&', '||'],
					['in', 'instanceof']
				],
				'allowSamePrecedence': false
			}
		],
		'global-require': 0,
		'no-unused-vars': ['warn'],
		'no-unused-expressions': ['error', {
			'allowTernary': true,
		}],
		'max-len': ['error', 100, 4, {
			ignoreUrls: true,
			ignoreComments: true,
			ignoreRegExpLiterals: true,
			ignoreStrings: true,
			ignoreTemplateLiterals: true,
		}],
		'jsx-a11y/href-no-hash': 'off',
		'jsx-a11y/anchor-is-valid': ['warn', { 'aspects': ['invalidHref'] }],
		'function-paren-newline': 0,
		'prefer-destructuring': ['warn', {'object': true, 'array': false}],
		'object-curly-newline': 0,
		'prefer-promise-reject-errors': 0,
		"vue/html-indent": ["error", "tab", {
			"attribute": 1,
			"closeBracket": 0,
			"alignAttributesVertically": false,
			"ignores": []
		}],
		"vue/no-v-html": 0,
		"vue/require-default-prop": 0,
		"vue/attributes-order": 0
	},
	overrides: [
		{
			'files': [ '**/*.html' ],
			'rules': {
				'max-len': 0
			}
		}
	]
}
