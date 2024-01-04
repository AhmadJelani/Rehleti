using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rehleti.Models;

public partial class AdventureTrip
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
    [NotMapped]
    public IFormFile PDFFile { get; set; }
    public string? ProofOfCompanyOwner { get; set; }

    public DateTime? JoinDate { get; set; }

    public int? NumberOfGuests { get; set; }

    public decimal? Price { get; set; }

    public decimal? CompanyOwnerId { get; set; }

    public DateTime? DateTo { get; set; }

    public DateTime? DateFrom { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
    public string? ImagePath { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BookTrip> BookTrips { get; set; } = new List<BookTrip>();

    public virtual UserGuest? CompanyOwner { get; set; }

    public virtual ICollection<TripFeedback> TripFeedbacks { get; set; } = new List<TripFeedback>();
}
