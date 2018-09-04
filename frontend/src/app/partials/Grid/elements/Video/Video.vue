<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	computed: {
		video() {
			return this.$_get(this, 'content.value.items[0]');
		},
		title() {
			return this.video.title || this.$_get(this.video, 'details.title');
		},
		description() {
			return this.video.description;
		},
		thumbnail() {
			const userThumbnail = this.$_get(this.video, 'thumbnail.cropUrl');

			if (userThumbnail) {
				return userThumbnail;
			}

			const nativeThumbnail = this.$_get(this.video, 'details.thumbnails');
			if (nativeThumbnail) {
				return nativeThumbnail[nativeThumbnail.length - 1].url;
			}

			return null;
		},
	},
};
</script>

<style src="./Video.scss" />
<template src="./Video.html" />
