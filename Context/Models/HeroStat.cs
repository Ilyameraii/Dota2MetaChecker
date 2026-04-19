using System;
using System.Collections.Generic;

namespace Context.Models;

public partial class HeroStat
{
    public int Id { get; set; }

    public int HeroId { get; set; }

    public string HeroRank { get; set; } = null!;

    public string HeroRole { get; set; } = null!;

    public int WinCount { get; set; }

    public int MatchCount { get; set; }

    public int MetaUpdateId { get; set; }

    public virtual MetaUpdate MetaUpdate { get; set; } = null!;
}
