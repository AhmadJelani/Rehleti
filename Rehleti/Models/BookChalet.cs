using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class BookChalet
{
    public decimal Id { get; set; }

    public decimal? ChaletId { get; set; }

    public decimal? UserId { get; set; }

    public decimal? DateId { get; set; }

    public virtual Chalet? Chalet { get; set; }

    public virtual ListOfDatesForChalet? Date { get; set; }

    public virtual UserGuest? User { get; set; }
}
