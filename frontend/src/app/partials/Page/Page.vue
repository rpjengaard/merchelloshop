<script>
export default {
	name: 'Page',
	props: {
		/**
		 * pid: takes a page id. Component then shows data from the
		 * given page.
		 */
		pid: {
			type: Number,
			required: true,
		},
		/**
		 * link: Whether or not the component should link to the page
		 */
		link: {
			type: Boolean,
			default: false,
		},
		/**
		 * linkQuery: Optional string to append to link as query params
		 * i.e. /my/link?myParam=true
		 */
		linkQuery: {
			type: String,
			required: false,
			default: '',
		},
		/**
		 * field: Which page prop to use as content (if no slotted
		 * content given)
		 */
		field: {
			type: String,
			required: false,
			default: 'title',
		},
		/**
		 * fallback: The node type that is used if link is false
		 */
		fallback: {
			type: String,
			required: false,
			default: 'span',
		},
	},
	computed: {
		pageInfo() {
			return this.$store.getters.pageTree_item(this.pid);
		},
		pageUrl() {
			return this.$_get(this, 'pageInfo.url');
		},
		href() {
			if (this.link && this.pageUrl) {
				return (this.linkQuery.length)
					? `${this.pageUrl}?${this.linkQuery}`
					: this.pageUrl;
			}
			return null;
		},
		className() {
			return this.link
				? 'page-link'
				: 'page-info';
		},
	},
};
</script>

<template src="./Page.html" />
