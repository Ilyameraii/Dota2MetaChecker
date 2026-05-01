using Dota2MetaChecker.Common.Enums;

namespace Services.Extensions;

public static class RoleExtensions
{
    public static bool IsIncludedIn(this Role role, RoleFlags flags)
    {
        var roleFlag = role switch
        {
            Role.Safelane => RoleFlags.Safelane,
            Role.Midlane => RoleFlags.Midlane,
            Role.Offlane => RoleFlags.Offlane,
            Role.Support => RoleFlags.Support,
            Role.HardSupport => RoleFlags.HardSupport,
            _ => RoleFlags.None
        };
        return flags.HasFlag(roleFlag);
    }
}