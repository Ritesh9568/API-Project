using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Models;

public partial class Beneficiary
{
    public long Beneficiaryid { get; set; }

    public string CustomeridFk { get; set; } = null!;

    public string Beneficiaryname { get; set; } = null!;

    public string Beneficiaryaccountnum { get; set; } = null!;

    public string Beneficiaryifsc { get; set; } = null!;

    public DateTime? Addedon { get; set; }

    public virtual Customer CustomeridFkNavigation { get; set; } = null!;
}
