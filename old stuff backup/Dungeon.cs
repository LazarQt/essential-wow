using System;
using System.Collections.Generic;

namespace essential_wow;

public partial class Dungeon
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Map> Maps { get; set; } = new List<Map>();
}
