using Newtonsoft.Json;

namespace code.Models.Spa.Site {

    public class SpaSiteFooter {

        #region Properties

        /// <summary>
        /// Gets a reference to the parent site.
        /// </summary>
        [JsonIgnore]
        public SpaSiteModel Site { get; }
        
        #endregion

        #region Constructors

        public SpaSiteFooter(SpaSiteModel site) {
            Site = site;
        }

        #endregion

    }

}