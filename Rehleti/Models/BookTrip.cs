using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class BookTrip
{
    public decimal Id { get; set; }

    public decimal? UserId { get; set; }

    public decimal? AdventureTripId { get; set; }

    public virtual AdventureTrip? AdventureTrip { get; set; }

    public virtual UserGuest? User { get; set; }
}
