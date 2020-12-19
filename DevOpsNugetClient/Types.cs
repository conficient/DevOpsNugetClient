using System;
using System.Text.Json.Serialization;

namespace DevOpsClient
{

    public class Package
    {
        public string id { get; set; }
        public string normalizedName { get; set; }
        public string name { get; set; }
        public string protocolType { get; set; }
        public string url { get; set; }
        public LinkRef[] versions { get; set; }
        
        [JsonPropertyName("_links")]
        public Links links { get; set; }
    }


    public class Version
    {
        public string id { get; set; }
        public string normalizedVersion { get; set; }
        public string version { get; set; }
        public bool isLatest { get; set; }
        public bool isListed { get; set; }
        public string storageId { get; set; }
        public View[] views { get; set; }
        public DateTime publishDate { get; set; }
    }

    public class View
    {
        public string id { get; set; }
        public string name { get; set; }
        public object url { get; set; }
        public string type { get; set; }
    }

    public class Feed
    {
        public string description { get; set; }
        public string url { get; set; }
        [JsonPropertyName("_links")] public Links links { get; set; }
        public bool hideDeletedPackageVersions { get; set; }
        public bool badgesEnabled { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public object viewId { get; set; }
        public object viewName { get; set; }
        public string fullyQualifiedName { get; set; }
        public string fullyQualifiedId { get; set; }
        public object[] upstreamSources { get; set; }
        public string capabilities { get; set; }
    }

    public class Links
    {
        public LinkRef self { get; set; }

        //public Self self { get; set; }
        public LinkRef packages { get; set; }
        public LinkRef permissions { get; set; }
    }

    public class LinkRef
    {
        public string href { get; set; }
    }


    public class PackageVersion
    {
        public string author { get; set; }
        public string description { get; set; }
        public DateTime? deletedDate { get; set; }
        public ProtocolMetadata protocolMetadata { get; set; }
        public object[] tags { get; set; }
        public string url { get; set; }
        public Dependency[] dependencies { get; set; }
        [JsonPropertyName("_links")] public Links links { get; set; }
        public object[] sourceChain { get; set; }
        public string id { get; set; }
        public string normalizedVersion { get; set; }
        public string version { get; set; }
        public bool isLatest { get; set; }
        public bool isDeleted { get; set; }
        public View[] views { get; set; }
        public DateTime publishDate { get; set; }
        public bool isListed { get; set; }
        public string storageId { get; set; }
    }

    public class ProtocolMetadata
    {
        public int schemaVersion { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string copyright { get; set; }
        public string releaseNotes { get; set; }
        public bool requireLicenseAcceptance { get; set; }
    }

    public class Dependency
    {
        public string packageName { get; set; }
        public string group { get; set; }
        public string versionRange { get; set; }
    }

}
