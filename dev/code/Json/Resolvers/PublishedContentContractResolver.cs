using System;
using System.Reflection;
using code.Grid.Spa;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Skybrud.Essentials.Strings;
using Skybrud.Umbraco.GridData;

namespace code.Json.Resolvers
{
    public class PublishedContentContractResolver : DefaultContractResolver
    {
        public new static readonly PublishedContentContractResolver Instance = new PublishedContentContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);


            var pi = member as PropertyInfo; 

            //if (pi != null && pi.PropertyType == typeof(CmSeperator))
            //{
            //    property.Ignored = true;
            //}

	        

			property.ShouldSerialize = instance =>
            {

                if (property.PropertyName == "CompositionAliases") return false;
                if (property.PropertyName == "ContentSet") return false;
                if (property.PropertyName == "PropertyTypes") return false;
                if (property.PropertyName == "Properties") return false;
                if (property.PropertyName == "Parent") return false;
                if (property.PropertyName == "Children") return false;
                if (property.PropertyName == "DocumentTypeId") return false;
                if (property.PropertyName == "WriterName") return false;
                if (property.PropertyName == "CreatorName") return false;
                if (property.PropertyName == "WriterId") return false;
                if (property.PropertyName == "CreatorId") return false;
                if (property.PropertyName == "CreateDate") return false;
                if (property.PropertyName == "UpdateDate") return false;
                if (property.PropertyName == "Version") return false;
                if (property.PropertyName == "SortOrder") return false;
                if (property.PropertyName == "TemplateId") return false;
                if (property.PropertyName == "IsDraft") return false;
                if (property.PropertyName == "ItemType") return false;
                if (property.PropertyName == "ContentType") return false;
                if (property.PropertyName == "ContentSet") return false;
                if (property.PropertyName == "Path") return false; //override path with patharray to make it an array
                if (property.PropertyName == "SeoMetaDescription") return false;
                if (property.PropertyName == "Seodashboard") return false;
                if (property.PropertyName == "Preview") return false;
                if (property.PropertyName == "SeoTitle") return false;
                //ADD CUSTOM OVERRRIDES AFTER THIS IN THE ABOVE FORMAT


				property.PropertyName = StringUtils.ToCamelCase(property.PropertyName);

                return true;
            };

            return property;
        }
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            // this will only be called once and then cached
            if (objectType == typeof(GridDataModel))
            {
                contract.Converter = new SpaGridJsonConverter();
            }

            return contract;
        }
    }
}
