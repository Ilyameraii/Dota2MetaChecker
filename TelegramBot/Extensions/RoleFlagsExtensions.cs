using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class RoleFlagsExtensions
{
    public static string ToDisplayName(this RoleFlags flag, RoleFlags selectedFlags)
    {
        var baseText = flag switch
        {
            RoleFlags.Safelane => "Safelane",
            RoleFlags.Midlane => "Midlane",
            RoleFlags.Offlane => "Offlane",
            RoleFlags.Support => "Support",
            RoleFlags.HardSupport => "Hard Support",
            _ => "?"
        };
        return selectedFlags.HasFlag(flag) ? "✅ " + baseText : baseText;
    }
}