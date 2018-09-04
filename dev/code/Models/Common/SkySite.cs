using Umbraco.Core.Models;

namespace code.Models.Common
{
    public class SkySite
    {
        #region Properties

		public IPublishedContent Content { get; set; }

        #endregion

        #region Constructors

        private SkySite(IPublishedContent content)
        {
			Content = content;
		}

        #endregion

        #region Statics

        public static SkySite GetFromContent(IPublishedContent content)
        {
            return content == null ? null : new SkySite(content);
        }

        #endregion
    }
}
