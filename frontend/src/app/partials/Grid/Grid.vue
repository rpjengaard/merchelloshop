<script>
import Placeholder from './elements/Placeholder';
import AccordionGrid from './elements/Accordion';
import CtaButtonGrid from './elements/CtaButton';
import FactsGrid from './elements/Facts';
import IframeGrid from './elements/Iframe';
import ImagesGrid from './elements/Images';
import MapGrid from './elements/Map';
import QuoteGrid from './elements/Quote';
import RteGrid from './elements/Rte';
import SectionCollapseGrid from './elements/SectionCollapse';
import VideoGrid from './elements/Video';

const gridComponents = {
	Placeholder,
	AccordionGrid,
	CtaButtonGrid,
	FactsGrid,
	IframeGrid,
	ImagesGrid,
	MapGrid,
	QuoteGrid,
	RteGrid,
	SectionCollapseGrid,
	VideoGrid,
};

const gridMap = {};

export default {
	name: 'Grid',
	components: gridComponents,
	props: ['grid'],
	data() {
		return {
			missingElements: [],
		};
	},
	created() {
		if (process.env.NODE_ENV === 'development') {
			this.grid.sections.forEach((section) => {
				section.rows.forEach((row) => {
					row.areas.forEach((area) => {
						area.controls.forEach((control) => {
							if (this.$_get(control, 'editor.alias')) {
								const name = this.componentName(control.editor.alias);
								const notFound = !Object.keys(gridComponents).includes(name);
								const notListed = !this.missingElements.includes(name);

								if (notFound && notListed) {
									this.missingElements.push(name);
								}
							}
						});
					});
				});
			});

			this.missingElements.forEach((element) => {
				console.warn(`Couldn't find component '${element}' in 'partials/Grid/elements/'`);
			});
		}
	},
	methods: {
		getCurrentElementClass(type, array, index) {
			const types = {
				row: (_array, _index) => {
					const string = _array[_index].areas.length > 1
						? `columns columns-${_array[_index].areas.length}`
						: `${this.componentName(_array[_index].areas[0].controls[0].editor.alias)}`;

					return string;
				},
				area: (_array, _index) => {
					const string = _array.length > 1
						? `${type}-${_array[_index].grid} ${this.componentName(_array[_index].controls[0].editor.alias)}`
						: `${type}-${_array[_index].grid}`;

					return string;
				},
			};

			return `${type} ${types[type](array, index)}`;
		},
		getClassNames(type, array, index) {
			const nextGridElement = this.nextGridElement(array, index);
			const previousGridElement = this.previousGridElement(array, index);
			const currentClass = this.getCurrentElementClass(type, array, index);

			return `${currentClass} ${nextGridElement} ${previousGridElement}`;
		},
		getAlias(source) {
			return this.$_get(source, 'controls[0].editor.alias')
				|| this.$_get(source, 'areas[0].controls[0].editor.alias');
		},
		aliasArray(array) {
			const aliasArr = [];
			array.forEach((item) => {
				aliasArr.push(this.getAlias(item));
			});

			return aliasArr;
		},
		componentName(alias) {
			if (!alias) {
				return '';
			}
			let returnName = `${alias.substr(0, 1).toUpperCase() + alias.substr(1)}Grid`;

			if (Object.keys(gridComponents).indexOf(returnName) === -1) {
				returnName = gridMap[returnName] || returnName;
			}

			return returnName;
		},
		nextGridElement(currentArray, currentIndex) {
			if (currentIndex === (currentArray.length - 1)) {
				return 'last';
			}

			const nextObject = currentArray[currentIndex + 1];
			const nextIsMultiColumn = this.$_get(nextObject, 'areas.length') > 1;

			if (nextIsMultiColumn) {
				return `next-columns-${nextObject.areas.length}`;
			}

			const alias = this.aliasArray(currentArray)[currentIndex + 1];

			return alias && this.componentName(alias)
				? `next-${this.componentName(alias)}`
				: '';
		},
		previousGridElement(currentArray, currentIndex) {
			if (currentIndex === 0) {
				return 'first';
			}

			const previousObject = currentArray[currentIndex - 1];
			const previousIsMultiColumn = this.$_get(previousObject, 'areas.length') > 1;

			if (previousIsMultiColumn) {
				return `previous-columns-${previousObject.areas.length}`;
			}

			const alias = this.aliasArray(currentArray)[currentIndex - 1];

			return alias && this.componentName(alias)
				? `previous-${this.componentName(alias)}`
				: '';
		},
	},
};
</script>

<template src="./Grid.html" />
<style src="./Grid.scss" />
