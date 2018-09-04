export default {
	props: {
		/**
		 * Data accepts an array of nav objects. Used for custom
		 * navigation such as the main menu and footer links.
		 */
		data: {
			type: Array,
			required: false,
			default: undefined,
		},
		/**
		 * Entry takes a page id. Component then shows child pages
		 * of given page.
		 */
		entry: {
			type: Number,
			required: false,
			default: -1,
		},
		/**
		 * Levels sets number of nested navigation levels
		 */
		levels: {
			type: Number,
			required: false,
			default: 99,
		},
		/**
		 * Type
		 * Available types: flat/normal | hover | foldable | nested
		 */
		type: {
			type: String,
			default: 'flat',
		},
		/**
		 * Slots
		 * Reference to this.$slots - used in "type".vue e.g. hover.vue
		 */
		slots: Object,
	},
};
