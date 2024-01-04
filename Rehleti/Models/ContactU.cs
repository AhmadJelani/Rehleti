using System;
using System.Collections.Generic;

namespace Rehleti.Models;

public partial class ContactU
{
    public decimal Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Subject { get; set; }

    public string? Text { get; set; }
}
