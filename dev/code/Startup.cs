using System;
using code.EventHandlers;
using code.Indexers;
using Skybrud.Umbraco.Search.Indexers;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;
using ContentDefaultValues = Skybrud.Umbraco.Module.EventHandlers.ContentDefaultValues;

namespace code
{
    public class Startup : IApplicationEventHandler
    {
        private static object _lock = new object();
        private static bool _started = false;

        private static ContentDefaultValues _defaultNewsValues;
        private static ContentDefaultValues _defaultValues;

        private static Skybrud.Umbraco.Module.Indexers.ExamineLciIndexer _examineLciIndexer;
		private static ExamineDateRangeIndexer _examineDateRangeIndexer;
		private static ExamineDefaultIndexer _examineDefaultIndexer;
		private static ExamineIndexer _examineIndexer;

		private static AssetsWatcher _assetsWatcher;
        public static Guid ContentChangesGuid;
        public static Guid JsChangesGuid;


        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (_started)
                return;

            lock (_lock)
            {
                if (!_started)
                {
                    _started = true;

					//Adding Custom GridConverter
					//GridContext.Current.Converters.Add(new RgGridConverter());

					_examineIndexer = new ExamineIndexer();                                                 //Register events for examine
					_examineDefaultIndexer = new ExamineDefaultIndexer();									// adds path_search + hideFromSearch to examine
					_examineLciIndexer = new Skybrud.Umbraco.Module.Indexers.ExamineLciIndexer();           //LowerCase Indexer
					_examineDateRangeIndexer = new ExamineDateRangeIndexer(new[] { Constants.SkyConstants.Properties.ContentDate });
					_defaultNewsValues = new ContentDefaultValues();										//Default newsdate defaultvalue
                    _defaultValues = new ContentDefaultValues();

					_assetsWatcher = new AssetsWatcher();                                                   //Watches pt. scripts-folder, to handle any js updates
                    ContentChangesGuid = Guid.NewGuid();                                                    //Sets guid to handle changes in Umbracontent, to update frontend NG-cache
                    JsChangesGuid = Guid.NewGuid();                                                         //Sets guid to handle changes in scripts-folder
                }
            }
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
	        PropertyValueConvertersResolver.Current.RemoveType<NestedContentManyValueConverter>();
        }

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
			Lucene.Net.Analysis.StopAnalyzer.ENGLISH_STOP_WORDS_SET = new System.Collections.Hashtable();
		}
    }
}
