namespace CoreBankingAPI.View_Model
{
    public class TransactionHistoryViewModel
    {
        /// <summary>Transaction Details.</summary>
        public long TransactionId { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionTimestamp { get; set; }
        public string Status { get; set; }

        /// <summary>Joined Account Details.</summary>
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
    }
}
