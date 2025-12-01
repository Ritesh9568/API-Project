using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Models;

public partial class Account
{
    public string Accountnumber { get; set; } = null!;

    public string Customer { get; set; } = null!;

    public string Accounttype { get; set; } = null!;

    public decimal CurrentBalance { get; set; }
    public decimal OverdraftLimit { get; set; }

    public DateOnly Opendate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? Lastupdated { get; set; }

    public virtual Customer CustomeridFkNavigation { get; set; } = null!;

    

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
