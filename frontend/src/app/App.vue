<script>
import meta from './meta';
import accessibility from './accessibility';

export default {
	name: 'app',
	mixins: [meta, accessibility],
	computed: {
		site() {
			return this.$store.getters.site;
		},
		currentTemplate() {
			return this.$store.getters.currentTemplate;
		},
	},
};
</script>

<template src="./App.html"></template>
<style src="./App.scss"></style>
