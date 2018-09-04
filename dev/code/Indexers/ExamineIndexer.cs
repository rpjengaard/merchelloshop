using System;
using System.Linq;
using Examine;
using Skybrud.Umbraco.GridData;
using Umbraco.Core.Logging;

namespace code.Indexers
{
    class ExamineIndexer
    {
        public ExamineIndexer()
        {
            var externalIndexer = ExamineManager.Instance.IndexProviderCollection["ExternalIndexer"];
            var memberIndexer = ExamineManager.Instance.IndexProviderCollection["InternalMemberIndexer"];

            externalIndexer.GatheringNodeData += externalIndexer_GatheringNodeData;
            memberIndexer.GatheringNodeData += memberIndexer_GatheringNodeData;
        }


        void externalIndexer_GatheringNodeData(object sender, IndexingNodeDataEventArgs e)
        {
			// add this if your want to make csv-properties searchable
			//MakeSearchable(e, "tags");
			//MakeSearchable(e, "filterTypes");

			if (e.Fields.ContainsKey(Constants.SkyConstants.Properties.Grid))
            {
                IndexGridDataModel(e, e.Fields[Constants.SkyConstants.Properties.Grid]);
            }

			if (e.Fields["nodeTypeAlias"] == Constants.SkyConstants.DocumentTypes.NewsPage)
			{
				DateTime newsdate;
				if (DateTime.TryParse(e.Fields[Constants.SkyConstants.Properties.ContentDate], out newsdate))
				{
					e.Fields["year"] = newsdate.Year.ToString();
				}
			}

		}

        void memberIndexer_GatheringNodeData(object sender, IndexingNodeDataEventArgs e)
        {
            //Bruges til at lave custom handlinger til et Examine index. I dette tilfælde "InternalMemberIndexer"
        }

        private void IndexGridDataModel(IndexingNodeDataEventArgs e, string propertyValue)
        {

			try
			{
				// Just return/exit now if the value doesn't look like JSON
				if (!propertyValue.StartsWith("{")) return;

				// Parse/deserialize the grid value
				GridDataModel grid = GridDataModel.Deserialize(propertyValue);

				// StringBuilder used for building the new grid value optimized for searching
				var combined = ExamineHelper.GetStringFromGridModel(grid);

				e.Fields[Constants.SkyConstants.Properties.ContentGrid] = combined;
			}
			catch (Exception ex)
			{
				LogHelper.Error<ExamineIndexer>("Exeption in IndexGridDataModel method", ex);
			}

		}


		/// <summary>
		/// Make csv-field searchable
		/// </summary>
		/// <param name="e"></param>
		/// <param name="fieldname"></param>
		private void MakeSearchable(IndexingNodeDataEventArgs e, string fieldname)
		{
			string value;
			if (e.Fields.TryGetValue(fieldname, out value)) e.Fields[fieldname + "_search"] = value.Replace(",", " ");
		}

		/// <summary>
		/// Make csv-fields with UDI´s searchable
		/// </summary>
		/// <param name="e"></param>
		/// <param name="fieldname"></param>
		private void MakeSearchableUdi(IndexingNodeDataEventArgs e, string fieldname)
		{
			if (e.Fields.ContainsKey(fieldname))
			{
				string searchFieldname = string.Format("{0}_search", fieldname);
				string fieldnameValue = e.Fields[fieldname];

				// extract Guids from UDIs
				string searchFieldValue = String.Join(" ", fieldnameValue.Split(',').Select(x => x.Split('/').Last()));

				// add new searchfield
				e.Fields.Add(searchFieldname, searchFieldValue);
			}
		}
	}
}
