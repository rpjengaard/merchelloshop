
The Startup class must at least contain the following:
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
