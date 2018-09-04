<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	computed: {
		link() {
			return this.$_get(this, 'content.value.link');
		},
	},
};
</script>

<style src="./CtaButton.scss" />
<template src="./CtaButton.html" />
