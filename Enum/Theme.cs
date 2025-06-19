using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AudioSwitch.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum Theme
{
    Dark,
    Light,
    System
}