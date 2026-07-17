using System.Text.Json.Serialization;

namespace DScratch.Client.BrowserInteractions.Metadata;

[JsonConverter(typeof(JsonStringEnumConverter<MetadataDelete>))]
public enum MetadataDelete
{
    [JsonStringEnumMemberName("onSelectionChange")]
    OnSelectionChange,
    
    [JsonStringEnumMemberName("onTyping")]
    OnTyping
}