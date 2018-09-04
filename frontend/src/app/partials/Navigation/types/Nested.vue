<script>
import Props from '../mixins/props.mixin';
import Core from '../mixins/core.mixin';

export default {
	name: 'Nested',
	mixins: [Core, Props],
	data() {
		return {
			navigation: {
				path: [],
				parts: [],
			},
			selectedAction: null,
		};
	},
	computed: {
		nestedBack() {
			return this.$slots.default.find(slot => slot.data.slot === 'nestedBack');
		},
		animationDirection() {
			if (this.selectedAction === 'next') {
				return 'forward';
			}

			if (this.selectedAction === 'previous') {
				return 'backward';
			}

			return null;
		},
		parentPageId() {
			const fetchIndex = (this.currentPage.path.length - 2) < 0
				? 0
				: this.currentPage.path.length - 2;

			return this.currentPage.path[fetchIndex];
		},
		atRoot() {
			return this.navigation.path.length <= 1;
		},
	},
	beforeMount() {
		// Called after "beforeMount" in coresSetup.mixin.

		this.navigation.parts.push(this.getCachedChildren(this.parentPageId));
		this.navigation.path = this.contentPath.slice(0, this.contentPath.length - 1);
	},
	methods: {
		getTitleFromId(id) {
			return this.$_get(this.$store.getters.pageTree_item(id), 'title') || '';
		},
		removePreviousNavigation() {
			this.navigation.parts.pop();
		},
		cleanupNavigationPath(index) {
			this.navigation.path.splice(index);
		},
		fetchNavPart(parentId) {
			this.$store
				.dispatch('FETCH_NAVIGATION', parentId)
				.then((res) => {
					if (this.$_get(res, 'children.length')) {
						this.navigation.parts
							.unshift(this.$_get(res, 'children'));
					}
				});
		},
		nextLevel(id) {
			this.selectedAction = 'next';
			this.navigation.path.push(id);

			this.fetchNavPart(id);
		},
		goBack(parentId) {
			this.selectedAction = 'previous';

			this.cleanupNavigationPath(this.navigation.path.indexOf(parentId) + 1);

			const previousParentIndex = this.navigation.path.length - 1;
			const previousParentId = this.navigation.path[previousParentIndex];

			this.fetchNavPart(previousParentId);
		},
		previousLevel(currentLevelParent) {
			this.selectedAction = 'previous';

			this.cleanupNavigationPath(this.navigation.path.indexOf(currentLevelParent));

			const previousParentIndex = this.navigation.path.length - 1;
			const previousParentId = this.navigation.path[previousParentIndex];

			this.fetchNavPart(previousParentId);
		},
	},
};
</script>

<style src="./Nested.scss" />
<template src="./Nested.html" />
