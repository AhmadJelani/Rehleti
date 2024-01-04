using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class ChaletFeedback
{
    public decimal Id { get; set; }

    public decimal? ChaletId { get; set; }

    public string? Text { get; set; }

    public decimal? UserId { get; set; }
    public decimal? Rate { get; set; }

    public virtual Chalet? Chalet { get; set; }

    public virtual UserGuest? User { get; set; }
}
