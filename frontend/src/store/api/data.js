import qs from 'qs';
import axios from 'axios';
import SkyLogError from 'SkyLogError';

const api = {
	host: process.env.API_HOST || '',
	domain: process.env.API_DOMAIN || '',
	endpoint: '/umbraco/api/spa/GetData',
	latestRequestUrl: '',
};

const axiosInstance = axios.create({
	paramsSerializer(p) {
		return qs.stringify(p, { encode: true });// {arrayFormat: 'brackets'}),
	},
});


function setDomain(origin) {
	if (process.env.API_DOMAIN) {
		api.domain = process.env.API_DOMAIN;
	} else if (typeof window !== 'undefined'
		&& window.location.origin) {
		api.domain = window.location.origin;
	} else {
		api.domain = origin;
	}
}

function setHost(host) {
	if (process.env.API_HOST) {
		api.host = process.env.API_HOST;
	} else {
		api.host = host;
	}
}

function setLatestRequest(url) {
	console.info(url);
	api.latestRequestUrl = url;
	SkyLogError.setLatestRequest(url);
}

axiosInstance.interceptors.request.use((config) => {
	const fullReqUrl = `${api.domain}${api.endpoint}?${config.paramsSerializer(config.params)}`;
	setLatestRequest(fullReqUrl);
	return config;
}, (error) => {
	setLatestRequest('API request failed on init');
	return Promise.reject(error);
});

function get(params, entry) {
	setHost(entry.host);
	setDomain(entry.origin);
	const baseParams = { appHost: api.host };
	const requestParams = Object.assign(baseParams, params);

	return axiosInstance.get(`${api.domain}${api.endpoint}`, {
		params: requestParams,
	});
}

export default {
	get,
	get latestRequest() {
		return api.latestRequestUrl;
	},
};
