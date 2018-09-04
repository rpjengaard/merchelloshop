using Newtonsoft.Json;

namespace code.Models.Skybrud.Base
{
    public class BaseItem
    {
        #region Properties

        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }

        #endregion

        #region Constructors

        

        #endregion
    }
}
