<script>
export default {
	name: 'Subpage',
	props: ['content'],
	serverCacheKey: (props) => {
		if (props.content && props.content.id && props.content.updated) {
			return `${props.content.id}::${props.content.updated}`;
		}
		return '';
	},
};
</script>

<template src="./Subpage.html"></template>
<style src="./Subpage.scss"></style>
