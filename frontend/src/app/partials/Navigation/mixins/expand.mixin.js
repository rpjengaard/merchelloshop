export default {
	data() {
		return {
			hovering: -1,
		};
	},
	methods: {
		isOpen(id) {
			if (this.type === 'hover') {
				return true;
			}
			if (this.type === 'foldable') {
				return this.open.indexOf(id) > -1;
			}
			return this.levels > 1 && this.contentPath.indexOf(id) !== -1;
		},
		isHovering(id) {
			return id === this.hovering;
		},
		toggleOpen(id) {
			const index = this.open.indexOf(id);
			if (index > -1) {
				this.open.splice(index, 1);
			} else {
				this.open.push(id);
			}
		},
		toggleFocus(item) {
			if (item && item.id && item.hasChildren) {
				this.$set(this, 'hovering', item.id);
			} else {
				this.$set(this, 'hovering', -1);
			}
		},
		showNextLevel(item) {
			return item.hasChildren && this.isOpen(item.id);
		},
	},
};
