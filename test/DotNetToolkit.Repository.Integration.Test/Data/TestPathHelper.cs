namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System.IO;

    public class TestPathHelper
    {
        public static string GetTempFileName()
        {
             var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (!File.Exists(tempPath))
                File.Create(tempPath).Dispose();

            return tempPath;
        }
    }
}
