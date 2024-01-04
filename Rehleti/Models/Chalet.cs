using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rehleti.Models;

public partial class Chalet
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
    [NotMapped]
    public IFormFile PDFFile { get; set; }
    public string? ProofOfOwner { get; set; }

    public decimal? Rating { get; set; }

    public DateTime? JoinDate { get; set; }

    public int? NumberOfGuests { get; set; }

    public decimal? Price { get; set; }

    public string? LacationLatitude { get; set; }

    public decimal? OwnerId { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
    public string? ImagePath { get; set; }

    public string? LacationLongitude { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BookChalet> BookChalets { get; set; } = new List<BookChalet>();

    public virtual ICollection<ChaletFeedback> ChaletFeedbacks { get; set; } = new List<ChaletFeedback>();

    public virtual ICollection<ListOfDatesForChalet> ListOfDatesForChalets { get; set; } = new List<ListOfDatesForChalet>();

    public virtual UserGuest? Owner { get; set; }
}
