namespace CoreBankingAPI.View_Model
{
    public class TransactionReportViewModel
    {
        public DateTime TransactionTimestamp { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Amount { get; set; }
    }
}
