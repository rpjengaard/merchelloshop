import Vue from 'vue';
import store from 'store';

const SkyLogError = require('SkyLogError');

function formatComponentName(vm) {
	if (vm.$root === vm) {
		return 'root';
	}
	const name = vm._isVue ? vm.$options.name || vm.$options._componentTag : vm.name;
	const componentName = name ? `<${name}>` : 'unknown';
	return componentName;
}

Vue.config.errorHandler = (error, vm, info) => {
	const isServer = process.env.VUE_ENV === 'server';
	// Get store
	const vuexStore = store.get();
	// Get server side entry details from store
	const entry = (vuexStore && vuexStore.state && vuexStore.state.entry)
		? vuexStore.state.entry
		: null;
	// Get route details (for fallback purposes)
	const route = (vuexStore && vuexStore.state && vuexStore.state.route)
		? vuexStore.state.route
		: null;
	const dataApi = (vuexStore && vuexStore.state && vuexStore.state.dataApi)
		? vuexStore.state.dataApi
		: null;

	const errorMeta = {
		host: isServer
			? entry.host
			: window.location.host,
		origin: isServer
			? entry.origin
			: window.location.origin,
		url: isServer
			? entry.href || route.fullPath || ''
			: window.location.href,
		userAgent: isServer ? entry.userAgent : window.navigator.userAgent,
		errorOrigin: 'app',
		message: error.message,
		dataApi: dataApi.latestRequest,
		error,
	};

	// vm and lifecycleHook are not always available
	if (Object.prototype.toString.call(vm) === '[object Object]') {
		errorMeta.component = formatComponentName(vm);
	}
	if (typeof info !== 'undefined') {
		errorMeta.lifecycleHook = info;
	}

	SkyLogError.log(errorMeta);
};
