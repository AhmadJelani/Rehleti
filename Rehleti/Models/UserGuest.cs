using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rehleti.Models;

public partial class UserGuest
{
    public decimal Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? JoinDate { get; set; }

    public decimal? RoleId { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public string? ImagePath { get; set; }

    public virtual ICollection<AdventureTrip> AdventureTrips { get; set; } = new List<AdventureTrip>();

    public virtual ICollection<BookChalet> BookChalets { get; set; } = new List<BookChalet>();

    public virtual ICollection<BookTrip> BookTrips { get; set; } = new List<BookTrip>();

    public virtual ICollection<Chalet> Chalets { get; set; } = new List<Chalet>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<ChaletFeedback> ChaletFeedbacks { get; set; } = new List<ChaletFeedback>();

    public virtual ICollection<TripFeedback> TripFeedbacks { get; set; } = new List<TripFeedback>();

}
