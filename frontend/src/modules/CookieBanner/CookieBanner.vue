<script>
import VueCookieAcceptDecline from 'vue-cookie-accept-decline';
/**
 * Primarily a mixin + custom styling.
 * Documentation:
 * https://github.com/promosis/vue-cookie-accept-decline
 */

export default {
	mixins: [VueCookieAcceptDecline],
	name: 'CookieBanner',
};
</script>

<!-- NOTE: Styles must be scoped to override -->
<style src="./CookieBanner.scss" scoped />
