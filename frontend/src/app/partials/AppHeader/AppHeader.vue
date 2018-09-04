<script>
export default {
	name: 'AppHeader',
	computed: {
		site() {
			return this.$store.getters.site;
		},
	},
};
</script>

<style src="./AppHeader.scss"></style>
<template src="./AppHeader.html"></template>
