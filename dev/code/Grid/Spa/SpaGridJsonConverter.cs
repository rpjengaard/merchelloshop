using System;
using System.Linq;
using code.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Values;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Grid.Spa
{
    public class SpaGridJsonConverter : JsonConverter
    {
        private object GetMediaValue(GridControlMediaValue value)
        {
            // Just return null if the value is null
            if (value == null) return null;

            // Get the media from the cache (and return null if not found)
            IPublishedContent media = UmbracoContext.Current.MediaCache.GetById(value.Id);
            if (media == null) return null;

            return new
            {
                id = media.Id,
                url = media.Url,
                cropUrl = media.GetCropUrl().SupportWebP(),
                name = media.Name
            };
        }
        
        private string GetRteParsedValue(GridControlRichTextValue value)
        {
            if (value == null) return null;

            return Umbraco.Web.Templates.TemplateUtilities.ParseInternalLinks(value.Value, UmbracoContext.Current.UrlProvider);
        }

        private object GetControl(GridControl control)
        {
            object value;
            switch (control.Editor.Alias)
            {
                case "media":
                case "media_wide":
                    value = GetMediaValue(control.GetValue<GridControlMediaValue>());
                    break;
                case "rte":
                    value = GetRteParsedValue(control.GetValue<GridControlRichTextValue>());
                    break;
				// put more value-parsers / converters here
                default:
                    // Avoid returning too much data by default
                    return new JObject {
                        {"hest", control.Editor.Alias },
                        {"value", control.Value?.GetType().Name }
                    };
            }

            return new
            {
                value,
                editor = GetEditor(control.Editor)
            };
        }

        private object GetEditor(GridEditor editor)
        {
            // Other properties are ommitted since I'm not sure we need them for SPA solutions (including the config)
            return new
            {
                alias = editor.Alias,
            };
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            GridDataModel grid = value as GridDataModel;
            if (grid == null) return;

	        var hai = new
	        {
		        name = grid.Name,
		        sections = from section in grid.Sections
			        select new
			        {
				        grid = section.Grid,
				        rows = from row in section.Rows
					        select new
					        {
						        id = row.Id,
						        name = row.Name,
						        styles = row.Styles.JObject,
						        config = row.Config.JObject,
						        areas = from area in row.Areas
							        select new
							        {
								        grid = area.Grid,
								        //allowAll = area.AllowAll,
								        //allowed = area.Allowed,
								        config = area.Config.JObject,
								        styles = area.Styles.JObject,
								        controls = from control in area.Controls
									        select GetControl(control)
							        }
					        }
			        }
	        };

            JObject obj = JObject.FromObject(hai);

            obj.WriteTo(writer);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}