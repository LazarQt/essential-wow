using System;
using System.Collections.Generic;

namespace essential_wow;

public partial class Block
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long LocX { get; set; }

    public long LocY { get; set; }

    public long? MapId { get; set; }

    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

    public virtual Map? Map { get; set; }
}
