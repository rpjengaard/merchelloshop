# Skybrud.Umbraco.Search

A package for searching the ExamineIndex in Umbraco



### Setup

The Startup class must at least contain the following:

```C#
public class Startup : IApplicationEventHandler

{

    	private static readonly object _lock = new object();
    	private static bool _started;
    
    	/* Interesting bits here */
        private static Skybrud.Umbraco.Search.Indexers.ExamineLciIndexer _examineLciIndexer;
    	private static ExamineDateRangeIndexer _examineDateRangeIndexer;
    	private static ExamineDefaultIndexer _defaultIndexer;
    	/* End of interesting bits */

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (_started)
                return;
    
            lock (_lock)
            {
                if (!_started)
                {
                    _started = true;
    
    				/* Interesting bits here */
                    _examineLciIndexer = new Skybrud.Umbraco.Search.Indexers.ExamineLciIndexer();
    				_examineDateRangeIndexer = new ExamineDateRangeIndexer(new[] { /* date fields to index */ });
    				_defaultIndexer = new ExamineDefaultIndexer();
    				/* End of interesting bits */
                }
            }
        }
        
    	public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
    		/* Interesting bits here */
            Lucene.Net.Analysis.StopAnalyzer.ENGLISH_STOP_WORDS_SET = new System.Collections.Hashtable();
    		/* End of interesting bits */
        }
    }

}

```



### Example

```c#
[System.Web.Http.HttpGet]
public object Search(string keywords = "", int offset = 0, int limit = 10, string doctypes = "", string fields = "")
{
SearchOptions so = new SearchOptions
{
  Text = keywords,
    RootId = 1060,
    DocumentTypes = string.IsNullOrWhiteSpace(doctypes)
    ? new[] {"umbHomePage", "umbNewsItem", "umbNewsOverview", "umbTextPage"}
  : doctypes.Split(','),
    Fields = string.IsNullOrWhiteSpace(fields)
    ? Fields.GetFromStringArray(new[] {"nodeName_lci", "title_lci", "teaser_lci"})
    : Fields.GetFromStringArray(fields.Split(',')),
      DateRange = new DateRange
    {
      FieldName = "createDate",
        Start = DateTime.Now.AddDays(-6),
        End = DateTime.Now
    },
      Order = new Order
    {
      FieldName = "nodeName_lci",
        OrderDirection = OrderDirection.Ascending,
        OrderType = OrderType.String
    },
      DisableHideFromSearch = true,
      Debug = true
};
  try
  {
    if (so.DocumentTypes.Any())
    {
      return
        Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Der er ikke angivet dokumenttype der skal søges i."));
    }

    int total = 0;

    IEnumerable<IPublishedContent> results = SkybrudSearch.SearchDocuments(out total, limit, offset, so);

    return
      Request.CreateResponse(JsonMetaResponse.GetSuccessFromIEnumerable(results,
                                                                        SiteSearchResult.GetFromContent, offset, limit));
  }
  catch (Exception ex)
  {
    LogHelper.Error<SiteSearchApiController>("", ex);
    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Something went wrong. If debug is on, check the tracelog"));
  }
}

```

#### Options:

1. `Text`: The text to search for.
2. `Offset`: How many results to skip.
3. `Limit`: How many results to return.
4. `RootIds`: The roots from where to search.
5. `DocumentTypes`: The document types to search in.
6. `ExamineIndex`: Specify alternate Examine index. Default `"ExternalSearcher"`.
7. `Fields`: Basically a list of the type `Field`  which contains a `Fieldname` to search in and optional `Boost` and `Fuzz` values for augmenting the field.
8. `Order`: Specify a `FieldName` and select an `OrderType` (order by string, int or search score) and `OrderDirection`.
9. `DateRange`: Specify a `FieldName` with a DateTime value (default is `"createDate"`), a `Start` and an `End` time to search in that range.
10.  `SpeceficFields`: Supply a `Dictionary<string, string[]>` where the `string` is a fieldName and the `string[]` contains search terms. `JoinType` determines how the search terms will be joined.
11. `CustomRawSearchString`: A string that will be appended to the search query. Must be valid Lucene syntax.
12. `DisableHideFromSearch`: By default, the search only includes results with the field `hideFromSearch:(0)"`, set this to true to disable that.
13. `Debug`: If set to `true`, the constructed query will be outputted in the UmbracoTraceLog.

