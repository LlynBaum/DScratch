using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DScratch.Client.BrowserInteractions;

[JsonConverter(typeof(JsonStringEnumConverter<SelectionDirection>))]
public enum SelectionDirection
{
    [EnumMember(Value = "none")]
    None,
    
    [EnumMember(Value = "backward")]
    Backward,
    
    [EnumMember(Value = "forward")]
    Forward
}