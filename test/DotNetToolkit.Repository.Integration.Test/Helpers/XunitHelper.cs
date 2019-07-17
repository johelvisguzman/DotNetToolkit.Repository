namespace DotNetToolkit.Repository.Integration.Test.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    public static class XunitHelper
    {
        public static IEnumerable<MethodInfo> GetAllTestingMethods(Type testClassType)
        {
            var methods = testClassType.GetMethods();

            foreach (var m in methods)
            {
                var methodAttr = m.GetCustomAttribute(typeof(FactAttribute));

                if (methodAttr != null)
                {
                    yield return m;
                }
            }
        }
    }
}
