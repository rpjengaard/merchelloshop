<script>
export default {
	name: 'AppFooter',
	computed: {
		site() {
			return this.$store.getters.site;
		},
	},
};
</script>

<style src="./AppFooter.scss"></style>
<template src="./AppFooter.html"></template>
