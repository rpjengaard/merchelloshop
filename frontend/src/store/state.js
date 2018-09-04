export default {
	site: {
		siteRootId: -1,
		siteName: '',
		mainNavigation: [],
	},
	/**
	 * siteChanged
	 * Triggered by culture/language change
	 */
	siteChanged: true,
	entry: {
		url: '',
		host: '',
	},
	dataApi: {
		latestRequest: '',
	},
	currentPage: {},
	currentTemplate: '',
	/**
	 * STATUS
	 * Property for telling nodejs express server whether or not currentPage is a 404 page
	 * (a bit hacky - takes advantage of the fact that vue-server-renderer has access to store
	 * on its context)
	 */
	currentStatus: 200,
	cache: {
		/**
		 * PAGE TREE CACHE
		 * For storing objects that hold meta data such as title, url, id for pages as
		 * well as ids for child pages
		 */
		pageTree: {
			items: {
				/* [id: string]: Page Tree Node */
			},
			urls: {},
			/**
			 * Max items pageTree is able to contain before cache is invalidated (and thereby
			 * cleared on next route change).
			 */
			limit: 5000,
			invalid: false,
		},
		/**
		 * CONTENT CACHE
		 * For storing objects that hold page data such as title, url, id for pages as
		 * well as ids for child pages
		 */
		content: {
			items: {
				/* [url: string]: Page Content */
			},
			/**
			 * Max number of pages content cache will keep (because of localStorage size limit)
			 */
			limit: 50,
			/**
			 * Content history is used to determine which pages were visited the longest while
			 * back when cache limit is exceeded (and therefore should be dumped first)
			 */
			history: [
				/* [url: string] */
			],
			/**
			 * Content guid used to invalidate client-side cache when CMS is updated
			 */
			guid: '',
			/**
			 * SiteId also used to invalidate client-side cache when (sub)site is changed
			 */
			siteId: -1,
			invalid: false,
		},
		/**
		 * Whether cache is invalidated. If valid = false cache will refresh from scratch
		 * on next route change
		 */
		// valid: true,
	},
	progressBar: {},
	preTransition: {
		name: 'default',
		active: false,
		promise: null,
		ignore: false,
		to: null,
	},
	/**
	 * Solution specific below (non-spa related)
	 */
};
