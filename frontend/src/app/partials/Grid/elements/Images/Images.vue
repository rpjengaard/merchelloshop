<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	computed: {
		images() {
			return this.$_get(this, 'content.value.items');
		},
		isSingle() {
			return this.images.length === 1;
		},
		isSwiper() {
			return this.images.length >= 2 && this.images.length <= 5;
		},
		isGallery() {
			return this.images.length >= 6;
		},
	},
};
</script>

<style src="./Images.scss" />
<template src="./Images.html" />
