using Entities.Classes;
using Entities.Enums;

namespace Services.Contracts.Models;

public class HeroProcessingOptions
{
    public RankFlags Ranks { get; init; } = RankFlags.None;
    
    public RoleFlags Roles { get; init; } = RoleFlags.None;
    
    public Func<IEnumerable<Hero>, IOrderedEnumerable<Hero>>? SortBy { get; init; }
}