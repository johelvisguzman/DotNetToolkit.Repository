namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Reflection;

    internal class DefaultJsonSerializeContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // Checks to see if this is a complex type
            if (((PropertyInfo)member).PropertyType.Namespace != "System")
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
