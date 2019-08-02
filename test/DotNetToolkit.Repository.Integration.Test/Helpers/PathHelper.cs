namespace DotNetToolkit.Repository.Integration.Test.Helpers
{
    using System.IO;

    public class PathHelper
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
