using System.Text.Json.Serialization;

namespace DScratch.Interactions;

[JsonConverter(typeof(JsonStringEnumConverter<SelectionDirection>))]
public enum SelectionDirection
{
    [JsonStringEnumMemberName("none")]
    None,
    
    [JsonStringEnumMemberName("backward")]
    Backward,
    
    [JsonStringEnumMemberName("forward")]
    Forward
}