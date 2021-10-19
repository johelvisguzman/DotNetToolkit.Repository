namespace DotNetToolkit.Repository.Integration.Test.Helpers
{
    using Couchbase;
    using Couchbase.Configuration.Client;
    using System;
    using System.Collections.Generic;

    public static class CouchbaseHelper
    {
        public static void ClearDatabase(string host, string username, string password, string bucketName)
        {
            var config = new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(host) },
                BucketConfigs = new Dictionary<string, BucketConfiguration>
                {
                    { bucketName, new BucketConfiguration
                        {
                            Username = username,
                            Password = password,
                            BucketName = bucketName
                        }
                    }
                }
            };

            using (var cluster = new Cluster(config))
            using (var bucket = cluster.OpenBucket())
            {
                var result = bucket.CreateManager(username, password).Flush();
            }
        }
    }
}
