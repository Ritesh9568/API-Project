using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Models;

public partial class Customer
{
    public string Customerid { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public DateOnly Dateofbirth { get; set; }

    public string? Address { get; set; }

    public string Mobilenumber { get; set; } = null!;

    public bool Isactive { get; set; }

    public string Kycstatus { get; set; } = null!;

    public DateTime? Lastupdated { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

 

    public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();
}
