using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using code.Json.Helpers;
using code.Json.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace code.Json.ValueConverters
{
   public class NestedContentConverter : IPropertyValueConverterMeta
    {
        private readonly IDataTypeService _dataTypeService;

        private static readonly ConcurrentDictionary<int, bool> Storages = new ConcurrentDictionary<int, bool>();

        public NestedContentConverter()
            : this(ApplicationContext.Current.Services.DataTypeService)
        {
        }

        public NestedContentConverter(IDataTypeService dataTypeService)
        {
            if (dataTypeService == null) throw new ArgumentNullException("dataTypeService");
            _dataTypeService = dataTypeService;
        }

        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Umbraco.NestedContent");

        }

        public object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
           List<IPublishedContent> items = new List<IPublishedContent>();
            if (source != null && !source.ToString().IsNullOrWhiteSpace())
            {
                var rawValue = JsonConvert.DeserializeObject<List<object>>(source.ToString());
                 var preValueCollection = SkybrudNestedContentHelper.GetPreValuesCollectionByDataTypeId(propertyType.DataTypeId);
                
                for (var i = 0; i < rawValue.Count; i++)
                {
                    var item = (JObject)rawValue[i];

                    // Convert from old style (v.0.1.1) data format if necessary
                    // - Please note: This call has virtually no impact on rendering performance for new style (>v0.1.1).
                    //                Even so, this should be removed eventually, when it's safe to assume that there is
                    //                no longer any need for conversion.
                    SkybrudNestedContentHelper.ConvertItemValueFromV011(item, propertyType.DataTypeId, ref preValueCollection);

                    var contentTypeAlias = SkybrudNestedContentHelper.GetContentTypeAliasFromItem(item);
                    if (string.IsNullOrEmpty(contentTypeAlias))
                    {
                        continue;
                    }

                    var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, contentTypeAlias);
                    if (publishedContentType == null)
                    {
                        continue;
                    }

                    var propValues = item.ToObject<Dictionary<string, object>>();
                    var properties = new List<IPublishedProperty>();

                    foreach (var jProp in propValues)
                    {
                        var propType = publishedContentType.GetPropertyType(jProp.Key);
                        if (propType != null)
                        {
                            properties.Add(new SkybrudDetachedPublishedProperty(propType, jProp.Value, preview));
                        }
                    }

                    // Parse out the name manually
                    object nameObj = null;
                    if (propValues.TryGetValue("name", out nameObj))
                    {
                        // Do nothing, we just want to parse out the name if we can
                    }

                    object keyObj;
                    var key = Guid.Empty;
                    if (propValues.TryGetValue("key", out keyObj))
                    {
                        key = Guid.Parse(keyObj.ToString());
                    }

                    // Get the current request node we are embedded in
                    var pcr = UmbracoContext.Current == null ? null : UmbracoContext.Current.PublishedContentRequest;
                    var containerNode = pcr != null && pcr.HasPublishedContent ? pcr.PublishedContent : null;

                    // Create the model based on our implementation of IPublishedContent
                    IPublishedContent content = new DetachedPublishedContent(
                        key,
                        nameObj == null ? null : nameObj.ToString(),
                        publishedContentType,
                        properties.ToArray(),
                        containerNode,
                        i,
                        preview);

                    if (PublishedContentModelFactoryResolver.HasCurrent && PublishedContentModelFactoryResolver.Current.HasValue)
                    {
                        // Let the current model factory create a typed model to wrap our model
                        content = PublishedContentModelFactoryResolver.Current.Factory.CreateModel(content);
                    }

                    // Add the (typed) model as a result
                    items.Add(content);
                }
                
                return items;
            }
       

        
           return items;
        }

        public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return null;
            }

            var sourceItems = (List<IPublishedContent>)source;
            var items = new List<object>();

            if (sourceItems.Any())
            {
                foreach (var item in sourceItems)
                {
                    if (item != null)
                    {
                        switch (item.DocumentTypeAlias)
                        {
                            /*
                                ADD YOUR NESTED CONTENT TYPES HERE IN THIS FORMAT
                                case "imageWithLink": 
                                    items.Add(new ImageWithLink(item));
                                break;
                                */
                        }

                    }
                }
               
                    return items;
               
            }

            return source;

        }

        public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            return source.ToString();
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof(IEnumerable<object>);
        
        }

     


        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            PropertyCacheLevel returnLevel;
            switch (cacheValue)
            {
                case PropertyCacheValue.Object:
                    returnLevel = PropertyCacheLevel.ContentCache;
                    break;
                case PropertyCacheValue.Source:
                    returnLevel = PropertyCacheLevel.Content;
                    break;
                case PropertyCacheValue.XPath:
                    returnLevel = PropertyCacheLevel.Content;
                    break;
                default:
                    returnLevel = PropertyCacheLevel.None;
                    break;
            }

            return returnLevel;
        }


    }
}
