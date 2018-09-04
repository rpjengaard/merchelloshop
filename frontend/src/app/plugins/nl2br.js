export default {
	install(Vue) {
		Vue.directive('nl2br', (el, binding) => {
			if (binding.value) {
				el.innerHTML = binding.value.replace(/(?:\r\n|\r|\n)/g, '<br />');
			}
		});
	},
};
