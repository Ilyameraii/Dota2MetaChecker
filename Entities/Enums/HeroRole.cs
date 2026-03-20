using System.Runtime.Serialization;

namespace Entities.Enums;

public enum HeroRole
{
    [EnumMember(Value = "POSITION_1")] Safelane,
    [EnumMember(Value = "POSITION_2")] Midlane,
    [EnumMember(Value = "POSITION_3")] Offlane,
    [EnumMember(Value = "POSITION_4")] Support,
    [EnumMember(Value = "POSITION_5")] HardSupport,
}