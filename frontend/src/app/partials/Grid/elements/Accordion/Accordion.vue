<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	methods: {
		setVideoThumbnail(videoObject) {
			const userThumbnail = this.$_get(videoObject, 'thumbnail.cropUrl');

			if (videoObject.HasThumbnail && userThumbnail) {
				return userThumbnail;
			}

			const nativeThumbnail = this.$_get(videoObject, 'details.Thumbnails');
			if (nativeThumbnail) {
				return nativeThumbnail[nativeThumbnail.length - 1].Url;
			}

			return null;
		},
	},
	computed: {
		value() {
			return this.$_get(this, 'content.value');
		},
	},
};
</script>

<style src="./Accordion.scss" />
<template src="./Accordion.html" />
