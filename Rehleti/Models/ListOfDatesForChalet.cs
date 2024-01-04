using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class ListOfDatesForChalet
{
    public decimal Id { get; set; }

    public decimal? ChaletId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BookChalet> BookChalets { get; set; } = new List<BookChalet>();

    public virtual Chalet? Chalet { get; set; }
}
