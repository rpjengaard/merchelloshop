<script>
import Vue from 'vue';

/**
 * Full VueGoogleMap implementation
 */
// import * as VueGoogleMaps from '~/node_modules/vue2-google-maps/src/main';


/**
 * Partial VueGoogleMap implementation
 */
import { load as VueGoogleMaps } from '~/node_modules/vue2-google-maps/src/manager';
import GmapMap from '~/node_modules/vue2-google-maps/src/components/Map';
import GmapMarker from '~/node_modules/vue2-google-maps/src/components/Marker';
import GmapInfoWindow from '~/node_modules/vue2-google-maps/src/components/infoWindow';

import MapStyling from './mapStyle';

// API Dokumentation til Vue2GoogleMaps:
// https://github.com/xkjyeah/vue-google-maps/blob/dc12b55da8c0cc272cb91e75886ccb03b7c37aca/API.md#options

/**
 * Full VueGoogleMap implementation
 */
// Vue.use(VueGoogleMaps, {
// 	load: {
// 		key: 'AIzaSyDUgSLyDSnQ1lxWLN6gvsl8JYXpTv02yVI',
// 		libraries: 'places', // This is required if you use the Autocomplete plugin
// 	},
// });
/** END: Full implementation */


/**
 * Partial VueGoogleMap implementation
 */
VueGoogleMaps({
	key: 'AIzaSyDUgSLyDSnQ1lxWLN6gvsl8JYXpTv02yVI',
	libraries: 'places', // This is required if you use the Autocomplete plugin
});

const defaultResizeBus = new Vue();
Vue.$gmapDefaultResizeBus = defaultResizeBus;
Vue.mixin({
	created() {
		this.$gmapDefaultResizeBus = defaultResizeBus;
	},
});
/** END: Partial implementation */


export default {
	// Add components when using partial VueGoogleMap implementation
	components: {
		GmapMap,
		GmapMarker,
		GmapInfoWindow,
	},
	props: {
		locations: Array,
		settings: Object,
	},
	data() {
		return {
			// icon: {
			// 	path: 'M 100, 100 m -75, 0 a 75,75 0 1,0 150,0 a 75,75 0 1,0 -150,0',
			// 	fillColor: '#24b549',
			// 	fillOpacity: 1,
			// 	strokeColor: '#fff',
			// 	strokeOpacity: 0,
			// 	strokeWeight: 0,
			// 	anchor: { x: -50, y: 50 }, // relative to viewport size
			// 	scale: 0.25,
			// },
			infoWindowPos: null,
			infoWinOpen: false,
			currentMarkerIndex: null,
			infoOptions: {
				pixelOffset: {
					width: 0,
					height: -42,
				},
			},
			options: {
				mapTypeControl: false,
				zoomControl: true,
				scaleControl: true,
				scrollwheel: false,
				draggable: true,
				streetViewControl: false,
				fullscreenControl: false,
				clustering: false,
				styles: MapStyling,
			},
		};
	},
	computed: {
		currentMarker() {
			return this.$_get(this.locations, this.currentMarkerIndex);
		},
		zoom() {
			return this.$_get(this, 'settings.zoom') || 10;
		},
		center() {
			const centerLat = this.$_get(this, 'settings.centerLat');
			const centerLng = this.$_get(this, 'settings.centerLng');
			return {
				lat: centerLat ? Number(centerLat) : 56.4599753,
				lng: centerLng ? Number(centerLng) : 10.0283903,
			};
		},
	},
	methods: {
		getPosition(marker) {
			return {
				lng: Number(marker.lng),
				lat: Number(marker.lat),
			};
		},
		clickOnMap(event) {
			console.log(event);
		},
		toggleInfoWindow(marker, index) {
			this.infoWindowPos = this.getPosition(marker);

			if (this.currentMarkerIndex === index) {
				// check if its the same marker that was selected if yes toggle
				this.infoWinOpen = !this.infoWinOpen;
			} else {
				// if different marker set infowindow to open and reset current marker index
				this.infoWinOpen = true;
				this.currentMarkerIndex = index;
			}
		},
		closeInfoWindow() {
			if (this.currentMarkerIndex > -1) {
				this.infoWinOpen = false;
				this.currentMarkerIndex = -1;
			}
		},
	},
};
</script>

<template src="./GoogleMaps.html" />
<style src="./GoogleMaps.scss" />
