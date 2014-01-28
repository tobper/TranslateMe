using System.Runtime.Serialization;

namespace TranslateMe.Model.GitHub
{
    [DataContract]
    public class GitHubRelease
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "tag_name")]
        public string TagName { get; set; }

        [DataMember(Name = "assets")]
        public GitHubAsset[] Assets { get; set; }
    }
}