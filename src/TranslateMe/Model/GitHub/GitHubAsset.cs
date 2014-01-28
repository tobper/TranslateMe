using System.Runtime.Serialization;

namespace TranslateMe.Model.GitHub
{
    [DataContract]
    public class GitHubAsset
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}