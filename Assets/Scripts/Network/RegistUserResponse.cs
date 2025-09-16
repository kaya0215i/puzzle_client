using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistUserResponse {
    [JsonProperty("token")]
    public string APIToken { get; set; }
}
