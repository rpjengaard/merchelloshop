<script>
import Props from '../mixins/props.mixin';
import Core from '../mixins/core.mixin';
import Expand from '../mixins/expand.mixin';


export default {
	name: 'Hover',
	mixins: [Core, Props, Expand],
	watch: {
		hovering(value, oldValue) {
			if (oldValue === -1) {
				this.listenForClickOutside();
			}
		},
	},
	methods: {
		// This makes sure clicks outside hover nav closes it.
		// Primarily useful on touch devices.
		listenForClickOutside() {
			window.addEventListener('click', this.onWindowClick);
		},
		onWindowClick(event) {
			if (!this.$el.contains(event.target)) {
				this.toggleFocus(-1);
				window.removeEventListener('click', this.onWindowClick);
			}
		},
	},
};
</script>

<style src="./Hover.scss" />
<template src="./Hover.html" />
