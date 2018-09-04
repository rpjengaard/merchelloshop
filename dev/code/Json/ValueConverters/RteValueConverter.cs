using System;
using System.Collections.Concurrent;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace code.Json.ValueConverters
{
	public class RteValueConverter : IPropertyValueConverterMeta
	{
		private readonly IDataTypeService _dataTypeService;

		private static readonly ConcurrentDictionary<int, bool> Storages = new ConcurrentDictionary<int, bool>();

		 public RteValueConverter() : this(ApplicationContext.Current.Services.DataTypeService)
        {
        }

        private RteValueConverter(IDataTypeService dataTypeService)
        {
            if (dataTypeService == null) throw new ArgumentNullException("dataTypeService");
            _dataTypeService = dataTypeService;

        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Umbraco.TinyMCEv3");

        }

        public object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            return source;
        }

        public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            return source.ToString();
        }

        public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            return null;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof(string);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            PropertyCacheLevel propertyCacheLevel;
            switch (cacheValue)
            {
                case PropertyCacheValue.Source:
                    propertyCacheLevel = PropertyCacheLevel.Content;
                    break;
                case PropertyCacheValue.Object:
                    propertyCacheLevel = PropertyCacheLevel.ContentCache;
                    break;
                case PropertyCacheValue.XPath:
                    propertyCacheLevel = PropertyCacheLevel.Content;
                    break;
                default:
                    propertyCacheLevel = PropertyCacheLevel.None;
                    break;
            }
            return propertyCacheLevel;
        }
	}
}
