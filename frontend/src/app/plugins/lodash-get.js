import _get from 'lodash.get';

export default {
	install(Vue) {
		Vue.prototype.$_get = (obj, pathToRequestedProp, defaultValue) =>
			_get(obj, pathToRequestedProp, defaultValue);
	},
};
