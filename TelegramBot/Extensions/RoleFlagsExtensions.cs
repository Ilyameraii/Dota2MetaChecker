using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class RoleFlagsExtensions
{
    public static string ToDisplayName(this RoleFlags roleFlag, RoleFlags selectedFlags)
    {
        var baseText = roleFlag switch
        {
            RoleFlags.Safelane => "Safelane",
            RoleFlags.Midlane => "Midlane",
            RoleFlags.Offlane => "Offlane",
            RoleFlags.Support => "Support",
            RoleFlags.HardSupport => "Hard Support",
            _ => "?"
        };
        return selectedFlags.HasFlag(roleFlag) ? "✅ " + baseText : baseText;
    }
}