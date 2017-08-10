using Newtonsoft.Json;

namespace ADB
{
    [JsonObject]
    public class UrbanDictList
    {
        [JsonProperty(PropertyName = "definition")]
        public string Definition { get; set; }

        [JsonProperty(PropertyName = "permalink")]
        public string Permalink { get; set; }

        [JsonProperty(PropertyName = "thumbs_up")]
        public int ThumbsUpCount { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "word")]
        public string Word { get; set; }

        [JsonProperty(PropertyName = "defid")]
        public string Defid { get; set; }

        [JsonProperty(PropertyName = "current_vote")]
        public string CurrentVote { get; set; }

        [JsonProperty(PropertyName = "example")]
        public string Example { get; set; }

        [JsonProperty(PropertyName = "thumbs_down")]
        public int ThumbsDownCount { get; set; }
    }
}