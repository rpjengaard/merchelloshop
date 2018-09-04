<script>
export default {
	name: 'Breadcrumb',
	props: {
		/**
			* Entry level controls which nested level breadcrumb
			* begins on - ie. 0 = from site root
			*/
		entryLevel: {
			type: Number,
			required: false,
			default: 1,
		},
		/**
			* Levels sets max number of levels breadcrumb shows
			*/
		maxLevels: {
			type: Number,
			required: false,
			default: 99,
		},
		path: {
			type: [Boolean, Array],
			default: false,
		},
	},
	data() {
		return {
			items: [],
		};
	},
	computed: {
		currentPage() {
			return this.$store.getters.content_cached(this.$route.path)
				|| this.$store.getters.currentPage;
		},
	},
	created() {
		// const ids = this.currentPage.path.slice(this.entryLevel, this.maxLevels);
		const targetPath = this.path !== false
			? this.path
			: this.currentPage.path;

		const ids = targetPath.slice(this.entryLevel, this.maxLevels);

		this.$set(this, 'items', this.$store.getters.pageTree_item(ids));
	},
	methods: {
		isCurrent(item) {
			return this.currentPage && this.currentPage.id === item.id;
		},
		localStripHtml(string) {
			const returnValue = string.replace(/&#(\d+);/g, (match, dec) => String.fromCharCode(dec));
			return returnValue.replace(/<[^>]*>/g, ' ').replace(/&shy;/g, '');
		},
	},
};
</script>

<style src="./Breadcrumb.scss" />
<template src="./Breadcrumb.html" />
