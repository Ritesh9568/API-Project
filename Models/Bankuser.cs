using System;
using System.Collections.Generic;

namespace CoreBankingAPI.Models;

public partial class Bankuser
{
    public string BranchidFk { get; set; }   // FK column in table

    public Branch Branch { get; set; }       // Navigation property

    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool Isactive { get; set; }

    public DateTime? Lastloggedin { get; set; }

    public DateTime? Createdon { get; set; }
}
