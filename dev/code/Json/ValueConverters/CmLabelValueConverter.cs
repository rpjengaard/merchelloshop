namespace code.Json.ValueConverters
{
   //public class CmLabelValueConverter : IPropertyValueConverterMeta
  //  {
  //      private readonly IDataTypeService _dataTypeService;

  //      private static readonly ConcurrentDictionary<int, bool> Storages = new ConcurrentDictionary<int, bool>();


  //      public CmLabelValueConverter() : this(ApplicationContext.Current.Services.DataTypeService)
  //      {
  //      }

  //      private CmLabelValueConverter(IDataTypeService dataTypeService)
  //      {
  //          if (dataTypeService == null) throw new ArgumentNullException("dataTypeService");
  //          _dataTypeService = dataTypeService;

  //      }

  //      public bool IsConverter(PublishedPropertyType propertyType)
  //      {
  //          return propertyType.PropertyEditorAlias.Equals("CodeMonkey.Seperator");

  //      }

  //      public object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
  //      {
  //          return source;
  //      }

  //      public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
  //      {
  //          return source;
  //      }

  //      public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
  //      {
  //          return null;
  //      }

		//public Type GetPropertyValueType(PublishedPropertyType propertyType)
		//{
		//	return typeof(CmSeperator);
		//}

		//public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
  //      {
  //          PropertyCacheLevel propertyCacheLevel;
  //          switch (cacheValue)
  //          {
  //              case PropertyCacheValue.Source:
  //                  propertyCacheLevel = PropertyCacheLevel.Content;
  //                  break;
  //              case PropertyCacheValue.Object:
  //                  propertyCacheLevel = PropertyCacheLevel.ContentCache;
  //                  break;
  //              case PropertyCacheValue.XPath:
  //                  propertyCacheLevel = PropertyCacheLevel.Content;
  //                  break;
  //              default:
  //                  propertyCacheLevel = PropertyCacheLevel.None;
  //                  break;
  //          }
  //          return propertyCacheLevel;
  //      }
  //  }
}
