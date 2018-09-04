<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	computed: {
		facts() {
			return this.$_get(this, 'content.value.items');
		},
	},
};
</script>

<style src="./Facts.scss" />
<template src="./Facts.html" />
