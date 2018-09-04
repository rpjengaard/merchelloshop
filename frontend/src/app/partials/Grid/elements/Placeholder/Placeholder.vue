<script>
export default {
	props: {
		content: {
			type: Object,
		},
		id: {
			type: String,
		},
		name: {
			type: String,
		},
	},
	computed: {
		isDev() {
			return process.env.NODE_ENV === 'development';
		},
		alias() {
			return this.$_get(this.content, 'editor.alias') || this.name || this.id;
		},
	},
};
</script>

<template src="./Placeholder.html" />
<style src="./Placeholder.scss" />
