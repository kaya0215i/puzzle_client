using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFieldDataRequest {
    [JsonProperty("character_type")] public CharacterManager.CHARACTER_TYPE CharacterType { get; set; }
    [JsonProperty("rank_id")] public int RankId { get; set; }
    [JsonProperty("round")] public int CurrentRound { get; set; }

    [JsonProperty("index")] public List<int> IndexNum { get; set; }
    [JsonProperty("item_id")] public List<int> ItemNum { get; set; }
    [JsonProperty("piece_id")] public List<int> PieceFormId { get; set; }
    [JsonProperty("piece_angle")] public List<Quaternion> PieceAngle { get; set; }
}
