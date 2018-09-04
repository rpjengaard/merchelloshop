using code.Extensions;
using Newtonsoft.Json;
using Skybrud.UmbracoEssentials.Extensions.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace code.Models.Website.Common {

    public class SpaImage {

        [JsonIgnore]
        public IPublishedContent Content { get; }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; }

        /// <summary>
        /// Gets the URL of the image.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the crop URL of the image.
        /// </summary>
        [JsonProperty("cropUrl")]
        public string CropUrl { get; }

        #region Constructors

        protected SpaImage(IPublishedContent content) {
            Content = content;
            Width = content.GetInt32(Umbraco.Core.Constants.Conventions.Media.Width);
            Height = content.GetInt32(Umbraco.Core.Constants.Conventions.Media.Height);
            Url = content.Url.SupportWebP();
            CropUrl = content.GetCropUrl(Width, Height, imageCropMode: ImageCropMode.Crop, preferFocalPoint: true).SupportWebP();
        }

        #endregion

        #region Member methods

        public string GetCropUrl(int width, int height) {
            return Content.GetCropUrl(width, height, imageCropMode: ImageCropMode.Crop, preferFocalPoint: true).SupportWebP();
        }

        #endregion

        #region Static methods

        public static SpaImage GetFromContent(IPublishedContent content) {
            return content == null ? null : new SpaImage(content);
        }

        #endregion

    }

}