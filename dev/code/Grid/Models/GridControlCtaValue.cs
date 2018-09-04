using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.LinkPicker;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.LeBlender.Values;

namespace code.Grid.Models
{
	public class GridControlCtaValue : SkyGridControlValueBase
	{
		#region Properties
		[JsonProperty("headline")]
		public string Headline { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("link")]
		public LinkPickerItem Link { get; set; }

		#endregion


		#region Constructors

		public GridControlCtaValue(GridControl control) : base(control)
		{
			var blenderCtrl = GridControlLeBlenderValue.Parse(control);
			if (blenderCtrl == null || blenderCtrl.Items == null) return;
			var item = blenderCtrl.Items.First();

			Headline = item.GetRawValue("headline");

			Text = item.GetRawValue("text");

			Link = !string.IsNullOrEmpty(item.GetRawValue("link"))
				? LinkPickerList.Parse(JObject.Parse(item.GetRawValue("link"))).Items.FirstOrDefault()
				: null;
		}

		#endregion


		#region Static methods

		public static GridControlCtaValue Parse(GridControl control)
		{
			return new GridControlCtaValue(control);
		}

		public override string GetSearchableText()
		{
			var combined = new StringBuilder();

			combined.AppendLine(Headline);
			combined.AppendLine(Text);

			return combined.ToString();
		}

		#endregion
	}
}
