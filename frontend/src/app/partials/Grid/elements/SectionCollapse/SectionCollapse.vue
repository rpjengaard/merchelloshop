<script>
export default {
	props: {
		content: {
			type: Object,
		},
	},
	data() {
		return {
			id: Math.random().toString(35).substr(2, 5),
		};
	},
};
</script>

<style src="./SectionCollapse.scss" />
<template src="./SectionCollapse.html" />
