<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	computed: {
		locations() {
			return this.$_get(this, 'content.value.items');
		},
		settings() {
			return this.$_get(this, 'content.value.settings');
		},
	},
};
</script>

<template src="./Map.html" />
<style src="./Map.scss" />
