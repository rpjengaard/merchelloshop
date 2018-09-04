<script>
export default {
	name: 'Frontpage',
	props: ['content'],
	serverCacheKey: (props) => {
		if (props.content && props.content.id && props.content.updated) {
			return `${props.content.id}::${props.content.updated}`;
		}
		return '';
	},
};
</script>

<style src="./Frontpage.scss"></style>
<template src="./Frontpage.html"></template>
