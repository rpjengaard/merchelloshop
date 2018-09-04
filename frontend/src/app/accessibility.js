export default {
	data() {
		return {
			focusList: [
				'button',
				'input',
				'textarea',
				'a',
			],
			// HTMLElement that redelagate focus: Array of HTMLElement types that could be focused
			reFocusList: {
				label: ['input', 'textarea', 'select'],
			},
		};
	},
	mounted() {
		this.$el.addEventListener('mousedown', this.onMouseDown);
	},
	beforeDestroy() {
		this.$el.removeEventListener('mousedown', this.onMouseDown);
	},
	methods: {
		getFocusTarget(element) {
			const elementName = element.tagName.toLowerCase();

			if (this.focusList.indexOf(elementName) !== -1) {
				return element;
			}

			if (Object.keys(this.reFocusList).indexOf(elementName) !== -1) {
				let foundElement = null;

				this.reFocusList[elementName].forEach((target) => {
					if (!foundElement) {
						foundElement = element.parentNode.querySelector(target);
					}
				});

				return foundElement;
			}

			return null;
		},
		onMouseDown(e) {
			const target = this.getFocusTarget(e.target);
			if (target) {
				setTimeout(() => {
					target.style.outline = 'none';
				}, 0);

				target.style.outline = 'none';
				target.addEventListener('blur', this.onBlur);
			}
		},
		onBlur(e) {
			// Blur event is triggered on window blur too - only stop
			// supressing focus styles if window in focus
			if (document.hasFocus()) {
				e.target.style.outline = '';
				e.target.removeEventListener('blur', this.onBlur);
			}
		},
	},
};
