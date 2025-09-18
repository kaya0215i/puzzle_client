using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFriendResponse {
    [JsonProperty("user_name")]
    public List<string> Name { get; set; }
    [JsonProperty("rank_id")]
    public List<int> RankId { get; set; }
    [JsonProperty("rank_point")]
    public List<int> RankPoint { get; set; }
}
