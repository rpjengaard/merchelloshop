import Vue from 'vue';
import Vuex from 'vuex';
import state from './state';
import actions from './actions';
import mutations from './mutations';
import getters from './getters';

Vue.use(Vuex);
const pendingModules = [];

let _store = {};

// We expose a factory function for creating a new store on each
// new app instance (specifically for server-side rendering, where
// we need a fresh store on each request)
export function createStore() {
	_store = new Vuex.Store({
		state,
		actions,
		mutations,
		getters,
	});

	pendingModules.forEach(({ name, store }) => {
		_store.registerModule(name, store);
	});
	pendingModules.length = 0;

	return _store;
}

createStore();

export default {
	get() {
		return _store;
	},
	addModule(name, store) {
		pendingModules.push({ name, store });
	},
};
