using Newtonsoft.Json;

namespace ensek_spark.Models;

public class UserAccount
{
    [JsonProperty("id")]
    public string AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
