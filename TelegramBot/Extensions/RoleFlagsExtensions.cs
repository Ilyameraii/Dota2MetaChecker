using Dota2MetaChecker.Common.Enums;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class RoleFlagsExtensions
{
    public static string ToDisplayName(this RoleFlags roleFlag, RoleFlags selectedFlags)
    {
        var baseText = roleFlag switch
        {
            RoleFlags.Safelane => "Керри",
            RoleFlags.Midlane => "Мидер",
            RoleFlags.Offlane => "Оффлейнер",
            RoleFlags.Support => "Роумер",
            RoleFlags.HardSupport => "Поддержка",
            _ => "?"
        };
        return selectedFlags.HasFlag(roleFlag) ? "✅ " + baseText : baseText;
    }
}