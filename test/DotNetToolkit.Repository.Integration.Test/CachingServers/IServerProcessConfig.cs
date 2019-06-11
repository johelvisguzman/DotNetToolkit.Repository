namespace DotNetToolkit.Repository.Integration.Test.CachingServers
{
    public interface IServerProcessConfig
    {
        string Args { get; }
        string BasePath { get; }
        string Exe { get; }
    }
}
