using System;
using System.Collections.Generic;

namespace essential_wow;

public partial class Entry
{
    public long Id { get; set; }

    public string Type { get; set; } = null!;

    public long ExtId { get; set; }

    public string Action { get; set; } = null!;

    public long? BlockId { get; set; }

    public virtual Block? Block { get; set; }
}
