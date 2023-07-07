using System;
using System.Collections.Generic;

namespace essential_wow;

public partial class Map
{
    public long Id { get; set; }

    public long Floor { get; set; }

    public string Dungeon { get; set; } = null!;

    public virtual ICollection<Block> Blocks { get; set; } = new List<Block>();

    public virtual Dungeon DungeonNavigation { get; set; } = null!;
}
