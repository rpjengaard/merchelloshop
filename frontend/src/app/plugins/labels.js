export default {
	install(Vue) {
		Vue.mixin({
			computed: {
				$labels() {
					const { site } = this.$store.getters;
					if (site && site.labels) {
						return site.labels;
					}
					return {};
				},
			},
		});
	},
};
