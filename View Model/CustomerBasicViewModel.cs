using CoreBankingAPI.Models;

namespace CoreBankingAPI.View_Model
{
    public class CustomerBasicViewModel
    {
        public string CustomerId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string MobileNumber { get; set; }
        public string KycStatus { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
