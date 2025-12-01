namespace CoreBankingAPI.View_Model
{
    public class CustomerDetailViewModel
    {
        /// <summary>Core Customer Details for a single profile.</summary>
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; } // Simplified to string for output
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string KycStatus { get; set; }

        /// <summary>Nested collection (JOIN) of the customer's accounts.</summary>
        public List<AccountSummaryViewModel> Accounts { get; set; }

    }
}
