using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Values;

namespace code.Indexers
{
	public class ExamineHelper
	{
		public static string GetStringFromGridModel(GridDataModel grid)
		{
			StringBuilder combined = new StringBuilder();

			foreach (GridControl ctrl in grid.GetAllControls())
			{
				switch (ctrl.Editor.Alias)
				{
					case "rte":
						{
							// Get the HTML value
							string html = ctrl.GetValue<GridControlRichTextValue>().HtmlValue.ToString();

							// Strip any HTML tags so we only have text
							string text = Regex.Replace(html, "<.*?>", "");

							// Extra decoding may be necessary
							text = HttpUtility.HtmlDecode(text);

							// Now append the text
							combined.AppendLine(text);

							break;
						}



					//case "facts":
					//	{
					//		var control = ctrl.GetValue<GridControlFactsValue>();

					//		string text = control.Headline;

					//		foreach (var factItem in control.Items)
					//		{
					//			text += " " + factItem.Heading;
					//			text += " " + Regex.Replace(factItem.Text.ToString(), "<.*?>", "");
					//		}

					//		combined.AppendLine(text);

					//		break;
					//	}

				}
			}
			return combined.ToString();
		}
	}
}
