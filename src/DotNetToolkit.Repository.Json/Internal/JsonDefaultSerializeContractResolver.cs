namespace DotNetToolkit.Repository.Json.Internal
{
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal class JsonDefaultSerializeContractResolver : CamelCasePropertyNamesContractResolver
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
