import Vue from 'vue';
import Router from 'vue-router';
import View from './views/View';

Vue.use(Router);

export default function createRouter() {
	return new Router({
		mode: 'history',
		fallback: true,
		scrollBehavior(to, from, savedPosition) {
			if (savedPosition) {
				return savedPosition;
			}
			return { x: 0, y: 0 };
		},
		routes: [
			{ path: '*', component: View },
		],
	});
}
