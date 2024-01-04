using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class Role
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UserGuest> UserGuests { get; set; } = new List<UserGuest>();
}
