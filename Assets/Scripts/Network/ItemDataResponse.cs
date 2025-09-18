using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataResponse {
    [JsonProperty("id")] public int id;
    [JsonProperty("name")] public string name;
    [JsonProperty("is_weapon")] public bool isWeapon;
    [JsonProperty("amount")] public float amount;
    [JsonProperty("energy_up")] public float energyUp;
    [JsonProperty("energy_cost")] public float energyCost;
    [JsonProperty("cool_time")] public float cooltime;
    [JsonProperty("text")] public string descriptionText;
    [JsonProperty("price")] public int price;
}

public class ItemDataRoot {
    [JsonProperty("data")] public List<ItemDataResponse> data;
}
