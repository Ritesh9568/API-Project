using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Models;

public partial class Branch
{
    public string Branchid { get; set; } = null!;

    public string Branchname { get; set; } = null!;

    public string Ifsccode { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public DateOnly Dateopened { get; set; }

    public DateTime? Lastupdated { get; set; }
}
