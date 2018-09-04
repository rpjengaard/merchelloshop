<script>
export default {
	props: {
		href: {
			required: true,
		},
		fallback: {
			type: String,
			required: false,
			default: 'span',
		},
		disabled: Boolean,
		target: String,
	},
	computed: {
		internalLink() {
			return /^\/(?!\/)/.exec(this.href) !== null
				&& this.target !== '_blank';
		},
		targetString() {
			if (!this.internalLink) {
				return '_blank';
			}
			return this.target || '';
		},
		tag() {
			if (!this.href || this.disabled) {
				return this.fallback;
			}
			if (this.internalLink) {
				return 'router-link';
			}
			return 'a';
		},
		isActive() {
			return this.$route.path.indexOf(this.href) === 0;
		},
		isActiveExact() {
			return this.$route.path === this.href;
		},
	},
	render(createElement) {
		switch (this.tag) {
		case 'router-link':
			return createElement(
				'router-link',
				{
					class: ['spa-link'],
					attrs: {
						title: this.title,
					},
					props: {
						to: this.href,
					},
				},
				this.$slots.default,
			);
		case 'a':
			return createElement(
				'a',
				{
					class: ['spa-link'],
					attrs: {
						href: this.href,
						target: this.targetString,
						rel: this.targetString === '_blank' ? 'noopener noreferrer' : '',
					},
				},
				this.$slots.default,
			);
		default:
			return createElement(
				this.fallback,
				{
					class: ['spa-link', {
						'router-link-active': this.isActive,
						'router-link-exact-active': this.isActiveExact,
					}],
				},
				this.$slots.default,
			);
		}
	},
};
</script>
