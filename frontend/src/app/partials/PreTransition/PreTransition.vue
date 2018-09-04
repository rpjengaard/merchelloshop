<script>
export default {
	name: 'PreTransition',
	computed: {
		active() {
			return this.$store.getters.preTransition.active;
		},
	},
	methods: {
		afterEnter() {
			setTimeout(() => {
				this.$store.dispatch('PRE_TRANSITION_DONE');
			});
		},
	},
};
</script>

<style src="./PreTransition.scss"></style>
<template src="./PreTransition.html"></template>
