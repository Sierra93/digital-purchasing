namespace DigitalPurchasing.Models
{
    public class Company : BaseModel
    {
        public string Name { get; set; }
        public string InvitationCode { get; set; }
        public bool IsSODeleteEnabled { get; set; }
    }
}
