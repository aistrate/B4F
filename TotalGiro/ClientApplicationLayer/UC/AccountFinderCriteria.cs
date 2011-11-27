namespace B4F.TotalGiro.ClientApplicationLayer.UC
{
    public class AccountFinderCriteria
    {
        public int AssetManagerId { get; set; }
        public int RemisierId { get; set; }
        public int RemisierEmployeeId { get; set; }
        public int ModelPortfolioId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public bool AccountStatusActive { get; set; }
        public bool AccountStatusInactive { get; set; }
        public bool EmailStatusYes { get; set; }
        public bool EmailStatusNo { get; set; }
        public string LoginStatus { get; set; }
    }
}
