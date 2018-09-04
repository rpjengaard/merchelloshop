<script>
import Props from './mixins/props.mixin';

import Hover from './types/Hover';
import Flat from './types/Flat';
import Foldable from './types/Foldable';
import Nested from './types/Nested';

const navigationTypeMap = {
	normal: Flat,
	flat: Flat,
	foldable: Foldable,
	hover: Hover,
	nested: Nested,
};

export default {
	name: 'Navigation',
	mixins: [Props],
	computed: {
		navigation() {
			return navigationTypeMap[this.type.toLowerCase()];
		},
		key() {
			return `${this.type}::${this.entry}::${this.levels}::${this.data}`;
		},
	},
};
</script>

<template>
	<component
		:is="navigation"
		:data="data"
		:entry="entry"
		:levels="levels"
		:type="type"
		:key="key"
	/>
</template>

<style src="./Navigation.scss" />
