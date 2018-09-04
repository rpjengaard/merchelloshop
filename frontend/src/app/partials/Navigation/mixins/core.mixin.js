export default {
	data() {
		return {
			items: [],
			open: [],
			loading: true,
			cached: false,
		};
	},
	computed: {
		currentPage() {
			return this.$store.getters.content_cached(this.$route.path)
				|| this.$store.getters.currentPage;
		},
		contentPath() {
			return this.currentPage.path || [];
		},
		pageId() {
			if (this.entry === -1 || !this.entry) {
				return this.contentPath[1] || this.currentPage.id;
			}
			return this.entry;
		},
		pageHasChildren() {
			const currentNode = this.$store.getters.pageTree_item(this.pageId);
			if (currentNode) {
				return currentNode.hasChildren;
			}

			return false;
		},
	},
	methods: {
		isActive(item) {
			return this.contentPath.indexOf(item.id) !== -1;
		},
		isCurrent(item) {
			return this.currentPage.id === item.id;
		},
		setItems(items) {
			this.items.length > 0
				? this.$set(this, 'items', [])
				: this.$set(this, 'items', items);

			this.loading = false;
		},
		getCachedChildren(requestId) {
			return this.$store.getters.pageTree_childrenOf(requestId);
		},
		fetchThisLevel() {
			return this.$store.dispatch('FETCH_NAVIGATION', this.pageId);
		},
		fetchNextLevel() {
			this.items.forEach((item) => {
				if (item.hasChildren && item.children.length === 0) {
					this.$store.dispatch('FETCH_NAVIGATION', item.id);
				}
			});
		},
	},
	created() {
		const cached = this.getCachedChildren(this.pageId);

		if (this.data) {
			this.setItems(this.data);
		} else if (cached.length) {
			this.setItems(cached);
			this.cached = true;
		} else if (!this.pageHasChildren) {
			this.loading = false;
		}

		this.$set(this, 'open', this.contentPath);
	},
	beforeMount() {
		// When component mounts, and we try to render child nodes that we
		// do not have fetched (cached) yet, we fetch them.
		if (!this.data
			&& !this.cached
			&& this.pageId !== -1
			&& this.pageHasChildren
		) {
			this.fetchThisLevel()
				.then(() => {
					this.setItems(this.getCachedChildren(this.pageId));
				})
				.catch((err) => {
					console.error('Could not render Navigation component. No navigation data received from API.', err);
				});
			this.$set(this, 'open', this.contentPath.slice(0));
		}
	},
	mounted() {
		if (this.type === 'hover') {
			// If the navigation type of our component is 'hover' we know
			// for sure that we need to iterate through all its child nodes,
			// and fetch their children since they might soon be rendered.
			this.fetchNextLevel();
		} else if ('requestIdleCallback' in window) {
			// If browser supports requestIdleCallback fetch child nodes anyway
			// - just for good measure.
			window.requestIdleCallback(this.fetchNextLevel);
		}
	},
};
