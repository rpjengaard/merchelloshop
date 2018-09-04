const axios = require('axios');
const pkg = require('./package');
const serializeError = require('serialize-error');

let latestRequest = '';

function postError(errorMsg, domain) {
	return axios.get(`${domain}/umbraco/api/logging/SkyLogError`, {
		params: {
			errorMsg,
		},
	});
}

function stringifyKeys(keys, srcObj) {
	if (Array.isArray(keys)) {
		return keys
			.reduce((acc, key) => {
				acc += `\r\n${key}: ${srcObj[key] || 'unknown'}`;
				return acc;
			}, '');
	}
	return null;
}

function toString(inputObj) {
	const obj = Object.assign({
		timestamp: `${new Date().toJSON().toString()} (UTC)`,
		vueEnv: process.env.VUE_ENV,
		dataApi: latestRequest,
	}, inputObj);

	const host = obj.host || `${pkg.project.id} - ${pkg.project.name}`;

	const mandatory = [
		'host',
		'origin',
		'timestamp',
		'url',
		'dataApi',
		'message',
	];

	const rest = Object.keys(obj)
		.filter(key => mandatory.indexOf(key) === -1 && key !== 'error')
		.sort((a, b) => {
			if (a.toUpperCase() < b.toUpperCase()) {
				return -1;
			}
			if (a.toUpperCase() > b.toUpperCase()) {
				return 1;
			}
			return 0;
		});

	let stack = '';
	if (obj.error) {
		const serializedError = serializeError(obj.error);
		if (serializedError.stack) {
			stack = stringifyKeys('stack', serializedError.stack);
		}
	}

	const stringifiedKeys = stringifyKeys(rest, obj);

	return `\r\n*${host}*\r\n${obj.timestamp}\r\n${obj.url}\r\n${obj.message}\r\n${obj.dataApi || 'no API url'}\r\n\`\`\`${stringifiedKeys}\r\n\r\n${stack}\`\`\``;
}

const SkyLogError = {
	log: (errMeta) => {
		if (errMeta) {
			const errorString = toString(errMeta);
			const notLocal = errMeta.host && /\.\w*$/.exec(errMeta.host); // host is not local, ie 'localhost', 'rpjengaard' etc.
			// Only post to error api if live and userAgent is not Slackbot
			const shouldPost = notLocal && errMeta.userAgent.indexOf('Slack') === -1;
			if (shouldPost) {
				postError(errorString, process.env.API_DOMAIN || errMeta.origin);
			} else {
				console.error(errorString);
			}
		}
	},
	setLatestRequest: (reqUrl) => {
		latestRequest = reqUrl;
	},
};

if (typeof module === 'object' && module.exports) {
	module.exports = SkyLogError;
}
