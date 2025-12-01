using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Models;

public partial class Transaction
{
    public long Transactionid { get; set; }

    public string AccountidFk { get; set; } = null!;

    public string Transactiontype { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime Transactiontimestamp { get; set; }

    public string Referencenumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Account AccountidFkNavigation { get; set; } = null!;
    public DateTime Lastupdated { get; internal set; }
}
