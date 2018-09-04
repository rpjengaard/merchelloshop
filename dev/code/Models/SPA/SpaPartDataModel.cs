using System;
using code.Models.Spa.Navigation;
using code.Models.Spa.Site;
using code.Models.Website;
using Newtonsoft.Json;

namespace code.Models.Spa {

    public class SpaPartDataModel {
        
        #region Properties

        /// <summary>
        /// Gets the ID of the current page.
        /// </summary>
        [JsonProperty("pageId")]
        public int PageId { get; set; }

        /// <summary>
        /// Gets the ID of the current page.
        /// </summary>
        [JsonProperty("siteId")]
        public int SiteId { get; set; } // id current site

        [JsonProperty("contentGuid")]
        public Guid ContentGuid { get; set; }

        [JsonProperty("navigation", NullValueHandling = NullValueHandling.Ignore)]
        public SpaNavigationModel Navigation { get; set; }

        [JsonProperty("site", NullValueHandling = NullValueHandling.Ignore)]
        public SpaSiteModel Site { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public Master Content { get; set; }

        [JsonProperty("executeTimeMs")]
        public long ExecuteTimeMs { get; set; }

        #endregion

        #region Constructors

        [JsonConstructor]
        public SpaPartDataModel() {
            ContentGuid = Startup.ContentChangesGuid;
        }

        public SpaPartDataModel(int siteId) {
            SiteId = siteId;
            ContentGuid = Startup.ContentChangesGuid;
        }

        public SpaPartDataModel(int siteId, int id) {
            PageId = id > 0 ? id : -1;
            SiteId = siteId;
            ContentGuid = Startup.ContentChangesGuid;
        }

        #endregion

    }

}