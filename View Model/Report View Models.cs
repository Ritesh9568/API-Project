using System;
using System.ComponentModel.DataAnnotations;

namespace CoreBankingAPI.Models
{
    // --- 1. Account Controller ---
    public class AccountReportViewModel
    {
        // Account Details (Primary Entity)
        public string AccountNumber { get; set; } = default!;
        public string AccountType { get; set; } = default!;
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }

        // Customer Details (Inner Join)
        public string CustomerId { get; set; } = default!;
        public string CustomerFullName { get; set; } = default!;

        // Loan Details (Left Join)
        public decimal? LoanAmount { get; set; }
        public string? LoanStatus { get; set; }
    }

    // --- 2. Customer Controller ---
    public class CustomerReportViewModel
    {
        // Customer Details (Primary Entity)
        public string CustomerId { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string KycStatus { get; set; } = default!;

        // Assigned Bank User / Relationship Manager (Inner Join)
        public int AssignedUserId { get; set; }
        public string AssignedUserRole { get; set; } = default!;

        // Primary Address (Left Join)
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }

    // --- 3. BankUser Controller ---
    public class BankUserReportViewModel
    {
        // Bankuser properties
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastLoggedIn { get; set; }

        // Branch properties from the join
        public string BranchId { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string IfscCode { get; set; } = string.Empty;
        public string BranchCity { get; set; } = string.Empty;
    }

    // --- 4. Beneficiary Controller ---
    public class BeneficiaryReportViewModel
    {
        // Beneficiary Details (Primary Entity)
        public long BeneficiaryId { get; set; }
        public string Nickname { get; set; } = default!;
        public string ExternalAccountNumber { get; set; } = default!;

        // Originating Account Details (Inner Join)
        public string OriginatingAccountNumber { get; set; } = default!;
        public string OriginatingAccountType { get; set; } = default!;

        // Transfer Limits (Left Join)
        public decimal? MaxTransferLimit { get; set; }
    }


    // --- 6. Transaction Controller ---
    public class TransactionReportViewModel
    {
        // Transaction Details (Primary Entity)
        public long TransactionId { get; set; }
        public DateTime TrDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = default!; // DEBIT/CREDIT

        // Account Details (Inner Join)
        public string AccountNumber { get; set; } = default!;
        public string CustomerId { get; set; } = default!;

    }
}