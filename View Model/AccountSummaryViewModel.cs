namespace CoreBankingAPI.View_Model
{
    public class AccountSummaryViewModel
    {
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal CurrentBalance { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerMobileNumber { get; set; }
    }
}
