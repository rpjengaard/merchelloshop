<script>
export default {
	data() {
		return {
			percent: 0,
			show: false,
			canSuccess: true,
			duration: 3000,
			height: '2px',
			color: '#00aeee',
			failedColor: '#e42b2b',
		};
	},
	computed: {
		styleObject() {
			return {
				width: `${this.percent}%`,
				height: this.height,
				backgroundColor: this.canSuccess ? this.color : this.failedColor,
				opacity: this.show ? 1 : 0,
			};
		},
	},
	methods: {
		start() {
			this.show = true;
			this.canSuccess = true;

			if (this._timer) {
				clearInterval(this._timer);
				this.percent = 0;
			}

			this._cut = 10000 / Math.floor(this.duration);

			this._timer = setInterval(() => {
				this.increase(this._cut * Math.random());
				if (this.percent > 95) {
					this.finish();
				}
			}, 100);

			return this;
		},
		set(num) {
			this.show = true;
			this.canSuccess = true;
			this.percent = Math.floor(num);
			return this;
		},
		get() {
			return Math.floor(this.percent);
		},
		increase(num) {
			this.percent = this.percent + Math.floor(num);
			return this;
		},
		decrease(num) {
			this.percent = this.percent - Math.floor(num);
			return this;
		},
		finish() {
			this.percent = 100;
			this.hide();
			return this;
		},
		pause() {
			clearInterval(this._timer);
			return this;
		},
		hide() {
			clearInterval(this._timer);
			this._timer = null;

			setTimeout(() => {
				this.show = false;
				this.$nextTick(() => {
					setTimeout(() => {
						this.percent = 0;
					}, 200);
				});
			}, 500);

			return this;
		},
		fail() {
			this.canSuccess = false;
			return this;
		},
	},
	mounted() {
		this.$store.commit('PROGRESS_BAR_MOUNT', {
			get: this.get,
			set: this.set,
			start: this.start,
			finish: this.finish,
			pause: this.pause,
			fail: this.fail,
			hide: this.hide,
		});
	},
};
</script>

<template src="./ProgressBar.html"></template>
<style src="./ProgressBar.scss"></style>
