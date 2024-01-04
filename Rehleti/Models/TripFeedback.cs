using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class TripFeedback
{
    public decimal Id { get; set; }

    public decimal? TripId { get; set; }

    public string? Text { get; set; }

    public decimal? UserId { get; set; }

    public virtual AdventureTrip? Trip { get; set; }

    public virtual UserGuest? User { get; set; }
}
