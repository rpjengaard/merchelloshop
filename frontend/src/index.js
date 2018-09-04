// NPM IMPORTS
// import Vue from 'vue';
// import SkyScroll from '@/sky-scroll/src';
// import SkyReveal from '@/sky-reveal/src/';
// import SkyAccordion from '@/sky-accordion/src/';

// IMPORT COOKIE BANNER
// import 'CookieBanner';

// MODULE IMPORTS
// import 'GoogleMaps';

import createApp from './app';
import fonts from './fonts';

fonts();

export default () => {
	const { app, store, router } = createApp();

	// INIT PLUGINS HERE
	// Vue.use(SkyScroll);
	// Vue.use(SkyReveal, { registerComponents: false }); // Use only reveal in relevant componenets.
	// Vue.use(SkyAccordion, { registerComponents: false }); // Use only accordion in relevant componenets.

	return { app, store, router };
};
