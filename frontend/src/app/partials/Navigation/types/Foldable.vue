<script>
import Props from '../mixins/props.mixin';
import Core from '../mixins/core.mixin';
import Expand from '../mixins/expand.mixin';


export default {
	name: 'Foldable',
	mixins: [Core, Props, Expand],
};
</script>

<style src="./Foldable.scss" />
<template src="./Foldable.html" />
