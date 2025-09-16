using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistUserRequest {
    [JsonProperty("name")]
    public string Name { get; set; }
}
