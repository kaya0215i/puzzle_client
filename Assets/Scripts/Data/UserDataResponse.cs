using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataResponse {
    [JsonProperty("name")]
    public string UserName { get; set; }
    [JsonProperty("rank_id")]
    public int RankId { get; set; }
    [JsonProperty("rank_point")]
    public int RankPoint {  get; set; }
}
